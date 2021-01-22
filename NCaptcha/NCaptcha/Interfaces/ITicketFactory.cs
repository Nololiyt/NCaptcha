using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Interfaces
{
    /// <summary>
    /// Represents a ticket factory which can produce tickets.
    /// </summary>
    public interface ITicketFactory : IDisposable
    {
        /// <summary>
        /// The life time of the produced tickets.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        TimeSpan? TicketsLifeTime { get; }
        /// <summary>
        /// Verify a ticket and make it invalid.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether the ticket is valid or not.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="ticket"/> is <c>null</c>.</exception>
        ValueTask<bool> VerifyAsync(string ticket, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generate a new ticket.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The ticket.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        ValueTask<string> GenerateNewAsync(CancellationToken cancellationToken = default);
    }
}
