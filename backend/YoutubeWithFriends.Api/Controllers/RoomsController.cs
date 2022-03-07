using Microsoft.AspNetCore.Mvc;

namespace YoutubeWithFriends.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase {
        [HttpGet("GetSessionInfo")]
        public IActionResult ConnectSession(string roomSession) {
            return Ok($"SessionID: {roomSession}");
        }
    }
}