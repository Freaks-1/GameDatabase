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
    }
}