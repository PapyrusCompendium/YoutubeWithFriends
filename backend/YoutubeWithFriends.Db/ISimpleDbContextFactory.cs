using Microsoft.EntityFrameworkCore;

namespace YoutubeWithFriends.Db {
    public interface ISimpleDbContextFactory {
        TContext CreateContext<TContext>() where TContext : DbContext;
    }
}