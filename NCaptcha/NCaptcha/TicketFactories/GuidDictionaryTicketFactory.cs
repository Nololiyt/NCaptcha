using Nololiyt.Captcha.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.TicketFactories
{
    public sealed class GuidDictionaryTicketFactory : ITicketFactory
    {
        private readonly ConcurrentDictionary<Guid, DateTime?> tickets
            = new ConcurrentDictionary<Guid, DateTime?>();
        private readonly TimeSpan? ticketsLifeTime;
        public GuidDictionaryTicketFactory(TimeSpan? ticketsLifeTime)
        {
            if (ticketsLifeTime <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(ticketsLifeTime));
            this.ticketsLifeTime = ticketsLifeTime;
        }

        public TimeSpan? TicketsLifeTime => this.ticketsLifeTime;

        private bool disposedValue = false;
        public ValueTask DisposeAsync()
        {
            this.disposedValue = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string> GenerateNewAsync(CancellationToken cancellationToken = default)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(null);
            var time = DateTime.UtcNow + this.ticketsLifeTime;

            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (!this.tickets.TryAdd(guid, time));

            return ValueTask.FromResult(guid.ToString("N"));
        }
        public ValueTask<bool> VerifyAsync(string ticket, CancellationToken cancellationToken = default)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(null);
            if (!Guid.TryParseExact(ticket, "N", out var guid))
                return ValueTask.FromResult(false);
            if (!this.tickets.TryRemove(guid, out var time))
                return ValueTask.FromResult(false);
            return ValueTask.FromResult(!time.HasValue || time >= DateTime.Now);
        }
    }
}
