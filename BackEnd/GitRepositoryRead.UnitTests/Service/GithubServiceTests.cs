using Bogus;
using GitRepositoryRead.Application.Service;
using GitRepositoryRead.Core.Entities;
using GitRepositoryRead.Core.Service;
using GitRepositoryRead.UnitTests.Helper;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GitRepositoryRead.UnitTests.Service
{
    public class GithubServiceTests
    {
        private IGithubService _githubService;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private Faker _faker;

        public GithubServiceTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _faker = new Faker();

        }

        #region MAKESUT
        private GithubService MakeSut(HttpClient stubHttpClient)
        {
            _httpClientFactory
              .Setup(_ => _.CreateClient(It.IsAny<string>()))
              .Returns(stubHttpClient).Verifiable();


            var stubService = new GithubService(_httpClientFactory.Object);

            return stubService;
        }

        private GithubService MakeSutProjects(HttpStatusCode statusCode, List<Project> stubProjects)
        {
            var stubHttpClient = HttpFactory
                                    .Inicializar()
                                    .HttpClientFactoryProjectsMock(statusCode, stubProjects);

            return MakeSut(stubHttpClient);
        }

        private GithubService MakeSutUser(HttpStatusCode statusCode, User user)
        {
            var stubHttpClient = HttpFactory
                                    .Inicializar()
                                    .HttpClientFactoryUserMock(statusCode, user);

            return MakeSut(stubHttpClient);
        }

        #endregion



        [Fact]
        public async Task GetRepositories_ShouldReturnListProjet_WhenUserNameExists()
        {
            var userName = _faker.Person.UserName;
            var stubProjects = new List<Project>();
            stubProjects.Add(new Project()
            {
                name = _faker.Commerce.ProductName(),
                html_url = _faker.Internet.Url(),
                description = _faker.Lorem.Locale,
            });

            _githubService = MakeSutProjects(HttpStatusCode.OK, stubProjects);

            var expect = await _githubService.GetRepositories(userName, 0, 25);

            Assert.Equal(stubProjects.Count, expect.Count);
        }

        [Fact]
        public async Task GetRepositories_ShouldReturnException_WhenUserNameEmpty()
        {
            var userName = string.Empty;

            _githubService = MakeSutProjects(HttpStatusCode.BadRequest, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetRepositories(userName, 0, 25));
        }

        [Fact]
        public async Task GetRepositories_ShouldReturnListEmpty_WhenUserNameNotHaveProject()
        {
            var userName = _faker.Person.UserName;
            var stubProjects = new List<Project>();

            _githubService = MakeSutProjects(HttpStatusCode.OK, stubProjects);

            var expect = await _githubService.GetRepositories(userName, 0, 25);

            Assert.Empty(expect);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserData_WhenUserNameExists()
        {
            var userName = _faker.Person.UserName;
            var userMock = new User
            {
                avatar_url = _faker.Person.Avatar,
                name = _faker.Person.UserName,
                email = _faker.Person.Email
            };

            _githubService = MakeSutUser(HttpStatusCode.OK, userMock);

            var expect = await _githubService.GetUser(userName);

            Assert.NotNull(expect);
        }

        [Fact]
        public async Task GetUser_ShouldReturnException_WhenUserNameEmpty()
        {
            var userName = string.Empty;

            _githubService = MakeSutUser(HttpStatusCode.OK, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => _githubService.GetUser(userName));
        }




    }
}
