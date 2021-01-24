using Nololiyt.Captcha.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.CaptchaFactories
{
    /// <summary>
    /// Represents a captcha factory's base but not all factories should extend this.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the answer.</typeparam>
    public abstract class StaticAnswerCaptchaFactory<TAnswer> : IDisposable
    {
        private readonly ICaptchaAnswerSaver<TAnswer> answerSaver;
        private readonly ITicketFactory ticketFactory;

        /// <summary>
        /// Get the bound ticket factory.
        /// </summary>
        public ITicketFactory TicketFactory => this.ticketFactory;
        private readonly bool disposeSaverAndFactory;
        /// <summary>
        /// Initialize a new instance of <see cref="StaticAnswerCaptchaFactory{TAnswer}"/> 。
        /// </summary>
        /// <param name="answerSaver">The answer saver.</param>
        /// <param name="ticketFactory">The ticket factory.</param>
        /// <param name="disposeSaverAndFactory">Whether to dispose the saver and the factory or not when being disposed.</param>
        public StaticAnswerCaptchaFactory(ICaptchaAnswerSaver<TAnswer> answerSaver,
            ITicketFactory ticketFactory, bool disposeSaverAndFactory = false)
        {
            if (answerSaver == null)
                throw new ArgumentNullException(nameof(answerSaver));
            if (ticketFactory == null)
                throw new ArgumentNullException(nameof(ticketFactory));
            this.answerSaver = answerSaver;
            this.disposeSaverAndFactory = disposeSaverAndFactory;
            this.ticketFactory = ticketFactory;
        }

        /// <summary>
        /// Save an answer into with the answer saver.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The generated identification.</returns>
        protected async ValueTask<string> SaveAnswerAsync(TAnswer answer,
            CancellationToken cancellationToken = default)
        {
            return await this.answerSaver.SaveAsync(answer, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Try to get the answer with the answer saver.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The answer. Or <c>null</c> if answer not found.</returns>
        protected async ValueTask<TAnswer?> TryGetAnswerAsync(string id, CancellationToken cancellationToken = default)
        {
            return await this.answerSaver.TryGetAsync(id, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Create a new ticket with the ticket factory.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The ticket.</returns>
        protected async ValueTask<string> NewTicketAsync(CancellationToken cancellationToken = default)
        {
            return await this.ticketFactory.GenerateNewAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        #region disposing

        /// <summary>
        /// When overridden in a subclass, dispose the objects as needed.
        /// </summary>
        protected virtual void DisposeMore() { }

        /// <summary>
        /// Get a value indicates whether the instance has been disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.disposeSaverAndFactory)
                {
                    this.answerSaver.Dispose();
                    this.ticketFactory.Dispose();
                    this.DisposeMore();
                }
                this.IsDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
