using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandValidator:AbstractValidator<MarkAllAsReadCommand>
    {
        public MarkAllAsReadCommandValidator()
        {
            RuleFor(n => n.UserId).NotEqual(Guid.Empty).WithMessage("UserId is required");
           
        }
    }
}
