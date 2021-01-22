using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Interfaces.Entities
{
    /// <summary>
    /// Represents a captcha which is often produced by a captcha factory.
    /// </summary>
    /// <typeparam name="TCaptchaDisplay">Type of the captcha's display.</typeparam>
    public interface ICaptcha<TCaptchaDisplay>
    {
        /// <summary>
        /// Get the captcha's identification.
        /// Clients shall call the captcha factory to verify the answer with this later.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Get the captcha's display.
        /// Clients shall show something transformed from this to its users, so that they can input the right answer.
        /// </summary>
        TCaptchaDisplay Display { get; }
    }
    internal sealed class Captcha<TCaptchaDisplay> : ICaptcha<TCaptchaDisplay>
    {
        public string Id { get; init; } = null!;

        public TCaptchaDisplay Display { get; init; } = default!;
    }
}
