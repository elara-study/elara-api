using Elara.Domain.Entities.Educational;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Elara.Scripts
{
    public class DataPopulator
    {
        public static async Task Run(AppDbContext context)
        {
            var physics = await context.Subjects.FirstOrDefaultAsync(s => s.Name == "Physics - 3rd Secondary");
            if (physics == null)
            {
                physics = new Subject
                {
                    Name = "Physics - 3rd Secondary",
                    Description = "Egyptian curriculum for Physics - covering Electricity, Magnetism, and Modern Physics."
                };
                await context.Subjects.AddAsync(physics);
                await context.SaveChangesAsync();
            }

            var teacher = await context.Teachers.FirstOrDefaultAsync();
            if (teacher == null)
            {
                throw new Exception("Please create at least one Teacher/User in the system first so the seed can link to it.");
            }

            if (!await context.Roadmaps.AnyAsync(r => r.Name == "Physics Mastery Roadmap" && r.TeacherId == teacher.Id))
            {
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

                var electricity = new Module
                {
                    Title = "Electric Current and Ohm's Law",
                    Description = "Study of current intensity, potential difference, resistance, and Kirchhoff's laws.",
                    SubjectId = physics.Id,
                    RoadmapId = physicsRoadmap.Id
                };
                await context.Modules.AddAsync(electricity);
                await context.SaveChangesAsync();

                var physicsHomework = new List<Homework>
                {
                    new Homework { Title = "Ohm's Law for Closed Circuits", Content = "Explaining electromotive force (EMF), internal resistance, and Ohm's Law.", ModuleId = electricity.Id },
                    new Homework { Title = "Resistors in Series and Parallel", Content = "Methods of calculating equivalent resistance in complex circuits.", ModuleId = electricity.Id },
                    new Homework { Title = "Kirchhoff's Laws", Content = "Solving complex electrical circuits using Kirchhoff's first and second laws.", ModuleId = electricity.Id }
                };
                await context.Homework.AddRangeAsync(physicsHomework);
            }

            var chemistry = await context.Subjects.FirstOrDefaultAsync(s => s.Name == "Chemistry - 3rd Secondary");
            if (chemistry == null)
            {
                chemistry = new Subject
                {
                    Name = "Chemistry - 3rd Secondary",
                    Description = "Egyptian curriculum for Chemistry - Transition Elements, Chemical Analysis, and Organic Chemistry."
                };
                await context.Subjects.AddAsync(chemistry);
                await context.SaveChangesAsync();
            }

            if (!await context.Roadmaps.AnyAsync(r => r.Name == "Chemistry Mastery Roadmap" && r.TeacherId == teacher.Id))
            {
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

                var organic = new Module
                {
                    Title = "Organic Chemistry",
                    Description = "Study of hydrocarbons, Alkanes, Alkenes, and Alkynes.",
                    SubjectId = chemistry.Id,
                    RoadmapId = chemistryRoadmap.Id
                };
                await context.Modules.AddAsync(organic);
                await context.SaveChangesAsync();

                var chemHomework = new List<Homework>
                {
                    new Homework { Title = "Introduction to Organic Chemistry", Content = "Difference between organic and inorganic compounds and chemical formulas.", ModuleId = organic.Id },
                    new Homework { Title = "Alkanes (Methane)", Content = "Naming Alkanes, properties of methane and its reactions.", ModuleId = organic.Id },
                    new Homework { Title = "Aromatic Benzene", Content = "Structure of benzene, methods of preparation and substitution reactions.", ModuleId = organic.Id }
                };
                await context.Homework.AddRangeAsync(chemHomework);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedPhysicsForTeacher(AppDbContext context, string teacherEmail)
        {
            var appUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == teacherEmail);
            if (appUser == null)
            {
                throw new Exception($"User with email '{teacherEmail}' not found.");
            }

            var teacher = await context.Teachers
                .FirstOrDefaultAsync(t => t.Id == appUser.Id);

            if (teacher == null)
            {
                throw new Exception($"Teacher with email '{teacherEmail}' not found.");
            }

            var physicsSubject = await context.Subjects.FirstOrDefaultAsync(s => s.Name == "Physics - 3rd Secondary");
            if (physicsSubject == null)
            {
                physicsSubject = new Subject { Name = "Physics - 3rd Secondary", Description = "Egyptian curriculum for Physics" };
                await context.Subjects.AddAsync(physicsSubject);
                await context.SaveChangesAsync();
            }

            if (teacher.SubjectId != physicsSubject.Id)
            {
                teacher.SubjectId = physicsSubject.Id;
                await context.SaveChangesAsync();
            }

            var existing = await context.Roadmaps
                .Include(r => r.Modules)
                .FirstOrDefaultAsync(r => r.Name == "Physics Mastery Roadmap" && r.TeacherId == teacher.Id);
            if (existing != null)
            {
                if (existing.Modules.Count > 0)
                {
                    Console.WriteLine($"✅ Physics roadmap already exists for '{teacherEmail}'. Skipping.");
                    return;
                }
                Console.WriteLine($"⚠️ Physics roadmap exists but has no modules. Recreating modules...");
            }

            Roadmap physicsRoadmap;
            if (existing != null)
            {
                physicsRoadmap = existing;
            }
            else
            {
                physicsRoadmap = new Roadmap
                {
                    Name = "Physics Mastery Roadmap",
                    Description = "Step by step guide to master 3rd Secondary Physics.",
                    Grade = Elara.Domain.Enums.GradeLevel.Grade12,
                    SubjectId = physicsSubject.Id,
                    TeacherId = teacher.Id
                };
                await context.Roadmaps.AddAsync(physicsRoadmap);
                await context.SaveChangesAsync();
            }

            var modules = new List<Module>
            {
                new()
                {
                    Title = "Electric Current and Ohm's Law",
                    Description = "Study of current intensity, potential difference, resistance, and Kirchhoff's laws.",
                    SubjectId = physicsSubject.Id,
                    RoadmapId = physicsRoadmap.Id
                },
                new()
                {
                    Title = "Magnetism and Electromagnetic Induction",
                    Description = "Magnetic fields, flux, Faraday's law, Lenz's law, and AC generators.",
                    SubjectId = physicsSubject.Id,
                    RoadmapId = physicsRoadmap.Id
                },
                new()
                {
                    Title = "Modern Physics",
                    Description = "Quantum theory, photoelectric effect, atomic models, and nuclear physics.",
                    SubjectId = physicsSubject.Id,
                    RoadmapId = physicsRoadmap.Id
                },
                new()
                {
                    Title = "Wave Motion and Sound",
                    Description = "Wave properties, interference, Doppler effect, and acoustic phenomena.",
                    SubjectId = physicsSubject.Id,
                    RoadmapId = physicsRoadmap.Id
                }
            };
            await context.Modules.AddRangeAsync(modules);
            await context.SaveChangesAsync();

            var homeworks = new List<Homework>
            {
                new() { Title = "Ohm's Law for Closed Circuits", Content = "Electromotive force (EMF), internal resistance, and Ohm's Law.", ModuleId = modules[0].Id },
                new() { Title = "Resistors in Series and Parallel", Content = "Calculating equivalent resistance in complex circuits.", ModuleId = modules[0].Id },
                new() { Title = "Kirchhoff's Laws", Content = "Solving complex circuits using Kirchhoff's first and second laws.", ModuleId = modules[0].Id },
                new() { Title = "Magnetic Flux and Faraday's Law", Content = "Magnetic flux, induced EMF, and Faraday's experiments.", ModuleId = modules[1].Id },
                new() { Title = "Lenz's Law and Applications", Content = "Direction of induced current and practical applications.", ModuleId = modules[1].Id },
                new() { Title = "AC Circuits and Generators", Content = "Alternating current, transformers, and power transmission.", ModuleId = modules[1].Id },
                new() { Title = "Photoelectric Effect", Content = "Einstein's photoelectric equation, work function, and threshold frequency.", ModuleId = modules[2].Id },
                new() { Title = "Atomic Models and Nuclear Physics", Content = "Rutherford and Bohr models, radioactivity, and nuclear reactions.", ModuleId = modules[2].Id },
                new() { Title = "Wave Properties and Doppler Effect", Content = "Wave types, superposition, and Doppler shift in sound.", ModuleId = modules[3].Id }
            };
            await context.Homework.AddRangeAsync(homeworks);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded Physics roadmap for teacher '{teacherEmail}' with {modules.Count} modules and {homeworks.Count} homeworks.");
        }
    }
}
