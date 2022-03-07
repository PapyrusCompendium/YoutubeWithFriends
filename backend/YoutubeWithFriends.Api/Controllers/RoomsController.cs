using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using YoutubeWithFriends.Api.Data;
using YoutubeWithFriends.Db.Models;

namespace YoutubeWithFriends.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase {
        private readonly DbApiContext _context;

        public RoomsController(DbApiContext context) {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoom() {
            return await _context.Rooms.ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(string id) {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null) {
                return NotFound();
            }

            return room;
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(string id, Room room) {
            if (id != room.ID) {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RoomExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room) {
            _context.Rooms.Add(room);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (RoomExists(room.ID)) {
                    return Conflict();
                }
                else {
                    throw;
                }
            }

            return CreatedAtAction("GetRoom", new { id = room.ID }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id) {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(string id) {
            return _context.Rooms.Any(e => e.ID == id);
        }
    }
}
