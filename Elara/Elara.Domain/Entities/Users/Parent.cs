using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Users
{
    public class Parent : User
    {
        // Navigation Properties
        public virtual ICollection<Student> Children { get; set; } = new List<Student>();
    }
}
