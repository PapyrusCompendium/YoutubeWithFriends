using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YoutubeWithFriends.Db.Models {
    public class Room {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string RoomName { get; set; }

        [ForeignKey(nameof(RoomOwner))]
        public Guid RoomOwnerId { get; set; }

        public User RoomOwner { get; set; }

        [Required]
        public string OwnerSessionId { get; set; }

        /// <summary>
        /// ',' delimited string.
        /// </summary>
        [Required]
        public string JoinedUserSessionIds { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        [Required]
        public DateTimeOffset LastActivity { get; set; }
    }

    public class RoomConfiguration : IEntityTypeConfiguration<Room> {
        public void Configure(EntityTypeBuilder<Room> builder) {
        }
    }
}