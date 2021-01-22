using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Interfaces
{
    /// <summary>
    /// Represents an answer saver to save captcha answers.
    /// </summary>
    /// <typeparam name="TAnswer"></typeparam>
    public interface ICaptchaAnswerSaver<TAnswer> : IAsyncDisposable
    {
        /// <summary>
        /// The life time of an answer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        TimeSpan? AnswersLifeTime { get; }

        /// <summary>
        /// Save an answer.
        /// </summary>
        /// <param name="answer">The answer to save.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The generated identification.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="answer"/> is <c>null</c>.</exception>
        ValueTask<string> SaveAsync(
            TAnswer answer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Try to get an answer with the identification and remove it in the meantime.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The answer. Or <c>null</c> if answer not found.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
        ValueTask<TAnswer?> TryGetAndRemoveAsync(
            string id, CancellationToken cancellationToken = default);
    }
}
