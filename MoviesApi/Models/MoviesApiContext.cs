using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace MoviesApi.Models
{
    public class MoviesApiContext : DbContext
    {
        public MoviesApiContext(DbContextOptions<MoviesApiContext> options) 
            : base(options)
        {
            // if there is no data - add some!
            if (this.Movies.Count() == 0 || this.UserRatings.Count() == 0)
            {
                AddTestData(this);
            }
        }

        public DbSet<MovieItem> Movies { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }

        /// <summary>
        /// Add seed data to the DB
        /// </summary>
        /// <param name="context">the DB to be updated</param>
        private static void AddTestData(MoviesApiContext context)
        {
            context.Movies.Add(new MovieItem
            {
                Id = 1,
                Title = "The Shawshank Redemption",
                YearOfRelease = 1994,
                RunningTime = 142,
                Genres = "Crime,Drama"
            });

            context.Movies.Add(new MovieItem
            {
                Id = 2,
                Title = "American Beauty",
                YearOfRelease = 1999,
                RunningTime = 122,
                Genres = "Drama"
            });

            context.Movies.Add(new MovieItem
            {
                Id = 3,
                Title = "Moana",
                YearOfRelease = 2016,
                RunningTime = 107,
                Genres = "Animation, Adventure, Comedy"
            });

            context.Movies.Add(new MovieItem
            {
                Id = 4,
                Title = "X-Men",
                YearOfRelease = 2000,
                RunningTime = 104,
                Genres = "Action,Adventure,Sci-fi"
            });

            context.Movies.Add(new MovieItem
            {
                Id = 5, 
                Title = "Pan's Labyrinth",
                YearOfRelease = 2006,
                RunningTime = 118,
                Genres = "Drama, Fantasy"
            });

            context.Movies.Add(new MovieItem
            {
                Id = 6,
                Title = "Zoolander",
                YearOfRelease = 2001,
                RunningTime = 90,
                Genres = "Comedy"
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 1,
                Rating = 5
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 2,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 3,
                Rating = 2
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 4,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 5,
                Rating = 5
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Stuart",
                MovieId = 6,
                Rating = 2
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Mary",
                MovieId = 1,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Mary",
                MovieId = 2,
                Rating = 2
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Mary",
                MovieId = 3,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Mary",
                MovieId = 4,
                Rating = 4
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Mary",
                MovieId = 5,
                Rating = 5
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Wendy",
                MovieId = 2,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Wendy",
                MovieId = 3,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Wendy",
                MovieId = 4,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Wendy",
                MovieId = 5,
                Rating = 3
            });

            context.UserRatings.Add(new UserRating
            {
                UserId = "Wendy",
                MovieId = 6,
                Rating = 3
            });

            context.SaveChanges();
        }

        /// <summary>
        /// Creates DB context for testing.
        /// </summary>
        /// <returns>The DB context with data for testing</returns>
        public static MoviesApiContext CreateContextWithDataForTesting()
        {
            var options = new DbContextOptionsBuilder<MoviesApiContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;

            MoviesApiContext context = new MoviesApiContext(options);

            return context;
        }


    }
}
