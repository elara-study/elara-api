using AutoMapper;
using Elara.Application.Common.Gamification;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using System.Text.Json;

namespace Elara.Application.Profiles
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<QuizSession, QuizSessionDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module != null ? src.Module.Title : ""))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Module != null && src.Module.Subject != null ? src.Module.Subject.Name : ""))
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => GetQuestionCount(src.QuestionsJson)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<QuizSession, QuizResultDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => GetQuizTitle(src.QuestionsJson)))
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.StudentProgress, opt => opt.MapFrom(src => src.Student))
                .ForMember(dest => dest.ElaraInsight, opt => opt.MapFrom(src => src.ElaraInsight));

            CreateMap<QuizSession, QuizStatsDto>()
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => GetQuestionCount(src.QuestionsJson)))
                .ForMember(dest => dest.AccuracyPercentage, opt => opt.MapFrom(src => CalcAccuracy(src)));
            CreateMap<Student, StudentProgressDto>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Elara.Application.Common.Gamification.StudentGamification.CalculateLevel(src.TotalXP)));

            CreateMap<QuizSession, QuizHistoryDto>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => GetQuizTitle(src.QuestionsJson)))
                .ForMember(dest => dest.AccuracyPercentage, opt => opt.MapFrom(src => CalcAccuracy(src)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module != null ? src.Module.Title : ""));
        }

        private static double CalcAccuracy(QuizSession src)
        {
            var total = GetQuestionCount(src.QuestionsJson);
            return total > 0 ? (double)src.CorrectAnswers / total * 100 : 0;
        }

        private static int GetQuestionCount(string? questionsJson)
        {
            if (string.IsNullOrEmpty(questionsJson)) return 0;
            try
            {
                var data = JsonSerializer.Deserialize<AIQuizResult>(questionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return data?.Questions?.Count ?? 0;
            }
            catch { return 0; }
        }

        private static string GetQuizTitle(string? questionsJson)
        {
            if (string.IsNullOrEmpty(questionsJson)) return "Quiz";
            try
            {
                var data = JsonSerializer.Deserialize<AIQuizResult>(questionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return data?.Title ?? "Quiz";
            }
            catch { return "Quiz"; }
        }

        private class AIQuizResult
        {
            public string Title { get; set; } = string.Empty;
            public List<AIQuestion> Questions { get; set; } = new();
        }

        private class AIQuestion
        {
            public string Text { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }
    }
}
