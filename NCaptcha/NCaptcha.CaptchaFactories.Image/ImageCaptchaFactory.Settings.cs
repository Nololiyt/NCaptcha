using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Nololiyt.Captcha.CaptchaFactories.Image
{
    public partial class ImageCaptchaFactory
    {
        /// <summary>
        /// The settings of the <see cref="ImageCaptchaFactory"/>.
        /// </summary>
        public sealed class Settings
        {
            internal IEnumerable<Font> allowedFonts = new Font[] {
                new Font("Arial", 13, FontStyle.Bold | FontStyle.Italic)
            };
            /// <summary>
            /// Get or set the font allowed.
            /// The default value just contains one item: arial with bold and italic as style and 13 as em-size.
            /// </summary>
            /// <exception cref="ArgumentNullException">A <c>null</c> value is going to be set.</exception>
            /// <exception cref="ArgumentException">A value without any items is going to be set.</exception>

            public IEnumerable<Font> AllowedFonts
            {
                get
                {
                    return this.allowedFonts;
                }
                init
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    if (!value.Any())
                        throw new ArgumentException("At least one font should be allowed.", nameof(value));
                    this.allowedFonts = value;
                }
            }

            internal IEnumerable<int> allowedLengths = new int[] { 4 };
            /// <summary>
            /// Get or set the length allowed.
            /// The default value is <c>{ 4 }</c>.
            /// </summary>
            /// <exception cref="ArgumentNullException">A <c>null</c> value is going to be set.</exception>
            /// <exception cref="ArgumentException">A value without any items is going to be set.</exception>

            public IEnumerable<int> AllowedLengths
            {
                get
                {
                    return this.allowedLengths;
                }
                init
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    if (!value.Any())
                        throw new ArgumentException("At least one length should be allowed.", nameof(value));
                    this.allowedLengths = value;
                }
            }


            internal IEnumerable<char> allowedCharacters =
                "ABDEFGHIJKLMNQRSTabdefghijkmnqrst23456789";
            /// <summary>
            /// Get or set the characters allowed.
            /// The default value is <c>"ABDEFGHIJKLMNQRSTabdefghijkmnqrst23456789"</c>.
            /// </summary>
            /// <exception cref="ArgumentNullException">A <c>null</c> value is going to be set.</exception>
            /// <exception cref="ArgumentException">A value without any items is going to be set.</exception>
            public IEnumerable<char> AllowedCharacters
            {
                get
                {
                    return this.allowedCharacters;
                }
                init
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    if (!value.Any())
                        throw new ArgumentException("At least one character should be allowed.", nameof(value));
                    this.allowedCharacters = value;
                }
            }
        }
    }
}
