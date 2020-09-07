using System;
using System.Collections.Generic;

namespace GameDataBaseProject.Models
{
    public class Genre
    {
         public int GenreID { get; set; }
        public string name { get; set; }
        public ICollection<BelongsTo> BelongsToGame { get; set; }
    }
}