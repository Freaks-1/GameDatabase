using Microsoft.EntityFrameworkCore;
using GameDataBaseProject.Models;

namespace GameDataBaseProject.Data
{
    public class GameDataBaseContext: DbContext
    {
        public GameDataBaseContext (DbContextOptions<GameDataBaseContext> options)
            : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Multimedia> Multimedias { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BelongsTo> Belongings { get; set; }

    }
}