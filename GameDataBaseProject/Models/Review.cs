using System;

namespace GameDataBaseProject.Models
{
    public class Review
    {
         public int ReviewID { get; set; }
        public int GameID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}