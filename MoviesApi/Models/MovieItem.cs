using System.Collections.Generic;
namespace MoviesApi.Models
{
    /// <summary>
    /// A Movie item to be stored in the DB
    /// </summary>
    public class MovieItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public string Genres { get; set; }
    }

}
