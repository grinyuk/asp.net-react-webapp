using Core.Interfaces.Service;
using Core.Helpers;
using Core.Models;
using Core.Models.Assignment;
using Core.ResourcesFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json.Serialization;
using System.Text.Json;
using Web2PnK.Models;
using Web2PnK.Models.Assignment;
using Core.Enums;
using Core.Entities;

namespace Web2PnK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentController : Controller
    {
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly IAssignmentService _assignmentService;
        private readonly IPnkUserService _pnkUserService;
        private readonly ICalculateService _calculateService;
        private readonly AppConfig _config;

        public AssignmentController(IAssignmentService assignmentService, IPnkUserService pnkUserService, ICalculateService calculateService, AppConfig config)
        {
            _assignmentService = assignmentService;
            _pnkUserService = pnkUserService;
            _calculateService = calculateService;
            _config = config;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAllAssignments()
        {
            if (Guid.TryParse(User.GetUserId(), out Guid userId))
            {
                var allAssignments = await _assignmentService.GetAssignmentsAdmin();
                return Json(allAssignments, jsonOptions);

            }

            return BadRequest();
        }

        [HttpGet("archive")]
        public async Task<IActionResult> GetArchiveAssignments()
        {
            if (Guid.TryParse(User.GetUserId(), out Guid userId))
            {
                var activeAssignments = await _assignmentService.GetAssignments(userId, false);
                return Json(activeAssignments, jsonOptions);
            }
            return BadRequest();
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAssignments()
        {
            if (Guid.TryParse(User.GetUserId(), out Guid userId))
            {
                var activeAssignments = await _assignmentService.GetAssignments(userId, true);
                return Json(activeAssignments, jsonOptions);

            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentModel assignment)
        {
            var newAssignment = new Assignment
            {
                Title = assignment.Title,
                Subject = assignment.Subject,
                Description = assignment.Description,
                Difficulty = assignment.Difficulty,
                AuthorName = assignment.AuthorName,
                AuthorDescription = assignment.AuthorDescription,
                StartDate = assignment.StartDate,
                EndDate = assignment.EndDate,
                File = new AssignmentFile
                {
                    File = assignment.File,
                    FileName = assignment.FileName
                },
                Answers = assignment.Answers?.Select(x => new AssignmentAnswer()
                {
                    Description = x.Description,
                    Answer = x.Answer,
                    MaxScore = x.MaxScore
                }),
                AssignmentThemesIds = assignment.AssignmentThemesIds
            };

            var createdAssignment = _assignmentService.CreateAssignment(newAssignment);

            return Ok(createdAssignment);
        }

        [HttpPatch]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentUpdateModel assignment)
        {
            var updateAssignment = new Assignment
            {
                Id = assignment.Id!.Value,
                Title = assignment.Title,
                Subject = assignment.Subject,
                Description = assignment.Description,
                Difficulty = assignment.Difficulty,
                AuthorName = assignment.AuthorName,
                AuthorDescription = assignment.AuthorDescription,
                StartDate = assignment.StartDate,
                EndDate = assignment.EndDate,
                File = new AssignmentFile
                {
                    File = assignment.File,
                    FileName = assignment.FileName
                },
                Answers = assignment.Answers?.Select(x => new AssignmentAnswer()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Answer = x.Answer,
                    MaxScore = x.MaxScore
                }),
                AssignmentThemesIds = assignment.AssignmentThemesIds
            };

            var createdAssignment = await _assignmentService.UpdateAssignmentAsync(updateAssignment);

            return Ok(createdAssignment);
        }

        [HttpPost("Answer")]
        public async Task<IActionResult> SetAnswer(UserAnswerModel userAnswerModel)
        {
            var userId = Guid.Parse(User.GetUserId()!);
            var isActionAllow = _pnkUserService.IsAllowAction(userId, ActionType.TaskAnswerAmount);
            if (isActionAllow.Result)
            {
                _assignmentService.SaveUserResult(userId, userAnswerModel);

                return Json(new Response { Status = nameof(ResponseType.Success), Message = DefaultLanguage.SetAnswer });
            }
            return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response { Status = nameof(ResponseType.Error), Message = isActionAllow.Message });
        }

        [HttpGet("themes")]
        public async Task<IActionResult> GetThemes()
        {
            var themes = await _assignmentService.GetThemes();
            return Json(themes, jsonOptions);
        }

        [HttpGet("{assignmentId}/file")]
        public async Task<IActionResult> GetFile(int assignmentId)
        {
            var file = await _assignmentService.GetFile(assignmentId);
            if (file == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.FileNotFound });

            return Json(file, jsonOptions);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.GetError();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Guid.TryParse(User.GetUserId(), out Guid userId))
            {
                context.Result = Forbid();
            }
        }

        [HttpPatch("recalculate")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SetRecalculation([FromBody] AssignmentIdModel assignmentId)
        {
            if (assignmentId.AssignmentId != null)
            {
                await _calculateService.SetRecalculation(assignmentId?.AssignmentId);
                return Ok(new Response { Status = nameof(ResponseType.Success), Message = string.Format(DefaultLanguage.RecalculateSuccess, _config.RecalculateUserScoreLifeSpan.TotalMinutes) });
            }

            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
        }

        [HttpDelete("delete")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteAssignment([FromBody] AssignmentIdModel assignmentId)
        {
            if (assignmentId.AssignmentId != null)
            {
                var result = await _assignmentService.DeleteAssignment(assignmentId?.AssignmentId);

                if (result)
                {
                    return Ok(new Response { Status = nameof(ResponseType.Success), Message = DefaultLanguage.DeleteAssignmentSuccess });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
        }
    }
}