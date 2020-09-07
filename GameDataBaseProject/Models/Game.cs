using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GameDataBaseProject.Models
{
    
    public class Game 
    {
        public int GameID { get; set; }
        public string name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public ICollection<BelongsTo> BelongToGenre { get; set; }
        public ICollection<Multimedia> Multimedias { get; set; }
    }
}