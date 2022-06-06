
using GitRepositoryRead.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitRepositoryRead.Core.Service
{
    public interface IGithubService
    {
        Task<List<Project>> GetRepositories(string userName, int skip, int take);
        Task<User> GetUser(string userName);
    }
}
