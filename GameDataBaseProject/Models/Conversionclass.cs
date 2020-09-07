namespace GameDataBaseProject.Conversion
{
    class GameConversion
    {
        public int id{ get; set; }
        public string name{ get; set; }
        public long first_release_date{ get; set; }
        public int[] genres{ get; set; }
        public int[] videos{ get; set; }
        public double total_rating{ get; set; }
        public int[] screenshots{ get; set; }
        public int[] artworks{ get; set; }

    }
    class GenreConversion
    {
        public int id{ get; set; }
        public string name{ get; set; }
    }
    class ArtworkConverion
    {
        public int id{ get; set; }
        public string image_id{ get; set; }
        public string url{ get; set; }
    }

    class VideoConversion
    {
        public int id{ get; set; }
        public string url{ get; set; }
    }
}