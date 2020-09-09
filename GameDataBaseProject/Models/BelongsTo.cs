using System;

namespace GameDataBaseProject.Models
{
    public class BelongsTo
    {
        public int BelongsToID { get; set; }
        public int GameID { get; set; }
        public int GenreID { get; set; }

 //       public Game Game{ get; set; }
 //       public Genre Genre{ get; set; }

    }
}