using System.ComponentModel.DataAnnotations;
using VideoGameCatalogue.API.Contracts;

namespace VideoGameCatalogue.API.Tests.Contracts
{
    public class VideoGameRequestValidationTests
    {
        public static TheoryData<string, VideoGameRequest> InvalidRequests => new()
        {
            { nameof(VideoGameRequest.Title), TestData.ValidRequest() with { Title = "" } },
            { nameof(VideoGameRequest.Title), TestData.ValidRequest() with { Title = "   " } },
            { nameof(VideoGameRequest.Title), TestData.ValidRequest() with { Title = new string('a', 201) } },
            { nameof(VideoGameRequest.Developer), TestData.ValidRequest() with { Developer = "" } },
            { nameof(VideoGameRequest.Developer), TestData.ValidRequest() with { Developer = new string('a', 151) } },
            { nameof(VideoGameRequest.Publisher), TestData.ValidRequest() with { Publisher = "" } },
            { nameof(VideoGameRequest.Publisher), TestData.ValidRequest() with { Publisher = new string('a', 151) } },
            { nameof(VideoGameRequest.Genre), TestData.ValidRequest() with { Genre = "" } },
            { nameof(VideoGameRequest.Genre), TestData.ValidRequest() with { Genre = "Not A Genre" } },
            { nameof(VideoGameRequest.Platform), TestData.ValidRequest() with { Platform = "" } },
            { nameof(VideoGameRequest.Platform), TestData.ValidRequest() with { Platform = "Dreamcast" } },
            { nameof(VideoGameRequest.Price), TestData.ValidRequest() with { Price = -0.01m } },
            { nameof(VideoGameRequest.MetacriticScore), TestData.ValidRequest() with { MetacriticScore = -1 } },
            { nameof(VideoGameRequest.MetacriticScore), TestData.ValidRequest() with { MetacriticScore = 101 } },
            { nameof(VideoGameRequest.Description), TestData.ValidRequest() with { Description = new string('a', 1001) } },
        };

        public static TheoryData<VideoGameRequest> ValidBoundaryRequests => new()
        {
            TestData.ValidRequest(),
            TestData.ValidRequest() with { Title = new string('a', 200) },
            TestData.ValidRequest() with { Price = 0m },
            TestData.ValidRequest() with { MetacriticScore = 0 },
            TestData.ValidRequest() with { MetacriticScore = 100 },
            TestData.ValidRequest() with { Description = null },
            TestData.ValidRequest() with { Description = new string('a', 1000) },
            TestData.ValidRequest() with { ReleaseDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)) },
        };

        [Theory]
        [MemberData(nameof(InvalidRequests))]
        public void Validate_InvalidField_ReportsErrorForThatField(string expectedMember, VideoGameRequest request)
        {
            var results = Validate(request);

            Assert.Contains(results, r => r.MemberNames.Contains(expectedMember));
        }

        [Fact]
        public void Validate_SeveralInvalidFields_ReportsEveryOneAtOnce()
        {
            var request = TestData.ValidRequest() with { Title = new string('a', 201), Genre = "Not A Genre", Platform = "Dreamcast" };

            var invalidMembers = Validate(request).SelectMany(r => r.MemberNames);

            Assert.Equal(
                [nameof(VideoGameRequest.Genre), nameof(VideoGameRequest.Platform), nameof(VideoGameRequest.Title)],
                invalidMembers.Order());
        }

        [Fact]
        public void Validate_EmptyGenre_ReportsOnlyTheRequiredError()
        {
            var results = Validate(TestData.ValidRequest() with { Genre = "" });

            var error = Assert.Single(results);
            Assert.Equal([nameof(VideoGameRequest.Genre)], error.MemberNames);
        }

        [Theory]
        [MemberData(nameof(ValidBoundaryRequests))]
        public void Validate_ValidRequest_HasNoErrors(VideoGameRequest request)
        {
            var results = Validate(request);

            Assert.Empty(results);
        }

        private static List<ValidationResult> Validate(VideoGameRequest request)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
            return results;
        }
    }
}
