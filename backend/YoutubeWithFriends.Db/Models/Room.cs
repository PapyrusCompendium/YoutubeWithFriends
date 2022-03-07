using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YoutubeWithFriends.Db.Models {
    public class Room {
        [Key]
        public string ID { get; set; }

        public string RoomName { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }
    }

    public class RoomConfiguration : IEntityTypeConfiguration<Room> {
        public void Configure(EntityTypeBuilder<Room> builder) {
            // Do any config here.
        }
    }
}