using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;
using System.Linq;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly MoviesApiContext _moviesApiContext;

        public MoviesController(MoviesApiContext moviesApiContext)
        {
            _moviesApiContext = moviesApiContext;
        }


        /// <summary>
        /// Gets a movie/movies based on filter criteria
        /// </summary>
        /// <returns>A collection of MovieItems that match the filter criteria</returns>
        /// <param name="filter">A MovieFilter. Must have at least one filter set. Title, Genre, or YearOfRelease</param>
        [HttpGet]
        public IActionResult GetMovie(MovieFilter filter)
        {
            //check we have a filter
            if (filter.Year == 0 && filter.Title == null && filter.Genre == null)
            {
                return BadRequest();
            }

            //get all movies that match the filter
            List<MovieItem> items = new List<MovieItem>();
            items.AddRange(_moviesApiContext.Movies.Where(m => m.YearOfRelease.Equals(filter.Year)
                                                          || (filter.Title != null && m.Title.Contains(filter.Title))
                                                          || (filter.Genre != null && m.Genres.Contains(filter.Genre))).ToList());

            // check whether there are any matching results
            if (items.Count == 0)
            {
                return NotFound();
            }

            return new ObjectResult(items);

        }

        /// <summary>
        /// Gets the top5 rated movies in the Database
        /// </summary>
        /// <returns>A collection of MovieItems of the top5 rated movies.</returns>
        [HttpGet("top5")]
        public IActionResult GetTop5RatedMovies()
        {

            List<MovieReturnItem> items = new List<MovieReturnItem>();
            items.AddRange(_moviesApiContext.UserRatings
                             .GroupBy(g => g.MovieId, r => r.Rating)
                             .Select(g => new
                             {
                                // get average rating for each movie
                                 MovieId = g.Key,
                                 Rating = g.Average()
                             })
                             .Join(_moviesApiContext.Movies,
                                   u => u.MovieId,
                                   m => m.Id,
                                 (u, m) => new MovieReturnItem
                                 {
                                     Id = m.Id,
                                     Rating = Math.Round(u.Rating,1), // round to 1 decimal place
                                     Title = m.Title,
                                     RunningTime = m.RunningTime,
                                     YearOfRelease = m.YearOfRelease
                                 })
                           //order then take top 5
                           .OrderByDescending(m => m.Rating)
                           .ThenBy(m => m.Title)
                           .Take(5)
                           .ToList());

            // check we have results (will only happen if DB is empty)
            if (items.Count == 0)
            {
                return NotFound();
            }

            return new ObjectResult(items);
        }

        /// <summary>
        /// Gets the top5 rated movies for a user.
        /// </summary>
        /// <returns>A collection of MovieItems of the top5 rated movies for a user</returns>
        /// <param name="userId">User id of user</param>
        [HttpGet("top5/{userId}")]
        public IActionResult GetTop5RatedMoviesForUser(string userId)
        {
            //check we have a userId - shouldnt happen if useing route
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            //check user exists
            IQueryable<UserRating> users = _moviesApiContext.UserRatings.Where(m => m.UserId == userId);

            if (users == null || users.Count() == 0)
            {
                return BadRequest();
            }

            //get top 5 movies rated by specified user
            List<MovieReturnItem> items = new List<MovieReturnItem>();
            items.AddRange(users
                           // get users top 5
                           .OrderByDescending(u => u.Rating)
                           .Take(5) 
                           // get movie data
                           .Join(_moviesApiContext.Movies, 
                                 u => u.MovieId, 
                                 m => m.Id, 
                                 (u, m) => new MovieReturnItem{ 
                                                            Id = m.Id, 
                                                            Rating = u.Rating, 
                                                            Title = m.Title, 
                                                            RunningTime = m.RunningTime, 
                                                            YearOfRelease = m.YearOfRelease })
                           .OrderByDescending(m => m.Rating)
                           .ThenBy(m => m.Title)
                           .ToList());

            // will always return an item if the user was found

            return new ObjectResult(items);
        }

        /// <summary>
        /// Updates or adds a user rating.
        /// </summary>
        /// <returns>NoContentResult</returns>
        /// <param name="rating">A UserRating to be added to the DB</param>
        [HttpPut("userRating")]
        public IActionResult UpdateAddUserRating([FromBody]UserRating rating)
        {
            //check user exists
            UserRating user = _moviesApiContext.UserRatings
                                               .FirstOrDefault(u => rating.UserId!=null && u.UserId == rating.UserId);
            if(user == null)
            {
                return NotFound();
            }

            //check movie exists
            MovieItem movie = _moviesApiContext.Movies.FirstOrDefault(m => m.Id == rating.MovieId);
            if (movie == null)
            {
                return NotFound();
            }

            //check rating is within correct range
            if(rating.Rating < 1 || rating.Rating > 5)
            {
                return BadRequest();
            }

            //add or update user rating
            UserRating userRating = _moviesApiContext.UserRatings
                                                     .FirstOrDefault(u => u.UserId == rating.UserId && u.MovieId == rating.MovieId);

            if(userRating == null)
            {
                _moviesApiContext.UserRatings.Add(rating);
                _moviesApiContext.SaveChanges();
            }
            else
            {
                userRating.Rating = rating.Rating;
                _moviesApiContext.SaveChanges();
            }

            return new NoContentResult();
        }


    }
}
