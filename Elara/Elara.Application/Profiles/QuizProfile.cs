using AutoMapper;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;

namespace Elara.Application.Profiles
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<QuizSession, QuizSessionDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Assignment.Topic.ModuleName ?? "General Module"))
                .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Assignment.Lesson != null ? src.Assignment.Lesson.Title : src.Assignment.Title))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Assignment.Topic.Subject.Name))
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Assignment.Questions.Count));

            CreateMap<Question, QuizQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.QuestionType.ToString()))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel.ToString()))
                .ForMember(dest => dest.HasHint, opt => opt.MapFrom(src => src.Hints.Any()));

            CreateMap<QuestionOption, QuestionOptionDto>()
                .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<QuizSession, QuizResultDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => src.Assignment.Title))
                .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Assignment.Lesson != null ? src.Assignment.Lesson.Title : ""))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Assignment.Topic.Subject.Name))
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.StudentProgress, opt => opt.MapFrom(src => src.Student))
                .ForMember(dest => dest.ElaraInsight, opt => opt.MapFrom(src => new ElaraInsightDto { 
                    Message = src.ElaraInsight ?? "", 
                    Recommendation = src.InsightRecommendation ?? "",
                    WeakTopics = (src.WeakTopics ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                }));

            CreateMap<QuizSession, QuizStatsDto>()
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Assignment.Questions.Count))
                .ForMember(dest => dest.AccuracyPercentage, opt => opt.MapFrom(src => src.Assignment.Questions.Count > 0 
                    ? (double)src.CorrectAnswers / src.Assignment.Questions.Count * 100 : 0))
                .ForPath(dest => dest.XpBreakdown.BaseXP, opt => opt.MapFrom(src => src.XpEarned)) 
                .ForPath(dest => dest.XpBreakdown.FinalXP, opt => opt.MapFrom(src => src.XpEarned));

            CreateMap<Student, StudentProgressDto>();

            CreateMap<Assignment, GeneratedQuizDto>()
                .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Title))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Topic.Subject.Name))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel.ToString()));

            CreateMap<Hint, HintDto>()
                .ForMember(dest => dest.HintId, opt => opt.MapFrom(src => src.Id));

            CreateMap<QuizSession, QuizHistoryDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => src.Assignment.Title))
                .ForMember(dest => dest.AccuracyPercentage, opt => opt.MapFrom(src => src.Assignment.Questions.Count > 0 
                    ? (double)src.CorrectAnswers / src.Assignment.Questions.Count * 100 : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
