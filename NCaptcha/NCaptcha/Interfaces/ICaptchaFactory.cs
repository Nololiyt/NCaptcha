using Nololiyt.Captcha.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Interfaces
{
    /// <summary>
    /// Represents a captcha factory which can produce captcha.
    /// </summary>
    /// <typeparam name="TCaptchaDisplay">Type of the captcha's display.</typeparam>
    /// <typeparam name="TAnswer">Type of the captcha's answer.</typeparam>
    public interface ICaptchaFactory<TCaptchaDisplay, TAnswer> : IDisposable
    {
        /// <summary>
        /// Get the bound answer saver.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        ICaptchaAnswerSaver<TAnswer> AnswerSaver { get; }
        /// <summary>
        /// Get the bound ticket factory.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        ITicketFactory TicketFactory { get; }
        /// <summary>
        /// Verify an answer.
        /// </summary>
        /// <param name="id">The identification of the captcha.</param>
        /// <param name="answer">The answer to verify.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The ticket. Or <c>null</c> if the answer isn't correct.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> or <paramref name="answer"/> is null.</exception>
        ValueTask<string?> VerifyAndGetTicketAsync(string id, TAnswer answer, CancellationToken cancellationToken = default);
        /// <summary>
        /// Generate a new captcha.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The captcha.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        ValueTask<ICaptcha<TCaptchaDisplay>> GenerateNewAsync(CancellationToken cancellationToken = default);
    }
}
