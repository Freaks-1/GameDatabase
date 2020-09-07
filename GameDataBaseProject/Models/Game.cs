using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GameDataBaseProject.Models
{
    class Game 
    {
        public int GamesID { get; set; }
        public string name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Multimedia> Multimedias { get; set; }
    }
}