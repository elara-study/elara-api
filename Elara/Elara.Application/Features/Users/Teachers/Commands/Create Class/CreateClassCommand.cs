using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Administrative.Classes.Commands.Create_Class
{
    public class CreateClassCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }

        public string Subject { get; set; } = string.Empty;
        public string? RoadmapName { get; set; }
        public Guid TeacherId { get; set; }
    }
}
