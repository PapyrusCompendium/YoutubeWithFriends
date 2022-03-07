using System.ComponentModel.DataAnnotations;

namespace YoutubeWithFriends.Db.Models {
    public class Room {
        [Key]
        public string ID { get; set; }
    }
}