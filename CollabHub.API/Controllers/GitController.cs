using CollabHub.Application.DTO.Git.Commit;
using CollabHub.Application.DTO.Git.GitPush;
using CollabHub.Application.DTO.Git.Pull;
using CollabHub.Application.Interfaces.Git;
using CollabHub.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api / webhooks / git")]
    //[Authorize(Roles = "Member")]

    public class WebHookController : ControllerBase
    {
        private readonly IGitService _gitService;
        public WebHookController(IGitService gitservice)
        {
            _gitService = gitservice;

        }

        [HttpPost("push")]
        public async Task <IActionResult>Push([FromBody]GitPushPayloadDTO payload)
        {
            var result=await _gitService.ProcessPushEventAsync(payload);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("commit")]
        public async Task<IActionResult> Commit([FromBody]CommitWebhookPayloadDTO payload)
        {
            var result=await _gitService.ProcessCommitWebHookAsync(payload);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("pull")]
        public async Task<IActionResult> PullRequest([FromBody ]GitPullRequestPayloadDTO payload)
        {
            var result=await _gitService.ProcessPullRequestEventAsync(payload);
            return StatusCode(result.StatusCode, result);
        }
    }
}
