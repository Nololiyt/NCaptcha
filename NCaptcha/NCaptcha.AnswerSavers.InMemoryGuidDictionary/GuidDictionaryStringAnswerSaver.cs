﻿using Nololiyt.Captcha.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.AnswerSavers.InMemoryGuidDictionary
{
    /// <summary>
    /// Represents an implementation of <see cref="ICaptchaAnswerSaver{TAnswer}"/>,
    /// which use <see cref="ConcurrentDictionary{TKey, TValue}"/> to save <see cref="string"/> answers, 
    /// and <see cref="Guid"/> to generate identifications.
    /// </summary>
    public sealed class GuidDictionaryStringAnswerSaver : ICaptchaAnswerSaver<string>
    {
        private readonly ConcurrentDictionary<Guid, (string, DateTime?)> answers
            = new ConcurrentDictionary<Guid, (string, DateTime?)>();
        private readonly TimeSpan? answersLifeTime;

        /// <summary>
        /// Initialize a new instance of <see cref="GuidDictionaryStringAnswerSaver"/>.
        /// </summary>
        /// <param name="answersLifeTime"></param>
        public GuidDictionaryStringAnswerSaver(TimeSpan? answersLifeTime)
        {
            if (answersLifeTime <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(answersLifeTime));
            this.answersLifeTime = answersLifeTime;
        }

        /// <summary>
        /// The life time of an answer.
        /// </summary>
        public TimeSpan? AnswersLifeTime => this.answersLifeTime;

        private bool disposedValue = false;
        /// <summary>
        /// Dispose the instance.
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            this.disposedValue = true;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Save an answer.
        /// </summary>
        /// <param name="answer">The answer to save.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The generated identification.</returns>
        public ValueTask<string> SaveAsync(string answer, CancellationToken cancellationToken = default)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(null);
            var time = DateTime.UtcNow + this.answersLifeTime;

            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (!this.answers.TryAdd(guid, (answer, time)));

            return ValueTask.FromResult(guid.ToString("N"));
        }

        /// <summary>
        /// Try to get an answer with the identification and remove it in the meantime.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The answer. Or <c>null</c> if answer not found.</returns>
        public ValueTask<string?> TryGetAndRemoveAsync(string id,
            CancellationToken cancellationToken = default)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(null);
            if (!Guid.TryParseExact(id, "N", out var guid))
                return ValueTask.FromResult((string?)null);
            if (!this.answers.TryRemove(guid, out var result))
                return ValueTask.FromResult((string?)null);
            var (answer, time) = result;
            return ValueTask.FromResult(!time.HasValue || time >= DateTime.UtcNow ? answer : null);
        }
    }
}