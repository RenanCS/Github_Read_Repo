using GitRepositoryRead.Core.Entities;
using GitRepositoryRead.Core.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitRepositoryRead.Application.Service
{
    public class GithubService : IGithubService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<Project>> GetRepositories(string userName, int skip, int take)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("Obrigatório informar o nome do usuário");
            }

            var client = _httpClientFactory.CreateClient("GithubApi");
            var response = await client.GetAsync(client.BaseAddress + $"/{userName}/repos?sort=stars&order=desc&page={skip + 1}&per_page={take}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string content = await response.Content.ReadAsStringAsync();
            List<Project> projects = JsonConvert.DeserializeObject<List<Project>>(content);

            return projects
                .OrderByDescending(project => project.stargazers_count)
                .ToList();
        }

        public async Task<User> GetUser(string userName)
        {

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("Obrigatório informar o nome do usuário");
            }

            var client = _httpClientFactory.CreateClient("GithubApi");
            var response = await client.GetAsync(client.BaseAddress + $"/{userName}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            User user = JsonConvert.DeserializeObject<User>(content);

            return user;
        }
    }
}
