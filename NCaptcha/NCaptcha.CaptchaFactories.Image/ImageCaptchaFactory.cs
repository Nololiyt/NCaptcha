using Nololiyt.Captcha.CaptchaFactories.Image.Extensions;
using Nololiyt.Captcha.Interfaces;
using Nololiyt.Captcha.Interfaces.Entities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.CaptchaFactories.Image
{
    /// <summary>
    /// Represents an implementation of <see cref="ICaptchaFactory{TCaptchaDisplay, TAnswer}"/>,
    /// which produce bitmap displays and users should correctly input the characters on the bitmap as the answer.
    /// </summary>
    public sealed partial class ImageCaptchaFactory : StaticAnswerCaptchaFactory<string>, ICaptchaFactory<Bitmap, string>
    {
        private readonly int[] lengths;
        private readonly char[] characters;
        private readonly Font[] fonts;

        /// <summary>
        /// Initialize a new instance of <see cref="ImageCaptchaFactory"/>.
        /// </summary>
        /// <param name="answerSaver">The answer saver.</param>
        /// <param name="ticketFactory">The ticket factory.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="disposeSaverAndFactory">Whether to dispose the saver and the factory or not when being disposed.</param>
        public ImageCaptchaFactory(ICaptchaAnswerSaver<string> answerSaver,
            ITicketFactory ticketFactory, Settings settings, bool disposeSaverAndFactory = false)
            : base(answerSaver, ticketFactory, disposeSaverAndFactory)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(null);
            if (answerSaver == null)
                throw new ArgumentNullException(nameof(answerSaver));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            this.lengths = settings.AllowedLengths.ToArray();
            this.fonts = settings.AllowedFonts.ToArray();
            this.characters = settings.AllowedCharacters.ToArray();
        }

        /// <summary>
        /// Generate a new captcha.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The captcha.</returns>
        public async ValueTask<ICaptcha<Bitmap>> GenerateNewAsync(CancellationToken cancellationToken = default)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(null);
            var random = RandomValueGenerator.GetRandom();
            return await Task.Run(async () =>
            {
                string resultString;
                {
                    var length = this.lengths[random.Next(this.lengths.Length)];
                    StringBuilder builder = new StringBuilder(length);
                    for (int i = 0; i < length; i++)
                        builder.Append(this.characters[random.Next(this.characters.Length)]);
                    resultString = builder.ToString();
                }
                Bitmap image = new Bitmap(resultString.Length * 16, 27);
                using Graphics g = Graphics.FromImage(image);
                g.Clear(Color.White);
                using var silverPen = new Pen(Color.Silver);
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(silverPen, x1, x2, y1, y2);
                }
                using LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(resultString, this.fonts[random.Next(this.fonts.Length)], brush, 3, 2);

                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                g.DrawRectangle(silverPen, 0, 0, image.Width - 1, image.Height - 1);
                var id = await this.SaveAnswerAsync(resultString, cancellationToken).ConfigureAwait(false);
                return new Captcha() {
                    Display = image,
                    Id = id
                };
            }, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Verify an answer.
        /// </summary>
        /// <param name="id">The identification of the captcha.</param>
        /// <param name="answer">The answer to verify.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The ticket. Or <c>null</c> if the answer isn't correct.</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        public async ValueTask<string?> VerifyAndGetTicketAsync(string id, string answer,
            CancellationToken cancellationToken = default)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(null);
            var realAnswer = await this.TryGetAnswerAsync(id, cancellationToken).ConfigureAwait(false);
            if (realAnswer == null)
                return null;
            if (realAnswer.ToLower() != answer.ToLower())
                return null;
            return await this.NewTicketAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}