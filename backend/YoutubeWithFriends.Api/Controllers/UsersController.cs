using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

using YoutubeWithFriends.Api.Data;
using YoutubeWithFriends.Api.Services;
using YoutubeWithFriends.Db;
using YoutubeWithFriends.Db.Models;

namespace YoutubeWithFriends.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private const int MAX_ACCOUNTS_UNDER_IP = 5;
        private const string USER_SESSION_ID_COOKIE_NAME = "userSessionId";

        private readonly ISimpleDbContextFactory _simpleDbContextFactory;
        private readonly IIpAddressResolver _ipAddressResolver;

        public UsersController(ISimpleDbContextFactory simpleDbContextFactory, IIpAddressResolver ipAddressResolver) {
            _simpleDbContextFactory = simpleDbContextFactory;
            _ipAddressResolver = ipAddressResolver;
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<string>> GetUser(string sessionId) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            if (string.IsNullOrWhiteSpace(sessionId)) {
                return BadRequest();
            }

            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);

            if (userSession is null) {
                return NotFound();
            }

            var ipAddress = _ipAddressResolver.GetIpAddress(Request);
            if (string.IsNullOrEmpty(ipAddress) || userSession.IpAddress != ipAddress) {
                return BadRequest();
            }

            userSession.LastActivity = DateTimeOffset.Now;
            await context.SaveChangesAsync();

            return Ok(userSession.Username);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<string>> CreateUser(string username) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var ipAddress = _ipAddressResolver.GetIpAddress(Request);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(ipAddress)) {
                return BadRequest();
            }

            var accountsUnderIp = await context.Users.CountAsync(i => i.IpAddress == ipAddress);
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
                ID = Guid.NewGuid()
            });

            await context.SaveChangesAsync();

            Response.Cookies.Append(USER_SESSION_ID_COOKIE_NAME, sessionId);
            return Ok();
        }

        [HttpDelete("Logout")]
        public async Task<ActionResult<string>> Logout(string sessionId) {
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            if (string.IsNullOrWhiteSpace(sessionId)) {
                return BadRequest();
            }

            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);
            if (userSession is null) {
                return new StatusCodeResult(400);
            }

            var ipAddress = _ipAddressResolver.GetIpAddress(Request);
            if (string.IsNullOrEmpty(ipAddress) || userSession.IpAddress != ipAddress) {
                return BadRequest();
            }

            context.Users.Remove(userSession);
            await context.SaveChangesAsync();

            Response.Cookies.Delete(USER_SESSION_ID_COOKIE_NAME);
            return Ok();
        }
    }
}