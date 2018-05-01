using System.Collections.Generic;
using System.Linq;
using MoviesApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;
using Xunit;

namespace MoviesApiTests
{
    public class MoviesControllerTests
    {

        [Fact]
        public void GetMovie_with_part_filter_values_dont_repeat()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            MovieFilter filter = new MovieFilter
            {
                Title = "Shawshank",
                Year = 1994
            };

            var result = controller.GetMovie(filter);
            var objectResult = result as ObjectResult;
            var value = objectResult.Value as List<MovieItem>;

            Assert.Single(value);

        }

        [Fact]
        public void GetMovie_with_genre_filter_return_multiple()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            MovieFilter filter = new MovieFilter
            {
                Genre = "Drama"
            };

            var result = controller.GetMovie(filter);
            var objectResult = result as ObjectResult;
            var value = objectResult.Value as List<MovieItem>;

            Assert.Equal(3,value.Count);

        }

        [Fact]
        public void GetMovie_with_no_filter_return_badrequest()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            MovieFilter filter = new MovieFilter();

            var result = controller.GetMovie(filter);

            Assert.IsType<BadRequestResult>(result);

        }

        [Fact]
        public void GetMovie_with_filter_noresults_return_notfound()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            MovieFilter filter = new MovieFilter
            {
                Title = "bad title"
            };

            var result = controller.GetMovie(filter);

            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public void GetTop5RatedMovies_correct_order()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            var result = controller.GetTop5RatedMovies() as ObjectResult;
            var value = result.Value as List<MovieReturnItem>;

            List<MovieReturnItem> list = value.OrderByDescending(m => m.Rating)
                                         .ThenBy(m => m.Title)
                            .ToList();

            Assert.Equal(5, value.Count);
            Assert.Equal(list, value);

        }

        [Fact]
        public void GetTop5RatedMoviesForUser_correct_order()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            var result = controller.GetTop5RatedMoviesForUser("Stuart") as ObjectResult;
            var value = result.Value as List<MovieReturnItem>;

            List<MovieReturnItem> list = value.OrderByDescending(m => m.Rating)
                                         .ThenBy(m => m.Title)
                                         .ToList();

            Assert.Equal(5, value.Count);
            Assert.Equal(list, value);

        }

        [Theory]
        [InlineData("bad user")] // user does not exist
        [InlineData("")] // user is blank
        public void GetTop5RatedMoviesForUser_badRequest(string userId)
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            var result = controller.GetTop5RatedMoviesForUser(userId);

            Assert.IsType<BadRequestResult>(result);

        }

        [Theory]
        [InlineData(1, "bad user")] // user does not exist
        [InlineData(72, "Mary")] // movie does not exist
        public void UpdateAddUserRating_return_notfound(int movieId, string userId)
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            var userRating = new UserRating { MovieId = movieId, UserId = userId, Rating = 1 };

            var result = controller.UpdateAddUserRating(userRating);

            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(6)] 
        [InlineData(0)] 
        [InlineData(-1)] 
        public void UpdateAddUserRating_rating_not_valid_return_badrequest(int ratingValue)
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            var userRating = new UserRating { MovieId = 1, UserId = "Mary", Rating = ratingValue };

            var result = controller.UpdateAddUserRating(userRating);

            Assert.IsType<BadRequestResult>(result);

        }

        [Fact]
        public void UpdateAddUserRating_update_user_rating()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            UserRating userRating = context.UserRatings
                                           .FirstOrDefault(u => u.UserId == "Mary" && u.MovieId == 1);

            userRating.Rating++;
            int expect = userRating.Rating;

            var result = controller.UpdateAddUserRating(userRating);

            Assert.IsType<NoContentResult>(result);

            userRating = context.UserRatings
                                .FirstOrDefault(u => u.UserId == "Mary" && u.MovieId == 1);

            Assert.Equal(expect, userRating.Rating);


        }

        [Fact]
        public void UpdateAddUserRating_add_user_rating()
        {
            MoviesApiContext context = MoviesApiContext.CreateContextWithDataForTesting();

            MoviesController controller = new MoviesController(context);

            UserRating userRating = new UserRating { UserId = "Wendy", MovieId = 1, Rating = 5 };

            var result = controller.UpdateAddUserRating(userRating);

            Assert.IsType<NoContentResult>(result);

            UserRating newRating = context.UserRatings
                                .FirstOrDefault(u => u.UserId == "Wendy" && u.MovieId == 1);

            Assert.NotNull(newRating);


        }

    }
}
