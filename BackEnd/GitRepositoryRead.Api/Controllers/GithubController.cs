using GitRepositoryRead.Core.Service;
using GitRepositoryRead.InputModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GitRepositoryRead.Api.Controllers
{
    [Route("api/github")]
    public class GithubController : ControllerBase
    {
        private readonly IGithubService _gitHubService;

        public GithubController(IGithubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpPost("GetRepositories")]
        public async Task<IActionResult> GetRepositories([FromBody] RepositoriesInputModel inputModel)
        {
            if (inputModel.Take > 30)
            {
                return BadRequest("Ultrapssou o limit do github");
            }

            var result = await _gitHubService.GetRepositories(inputModel.UserName, inputModel.Skip, inputModel.Take);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }



        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Nome do usuáiro em branco");
            }

            var result = await _gitHubService.GetUser(userName);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
