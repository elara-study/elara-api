using Asp.Versioning;
using Elara.Application.Features.Users.Teachers.Commands.CreateClass;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps;
using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement;
using Elara.Application.Features.Users.Teachers.Commands.EditAnnouncement;
using Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements;
using Elara.Application.Features.Users.Teachers.Queries.GetClassInfo;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherProfile;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherHome;
using Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents;
using Elara.Application.Features.Users.Teachers.Queries.GetGroupRoadmap;
using Elara.Application.Features.Users.Teachers.Queries.GetStudentDetail;
using Elara.Application.Features.Users.Teachers.Commands.AddStudentByUsername;
using Elara.Application.Features.Users.Teachers.Commands.DeleteGroupStudent;
using Elara.Application.Features.Users.Teachers.Commands.DeleteClass;
using Elara.Application.Features.Users.Teachers.Commands.AddInsight;
using Elara.Application.Features.Users.Teachers.Commands.EditInsight;
using Elara.API.Controllers.Requests;
using Elara.Application.Features.Users.Teachers.Commands.AddModuleResource;
using Elara.Application.Features.Users.Teachers.Commands.DeleteModuleResource;
using Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Application.Features.Users.Teachers.Queries.GetModuleResources;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions;
using Elara.Application.Features.Users.Teachers.Queries.GetStudentSubmission;
using Elara.Application.Features.Users.Teachers.Commands.GradeStudentSubmission;
using Elara.Application.Features.Users.Teachers.Commands.EditProblem;
using Elara.Application.Features.Users.Teachers.Commands.DeleteProblem;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elara.Application.Responses;
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

        [HttpGet("home")]
        [ProducesResponseType(typeof(BaseResponse<TeacherHomeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHome(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetTeacherHomeQuery(), cancellationToken);
            return Ok(new BaseResponse<TeacherHomeDto>
            {
                Message = "Teacher home data retrieved successfully.",
                Data = response
            });
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

        [HttpGet("groups")]
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

        [HttpPost("groups")]
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
            var command = new CreateRoadmapCommand(request.Name, request.Grade);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateRoadmap), new BaseResponse<CreateRoadmapResponse>
            {
                Message = "Roadmap created successfully.",
                Data = result
            });
        }

        [HttpGet("roadmaps")]
        [ProducesResponseType(typeof(BaseResponse<List<TeacherRoadmapListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTeacherRoadmaps(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTeacherRoadmapsQuery(), cancellationToken);
            return Ok(new BaseResponse<List<TeacherRoadmapListDto>>
            {
                Message = "Roadmaps retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("roadmaps/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<TeacherRoadmapDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeacherRoadmapDetail(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTeacherRoadmapDetailQuery(id), cancellationToken);
            return Ok(new BaseResponse<TeacherRoadmapDetailDto>
            {
                Message = "Roadmap detail retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("groups/{id:guid}")]
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

        [HttpDelete("groups/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClass(Guid id)
        {
            var command = new DeleteClassCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("groups/{id:guid}/roadmap")]
        [ProducesResponseType(typeof(BaseResponse<TeacherRoadmapDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupRoadmap(Guid id)
        {
            var query = new GetGroupRoadmapQuery(id);
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<TeacherRoadmapDetailDto>
            {
                Message = "Group roadmap retrieved successfully.",
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

        [HttpPatch("groups/{id:guid}/announcements/{announcementId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AddAnnouncementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditAnnouncement(Guid id, Guid announcementId, [FromBody] EditAnnouncementRequest request)
        {
            var command = new EditAnnouncementCommand(id, announcementId, request.Title, request.Content);
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<AddAnnouncementResponse>
            {
                Message = "Announcement updated successfully.",
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

        [HttpDelete("groups/{id:guid}/students/{studentId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGroupStudent(Guid id, Guid studentId)
        {
            var command = new DeleteGroupStudentCommand(id, studentId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("students/{studentId:guid}/insights")]
        [ProducesResponseType(typeof(BaseResponse<AddInsightResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddInsight(Guid studentId, [FromBody] AddInsightRequest request)
        {
            var command = new AddInsightCommand(studentId, request.Content);
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<AddInsightResponse>
            {
                Message = "Insight added successfully.",
                Data = result
            });
        }

        [HttpPatch("insights/{insightId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AddInsightResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditInsight(Guid insightId, [FromBody] EditInsightRequest request)
        {
            var command = new EditInsightCommand(insightId, request.Content);
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<AddInsightResponse>
            {
                Message = "Insight updated successfully.",
                Data = result
            });
        }

        [HttpGet("students/{studentId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<StudentDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentDetail(Guid studentId)
        {
            var query = new GetStudentDetailQuery(studentId);
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<StudentDetailResponse>
            {
                Message = "Student details retrieved successfully.",
                Data = result
            });
        }

        #region Resource Management

        [HttpPost("modules/{moduleId:guid}/resources")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<ResourceItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddModuleResource(Guid moduleId, [FromForm] string title, IFormFile file, CancellationToken cancellationToken)
        {
            var command = new AddModuleResourceCommand(moduleId, title, file);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<ResourceItemDto>
            {
                Message = "Resource added successfully.",
                Data = result
            });
        }

        [HttpDelete("resources/{resourceId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteModuleResource(int resourceId, CancellationToken cancellationToken)
        {
            var command = new DeleteModuleResourceCommand { ResourceId = resourceId };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        #endregion

        #region Homework Management

        [HttpPost("modules/{moduleId:guid}/homework/problems")]
        [ProducesResponseType(typeof(BaseResponse<HomeworkProblemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddHomeworkProblem(Guid moduleId, [FromBody] AddHomeworkProblemRequest request, CancellationToken cancellationToken)
        {
            var command = new AddHomeworkProblemCommand(moduleId, request.Description);
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<HomeworkProblemDto>
            {
                Message = "Homework problem added successfully.",
                Data = result
            });
        }

        [HttpPatch("problems/{problemId:int}")]
        [ProducesResponseType(typeof(BaseResponse<HomeworkProblemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditProblem(int problemId, [FromBody] EditProblemRequest request, CancellationToken cancellationToken)
        {
            var command = new EditProblemCommand(problemId, request.Description);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse<HomeworkProblemDto>
            {
                Message = "Problem updated successfully.",
                Data = result
            });
        }

        [HttpDelete("problems/{problemId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProblem(int problemId, CancellationToken cancellationToken)
        {
            var command = new DeleteProblemCommand(problemId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        #endregion

        #region Submissions & Grading

        [HttpGet("modules/{moduleId:guid}/homework/submissions")]
        [ProducesResponseType(typeof(BaseResponse<List<HomeworkSubmissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHomeworkSubmissions(Guid moduleId, [FromQuery] string status = "unrated", CancellationToken cancellationToken = default)
        {
            var query = new GetHomeworkSubmissionsQuery { ModuleId = moduleId, Status = status };
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

        [HttpPut("submissions/{submissionId:int}/grade")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GradeSubmission(int submissionId, [FromBody] GradeStudentSubmissionRequest request, CancellationToken cancellationToken)
        {
            var command = new GradeStudentSubmissionCommand(submissionId, request.Score, request.Feedback);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse<bool>
            {
                Message = "Submission graded successfully.",
                Data = true
            });
        }

        #endregion
    }
}
