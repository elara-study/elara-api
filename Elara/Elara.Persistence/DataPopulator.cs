using Elara.Domain.Entities.Educational;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Scripts
{
    public class DataPopulator
    {
        public static async Task Run(AppDbContext context)
        {
            // 1. Physics - 3rd Secondary
            var physics = new Subject 
            { 
                Name = "Physics - 3rd Secondary", 
                Description = "Egyptian curriculum for Physics - covering Electricity, Magnetism, and Modern Physics." 
            };
            await context.Subjects.AddAsync(physics);
            await context.SaveChangesAsync();

            var teacher = await context.Teachers.FirstOrDefaultAsync();
            if (teacher == null)
            {
                throw new Exception("Please create at least one Teacher/User in the system first so the seed can link to it.");
            }

            // Create a Roadmap 
            var physicsRoadmap = new Roadmap
            {
                Name = "Physics Mastery Roadmap",
                Description = "Step by step guide to master 3rd Secondary Physics.",
                SubjectId = physics.Id,
                TeacherId = teacher.Id,
                Grade = Elara.Domain.Enums.GradeLevel.Grade12
            };
            await context.Roadmaps.AddAsync(physicsRoadmap);
            await context.SaveChangesAsync();

            // Unit 1: Electric Current and Ohm's Law
            var electricity = new Topic 
            { 
                Title = "Electric Current and Ohm's Law", 
                Description = "Study of current intensity, potential difference, resistance, and Kirchhoff's laws.", 
                SubjectId = physics.Id,
                RoadmapId = physicsRoadmap.Id 
            };
            await context.Topics.AddAsync(electricity);
            await context.SaveChangesAsync();

            var physicsLessons = new List<Lesson>
            {
                new Lesson { Title = "Ohm's Law for Closed Circuits", Content = "Explaining electromotive force (EMF), internal resistance, and Ohm's Law.", TopicId = electricity.Id },
                new Lesson { Title = "Resistors in Series and Parallel", Content = "Methods of calculating equivalent resistance in complex circuits.", TopicId = electricity.Id },
                new Lesson { Title = "Kirchhoff's Laws", Content = "Solving complex electrical circuits using Kirchhoff's first and second laws.", TopicId = electricity.Id }
            };
            await context.Lessons.AddRangeAsync(physicsLessons);

            // 2. Chemistry - 3rd Secondary
            var chemistry = new Subject 
            { 
                Name = "Chemistry - 3rd Secondary", 
                Description = "Egyptian curriculum for Chemistry - Transition Elements, Chemical Analysis, and Organic Chemistry." 
            };
            await context.Subjects.AddAsync(chemistry);
            await context.SaveChangesAsync();

            // Create a Roadmap for Chemistry
            var chemistryRoadmap = new Roadmap
            {
                Name = "Chemistry Mastery Roadmap",
                Description = "A complete roadmap for 3rd Secondary Organic Chemistry.",
                SubjectId = chemistry.Id,
                TeacherId = teacher.Id,
                Grade = Elara.Domain.Enums.GradeLevel.Grade12
            };
            await context.Roadmaps.AddAsync(chemistryRoadmap);
            await context.SaveChangesAsync();

            // Chapter 5: Organic Chemistry
            var organic = new Topic 
            { 
                Title = "Organic Chemistry", 
                Description = "Study of hydrocarbons, Alkanes, Alkenes, and Alkynes.", 
                SubjectId = chemistry.Id,
                RoadmapId = chemistryRoadmap.Id
            };
            await context.Topics.AddAsync(organic);
            await context.SaveChangesAsync();

            var chemLessons = new List<Lesson>
            {
                new Lesson { Title = "Introduction to Organic Chemistry", Content = "Difference between organic and inorganic compounds and chemical formulas.", TopicId = organic.Id },
                new Lesson { Title = "Alkanes (Methane)", Content = "Naming Alkanes, properties of methane and its reactions.", TopicId = organic.Id },
                new Lesson { Title = "Aromatic Benzene", Content = "Structure of benzene, methods of preparation and substitution reactions.", TopicId = organic.Id }
            };
            await context.Lessons.AddRangeAsync(chemLessons);
            await context.SaveChangesAsync();
        }
    }
}
