using System;
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
        private readonly ISimpleDbContextFactory _simpleDbContextFactory;

        public UsersController(ISimpleDbContextFactory simpleDbContextFactory) {
            _simpleDbContextFactory = simpleDbContextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetUsers(string sessionId) {
            var context = _simpleDbContextFactory.CreateContext<DbApiContext>();
            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);

            return userSession is null
                ? NotFound()
                : userSession.Username;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateUser(string username) {
            var context = _simpleDbContextFactory.CreateContext<DbApiContext>();
            var existingUser = await context.Users.FirstOrDefaultAsync(i => i.Username == username);

            if (existingUser is not null) {
                return new StatusCodeResult(400);
            }

            var sessionId = Guid.NewGuid().ToString();

            var user = new User {
                Username = username,
                SessionID = sessionId,
                CreatedDate = DateTimeOffset.Now,
                LastActivity = DateTimeOffset.Now,
                ID = new Guid()
            };
            context.Users.Add(user);
            context.Entry(user).State = EntityState.Added;

            context.SaveChanges();
            return Ok(sessionId);
        }

        [HttpDelete]
        public async Task<ActionResult<string>> Logout(string sessionId) {
            var context = _simpleDbContextFactory.CreateContext<DbApiContext>();
            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);

            if (userSession is null) {
                return new StatusCodeResult(400);
            }

            context.Users.Remove(userSession);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}