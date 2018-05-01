using System;
namespace MoviesApi.Models
{
    public class MovieReturnItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public double Rating { get; set; }

    }
}
