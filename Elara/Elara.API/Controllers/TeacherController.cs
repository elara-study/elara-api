using Asp.Versioning;
using Elara.Application.Features.Users.Teachers.Commands.CreateClass;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement;
using Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements;
using Elara.Application.Features.Users.Teachers.Queries.GetClassInfo;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherProfile;
using Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents;
using Elara.Application.Features.Users.Teachers.Commands.AddStudentByUsername;
using Elara.API.Controllers.Requests;
using Elara.Application.Features.Users.Teachers.Queries.GetTopicResources;
using Elara.Application.Features.Users.Teachers.Commands.AddTopicResource;
using Elara.Application.Features.Users.Teachers.Commands.DeleteTopicResource;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions;
using Elara.Application.Features.Users.Teachers.Queries.GetStudentSubmission;
using Elara.Application.Features.Users.Teachers.Commands.RateStudentSubmission;
using Elara.Domain.Constants;
using Elara.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elara.Application.Responses;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetStudentInsightsForTeacher;
using System;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/teacher")]
    [Authorize(Roles = Roles.Teacher)]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;
        private Guid TeacherId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var teacherId)
            ? teacherId
            : throw new UnauthorizedAccessException("User is not authenticated.");

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(BaseResponse<TeacherProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetTeacherProfileQuery(TeacherId), cancellationToken);
            return Ok(new BaseResponse<TeacherProfileDto>
            {
                Message = "Teacher profile retrieved successfully.",
                Data = response
            });
        }

        [HttpGet("classes")]
        [ProducesResponseType(typeof(BaseResponse<List<GetTeacherClassesResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTeacherClasses()
        {
            var query = new GetTeacherClassesQuery();
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<List<GetTeacherClassesResponse>>
            {
                Message = "Teacher classes retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("classes")]
        [ProducesResponseType(typeof(BaseResponse<CreateClassResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
        {
            var command = new CreateClassCommand(request.Name, request.Grade, request.RoadmapName);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetClassInfo), new { id = result.Id }, new BaseResponse<CreateClassResponse>
            {
                Message = "Class created successfully.",
                Data = result
            });
        }

        [HttpPost("roadmaps")]
        [ProducesResponseType(typeof(BaseResponse<CreateRoadmapResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateRoadmap([FromBody] CreateRoadmapRequest request)
        {
            var command = new CreateRoadmapCommand(request.Name, request.Grade, request.Subject);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateRoadmap), new BaseResponse<CreateRoadmapResponse>
            {
                Message = "Roadmap created successfully.",
                Data = result
            });
        }

        [HttpGet("classes/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<GetClassInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetClassInfo(Guid id)
        {
            var query = new GetClassInfoQuery { ClassId = id };
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<GetClassInfoResponse>
            {
                Message = "Class information retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("groups/{id:guid}/announcements")]
        [ProducesResponseType(typeof(BaseResponse<AddAnnouncementResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddAnnouncement(Guid id, [FromBody] AddAnnouncementRequest request)
        {
            var command = new AddAnnouncementCommand(id, request.Title, request.Content);
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<AddAnnouncementResponse>
            {
                Message = "Announcement added successfully.",
                Data = result
            });
        }

        [HttpGet("groups/{id:guid}/announcements")]
        [ProducesResponseType(typeof(BaseResponse<List<GetAnnouncementsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnnouncements(Guid id)
        {
            var query = new GetAnnouncementsQuery { ClassId = id };
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<List<GetAnnouncementsResponse>>
            {
                Message = "Announcements retrieved successfully.",
                Data = result
            });
        }

        [HttpDelete("groups/{id:guid}/announcements/{announcementId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAnnouncement(Guid id, Guid announcementId)
        {
            var command = new DeleteAnnouncementCommand(id, announcementId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("groups/{id:guid}/students")]
        [ProducesResponseType(typeof(BaseResponse<List<GetGroupStudentsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupStudents(Guid id)
        {
            var query = new GetGroupStudentsQuery { ClassId = id };
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<List<GetGroupStudentsResponse>>
            {
                Message = "Students retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("groups/{id:guid}/students")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStudent(Guid id, [FromBody] AddStudentRequest request)
        {
            var command = new AddStudentByUsernameCommand(id, request.Username);
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<bool>
            {
                Message = "Student added successfully.",
                Data = result
            });
        }

        [HttpGet("students/{studentId:guid}/insights")]
        [ProducesResponseType(typeof(BaseResponse<StudentInsightForTeacherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentInsights(Guid studentId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetStudentInsightsForTeacherQuery { StudentId = studentId }, cancellationToken);
            return Ok(new BaseResponse<StudentInsightForTeacherDto>
            {
                Message = "Student insights retrieved successfully.",
                Data = result
            });
        }

        #region Resource Management

        [HttpGet("topics/{topicId:int}/resources")]
        [ProducesResponseType(typeof(BaseResponse<TopicResourcesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTopicResources(int topicId, CancellationToken cancellationToken)
        {
            var query = new GetTopicResourcesQuery { TopicId = topicId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<TopicResourcesDto>
            {
                Message = "Topic resources retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("topics/{topicId:int}/resources")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<ResourceItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddTopicResource(int topicId, [FromForm] string title, [FromForm] ResourceType type, IFormFile file, CancellationToken cancellationToken)
        {
            var command = new AddTopicResourceCommand(topicId, title, type, file);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<ResourceItemDto>
            {
                Message = "Resource added successfully.",
                Data = result
            });
        }

        [HttpDelete("resources/{resourceId:int}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteTopicResource(int resourceId, CancellationToken cancellationToken)
        {
            var command = new DeleteTopicResourceCommand { ResourceId = resourceId };
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse<bool>
            {
                Message = "Resource deleted successfully.",
                Data = true
            });
        }

        #endregion

        #region Homework Management

        [HttpGet("topics/{topicId:int}/homework")]
        [ProducesResponseType(typeof(BaseResponse<HomeworkOverviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHomeworkOverview(int topicId, CancellationToken cancellationToken)
        {
            var query = new GetHomeworkOverviewQuery { TopicId = topicId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<HomeworkOverviewDto>
            {
                Message = "Homework overview retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("assignments/{assignmentId:int}/problems")]
        [ProducesResponseType(typeof(BaseResponse<HomeworkProblemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddHomeworkProblem(int assignmentId, [FromBody] AddHomeworkProblemRequest request, CancellationToken cancellationToken)
        {
            var command = new AddHomeworkProblemCommand(assignmentId, request.Description);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<HomeworkProblemDto>
            {
                Message = "Homework problem added successfully.",
                Data = result
            });
        }

        #endregion

        #region Submissions & Rating

        [HttpGet("assignments/{assignmentId:int}/submissions")]
        [ProducesResponseType(typeof(BaseResponse<List<HomeworkSubmissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHomeworkSubmissions(int assignmentId, [FromQuery] string status = "unrated", CancellationToken cancellationToken = default)
        {
            var query = new GetHomeworkSubmissionsQuery { AssignmentId = assignmentId, Status = status };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<List<HomeworkSubmissionDto>>
            {
                Message = "Submissions retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("submissions/{submissionId:int}")]
        [ProducesResponseType(typeof(BaseResponse<StudentSubmissionDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentSubmission(int submissionId, CancellationToken cancellationToken)
        {
            var query = new GetStudentSubmissionQuery { SubmissionId = submissionId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<StudentSubmissionDetailDto>
            {
                Message = "Submission details retrieved successfully.",
                Data = result
            });
        }

        [HttpPut("submissions/{submissionId:int}/rate")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RateSubmission(int submissionId, [FromBody] RateSubmissionRequest request, CancellationToken cancellationToken)
        {
            var command = new RateStudentSubmissionCommand(submissionId, request.AwardedXp);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse<bool>
            {
                Message = "Submission rated successfully.",
                Data = true
            });
        }

        #endregion
    }
}