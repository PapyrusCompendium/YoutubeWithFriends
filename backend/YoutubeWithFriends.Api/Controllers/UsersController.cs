using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using YoutubeWithFriends.Api.Data;
using YoutubeWithFriends.Db;
using YoutubeWithFriends.Db.Models;

namespace YoutubeWithFriends.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private const int RATE_LIMIT_SPEED = 300;
        private const int MAX_ACCOUNTS_UNDER_IP = 5;

        private readonly ISimpleDbContextFactory _simpleDbContextFactory;

        public UsersController(ISimpleDbContextFactory simpleDbContextFactory) {
            _simpleDbContextFactory = simpleDbContextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetUsers(string sessionId) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();
            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);

            if (userSession is null) {
                return NotFound();
            }

            var ipAddress = GetIpAddress();
            if (string.IsNullOrEmpty(ipAddress) || userSession.IpAddress != ipAddress) {
                return BadRequest();
            }

            var now = DateTimeOffset.Now;
            if (now.Subtract(userSession.LastActivity).TotalMilliseconds < RATE_LIMIT_SPEED) {
                return new StatusCodeResult(429);
            }

            userSession.LastActivity = now;
            await context.SaveChangesAsync();

            return Ok(userSession.Username);
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateUser(string username) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var ipAddress = GetIpAddress();
            if (string.IsNullOrEmpty(ipAddress)) {
                return new StatusCodeResult(500);
            }

            var accountsUnderIp = context.Users.Count(i => i.IpAddress == ipAddress);
            if (accountsUnderIp > MAX_ACCOUNTS_UNDER_IP) {
                return new StatusCodeResult(500);
            }

            var existingUser = await context.Users.FirstOrDefaultAsync(i => i.Username == username);
            if (existingUser is not null) {
                return new StatusCodeResult(400);
            }

            var sessionId = Guid.NewGuid().ToString();
            context.Users.Add(new User {
                Username = username,
                SessionID = sessionId,
                IpAddress = ipAddress,
                CreatedDate = DateTimeOffset.Now,
                LastActivity = DateTimeOffset.Now,
                ID = new Guid()
            });

            await context.SaveChangesAsync();
            return Ok(sessionId);
        }

        [HttpDelete]
        public async Task<ActionResult<string>> Logout(string sessionId) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);
            if (userSession is null) {
                return new StatusCodeResult(400);
            }

            var ipAddress = GetIpAddress();
            if (string.IsNullOrEmpty(ipAddress) || userSession.IpAddress != ipAddress) {
                return BadRequest();
            }

            var now = DateTimeOffset.Now;
            if (now.Subtract(userSession.LastActivity).TotalMilliseconds < RATE_LIMIT_SPEED) {
                return new StatusCodeResult(429);
            }

            context.Users.Remove(userSession);
            await context.SaveChangesAsync();

            return Ok();
        }

        private string GetIpAddress() {
            if (Request.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault() is string forwardedAddress) {
                return forwardedAddress;
            }

            if (Request.Headers["REMOTE_ADDR"].FirstOrDefault() is string remoteAddress) {
                return remoteAddress;
            }

            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}