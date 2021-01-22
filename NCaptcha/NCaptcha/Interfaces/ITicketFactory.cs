using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Interfaces
{
    public interface ITicketFactory : IAsyncDisposable
    {
        TimeSpan? TicketsLifeTime { get; }
        ValueTask<bool> VerifyAsync(string ticket, CancellationToken cancellationToken = default);
        ValueTask<string> GenerateNewAsync(CancellationToken cancellationToken = default);
    }
}
