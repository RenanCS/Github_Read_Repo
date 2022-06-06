using Bogus;
using GitRepositoryRead.Core.Entities;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitRepositoryRead.UnitTests.Helper
{
    public class HttpFactory
    {

        public static HttpFactory Inicializar()
        {
            return new HttpFactory();
        }

        public HttpClient HttpClientFactoryProjectsMock(HttpStatusCode statusCode, List<Project> projects)
        {
            return HttpClientFactoryMock(statusCode, JsonConvert.SerializeObject(projects).ToString());
        }

        public HttpClient HttpClientFactoryUserMock(HttpStatusCode statusCode, User user)
        {
            return HttpClientFactoryMock(statusCode, JsonConvert.SerializeObject(user).ToString());
        }



        private HttpClient HttpClientFactoryMock(HttpStatusCode statusCode, string objectMock)
        {

            var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(objectMock)
                })
                .Verifiable();

            var httpClientMock = new HttpClient(httpMessageHandler.Object);
            httpClientMock.BaseAddress = new Uri(new Faker().Internet.Url());

            return httpClientMock;

        }
    }
}
