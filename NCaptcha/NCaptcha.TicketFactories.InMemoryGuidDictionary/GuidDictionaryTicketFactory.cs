using Nololiyt.Captcha.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.TicketFactories.InMemoryGuidDictionary
{
    /// <summary>
    /// Represents an implementation of <see cref="ITicketFactory"/>,
    /// which use <see cref="ConcurrentDictionary{TKey, TValue}"/> to save tickets, 
    /// and <see cref="Guid"/> to generate identifications.
    /// </summary>
    public sealed class GuidDictionaryTicketFactory : ITicketFactory
    {
        private readonly ConcurrentDictionary<Guid, DateTime?> tickets
            = new ConcurrentDictionary<Guid, DateTime?>();
        private readonly TimeSpan? ticketsLifeTime;

        /// <summary>
        /// Initialize a new instance of <see cref="GuidDictionaryTicketFactory"/>.
        /// </summary>
        /// <param name="ticketsLifeTime"></param>
        public GuidDictionaryTicketFactory(TimeSpan? ticketsLifeTime)
        {
            if (ticketsLifeTime <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(ticketsLifeTime));
            this.ticketsLifeTime = ticketsLifeTime;
        }

        /// <summary>
        /// The life time of the produced tickets.
        /// </summary>
        public TimeSpan? TicketsLifeTime => this.ticketsLifeTime;

        private bool disposedValue = false;
        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            this.disposedValue = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Generate a new ticket.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The ticket.</returns>
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
        /// <summary>
        /// Verify a ticket and make it invalid.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether the ticket is valid or not.</returns>
        public ValueTask<bool> VerifyAsync(string ticket, CancellationToken cancellationToken = default)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(null);
            if (!Guid.TryParseExact(ticket, "N", out var guid))
                return ValueTask.FromResult(false);
            if (!this.tickets.TryRemove(guid, out var time))
                return ValueTask.FromResult(false);
            return ValueTask.FromResult(!time.HasValue || time >= DateTime.UtcNow);
        }
    }
}
