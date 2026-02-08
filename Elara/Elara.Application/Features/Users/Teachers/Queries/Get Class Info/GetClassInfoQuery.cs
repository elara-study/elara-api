using AutoMapper.Configuration.Conventions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoQuery : IRequest<GetClassInfoResponse>
    {
        public int ClassId { get; set; }
    }
}
