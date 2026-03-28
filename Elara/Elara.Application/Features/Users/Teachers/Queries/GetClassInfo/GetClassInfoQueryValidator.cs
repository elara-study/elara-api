using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.GetClassInfo
{
    public class GetClassInfoQueryValidator:AbstractValidator<GetClassInfoQuery>
    {
        public GetClassInfoQueryValidator()
        {
            RuleFor(x => x.ClassId).NotEmpty().WithMessage("Class id is required.")
                .NotEqual(Guid.Empty).WithMessage("Class ID must be a valid UUID.");
        }
    }
}
