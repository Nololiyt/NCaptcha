using Nololiyt.Captcha.Interfaces.Entities;
using System.Drawing;

namespace Nololiyt.Captcha.CaptchaFactories.Image
{
    internal sealed class Captcha : ICaptcha<Bitmap>
    {
        public string Id { get; init; } = null!;

        public Bitmap Display { get; init; } = default!;
    }
}
