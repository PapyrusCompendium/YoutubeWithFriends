using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using YoutubeWithFriends.Api.Data;
using YoutubeWithFriends.Api.Services;
using YoutubeWithFriends.Db;
using YoutubeWithFriends.Db.Models;

namespace YoutubeWithFriends.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase {
        private const string ROOM_SESSION_ID_COOKIE_NAME = "roomSessionId",
            COMMA_SEPERATOR = ",";

        private readonly ISimpleDbContextFactory _simpleDbContextFactory;
        private readonly IIpAddressResolver _ipAddressResolver;

        public RoomsController(ISimpleDbContextFactory simpleDbContextFactory, IIpAddressResolver ipAddressResolver) {
            _simpleDbContextFactory = simpleDbContextFactory;
            _ipAddressResolver = ipAddressResolver;
        }

        [HttpPost("CreateRoom")]
        public async Task<ActionResult<string>> CreateRoom(string roomName) {
            var sessionId = Request.Cookies[Program.USER_SESSION_ID_COOKIE_NAME];

            if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(sessionId)) {
                return BadRequest();
            }

            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var ipAddress = _ipAddressResolver.GetIpAddress(Request);
            var userSession = await context.Users.FirstOrDefaultAsync(i => i.SessionID == sessionId);
            if (userSession is null || ipAddress != userSession.IpAddress) {
                return BadRequest();
            }

            var ownedRoom = await context.Rooms.FirstOrDefaultAsync(i => i.RoomOwnerId == userSession.ID);
            if (ownedRoom is not null) {
                Response.Cookies.Append(ROOM_SESSION_ID_COOKIE_NAME, ownedRoom.ID.ToString());
                return Ok(new { roomId = ownedRoom.ToString(), roomName = ownedRoom.RoomName });
            }

            var roomId = Guid.NewGuid();
            context.Rooms.Add(new Room {
                ID = roomId,
                RoomName = roomName,
                OwnerSessionId = sessionId,
                RoomOwnerId = userSession.ID,
                JoinedUserSessionIds = string.Empty
            });

            await context.SaveChangesAsync();

            return Ok(new { roomId = roomId.ToString(), roomName = roomName });
        }

        [HttpGet("Information")]
        public async Task<ActionResult<string>> GetRoomDetails(string roomId) {
            if (string.IsNullOrWhiteSpace(roomId) || !Guid.TryParse(roomId, out var roomGuid)) {
                return BadRequest();
            }

            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var roomSession = await context.Rooms
                .Include(i => i.RoomOwner)
                .FirstOrDefaultAsync(i => i.ID == roomGuid);

            if (roomSession is null) {
                return BadRequest();
            }

            return Ok(new {
                roomId = roomSession.ID,
                roomName = roomSession.RoomName,
                roomOwner = roomSession.RoomOwner.Username,
                roomCreated = roomSession.CreatedDate,
                joinedUsers = roomSession.JoinedUserSessionIds.Split(COMMA_SEPERATOR),
            });
        }

        [HttpPut("JoinRoom")]
        public async Task<ActionResult<string>> JoinRoom(string roomId) {
            var sessionId = Request.Cookies[Program.USER_SESSION_ID_COOKIE_NAME];

            if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(roomId)
                || !Guid.TryParse(roomId, out var roomGuid)) {
                return BadRequest();
            }
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var room = await context.Rooms.FirstOrDefaultAsync(i => i.ID == roomGuid);
            if (room is null) {
                return BadRequest();
            }

            var userIds = room.JoinedUserSessionIds.Split(COMMA_SEPERATOR).Where(i => !string.IsNullOrWhiteSpace(i));
            if (userIds.Contains(sessionId, StringComparer.OrdinalIgnoreCase)) {
                return BadRequest();
            }

            var newUsers = userIds.ToList();
            newUsers.Add(sessionId);
            room.JoinedUserSessionIds = string.Join(COMMA_SEPERATOR, newUsers);
            await context.SaveChangesAsync();

            return Ok(room.ID.ToString());
        }

        [HttpDelete("LeaveRoom")]
        public async Task<ActionResult<string>> LeaveRoom(string roomId) {
            var sessionId = Request.Cookies[Program.USER_SESSION_ID_COOKIE_NAME];

            if (string.IsNullOrWhiteSpace(sessionId) || string.IsNullOrWhiteSpace(roomId)
                || !Guid.TryParse(roomId, out var roomGuid)) {
                return BadRequest();
            }
            using var context = _simpleDbContextFactory.CreateContext<DbApiContext>();

            var room = await context.Rooms.FirstOrDefaultAsync(i => i.ID == roomGuid);
            if (room is null) {
                return BadRequest();
            }

            var userIds = room.JoinedUserSessionIds.Split(COMMA_SEPERATOR).Where(i => !string.IsNullOrWhiteSpace(i));
            if (!userIds.Contains(sessionId, StringComparer.OrdinalIgnoreCase)) {
                return BadRequest();
            }

            var newUsers = userIds.ToList();
            newUsers.Remove(sessionId);
            room.JoinedUserSessionIds = string.Join(COMMA_SEPERATOR, newUsers);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}