using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendToTopicAsync(string topic , string title , string body ,Dictionary<string,string>?data=null,CancellationToken ct = default);
        Task SendToTokenAsync(IEnumerable<string> tokens,string title , string body ,Dictionary<string,string>?data=null, CancellationToken ct = default);
    }
}
