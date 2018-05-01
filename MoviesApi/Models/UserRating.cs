using System;
namespace MoviesApi.Models
{
    /// <summary>
    /// A User rating of a movie
    /// </summary>
    public class UserRating
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int Rating { get; set; }
    }
}
