using System;

using Microsoft.EntityFrameworkCore;

namespace YoutubeWithFriends.Db {
    public class SimpleDbContextFactory : ISimpleDbContextFactory {
        private readonly Action<DbContextOptionsBuilder> _dbContextOptionsAction;

        public SimpleDbContextFactory(Action<DbContextOptionsBuilder> dbContextOptionsAction) {
            _dbContextOptionsAction = dbContextOptionsAction;
        }

        public TContext CreateContext<TContext>() where TContext : DbContext {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
            _dbContextOptionsAction.Invoke(dbContextOptionsBuilder);

            return (TContext)Activator.CreateInstance(typeof(TContext), new object[]
            {
               dbContextOptionsBuilder.Options
            });
        }
    }
}