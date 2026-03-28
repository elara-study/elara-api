using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses
{
    public class GetTeacherClassesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int Grade { get; set; }
        public int StudentsCount { get; set; }
    }
}
