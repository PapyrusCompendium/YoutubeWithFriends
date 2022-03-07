
using Microsoft.EntityFrameworkCore;

using YoutubeWithFriends.Db.Models;

namespace YoutubeWithFriends.Api.Data {
    public class DbApiContext : DbContext {
        public DbApiContext(DbContextOptions<DbApiContext> options)
            : base(options) {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}