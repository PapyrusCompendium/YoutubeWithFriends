
using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YoutubeWithFriends.Db.Models {
    public class User {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string SessionID { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            // Do any config here.
        }
    }
}