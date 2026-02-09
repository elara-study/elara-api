using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoQueryValidator:AbstractValidator<GetClassInfoQuery>
    {
        public GetClassInfoQueryValidator()
        {
            RuleFor(x => x.ClassId).NotEmpty().WithMessage("Class id is required.")
                .GreaterThan(0).WithMessage("Class ID must be greater than 0.");
        }
    }
}
