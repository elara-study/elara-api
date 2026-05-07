using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommandValidator:AbstractValidator<MarkAsReadCommand>
    {
        public MarkAsReadCommandValidator()
        {
            RuleFor(n => n.UserId).NotEqual(Guid.Empty).WithMessage("User Id is required");

            RuleFor(n => n.NotificationId).NotEqual(Guid.Empty).WithMessage("Notification Id is required");

        }
    }
}
