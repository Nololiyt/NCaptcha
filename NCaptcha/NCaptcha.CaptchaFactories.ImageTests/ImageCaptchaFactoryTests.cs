using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nololiyt.Captcha.CaptchaFactories.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nololiyt.Captcha.AnswerSavers.InMemoryGuidDictionary;
using Nololiyt.Captcha.TicketFactories.InMemoryGuidDictionary;

namespace Nololiyt.Captcha.CaptchaFactories.Image.Tests
{
    [TestClass()]
    public class ImageCaptchaFactoryTests
    {
        [TestMethod()]
        public void ImageCaptchaFactoryTest()
        {
            var q = new GuidDictionaryTicketFactory(new TimeSpan(24, 0, 0));
            using var fac = new ImageCaptchaFactory(
                new GuidDictionaryStringAnswerSaver(new TimeSpan(24, 0, 0)),
                q,
                new ImageCaptchaFactory.Settings() {
                }, true);
            var cap = fac.GenerateNewAsync().AsTask().Result;
            cap.Display.Save("abc.png");
            // Human test required. You should input it correct.
            var humanInputHere = "";
            var tic = fac.VerifyAndGetTicketAsync(cap.Id, humanInputHere).AsTask().Result;
            Assert.AreNotEqual(null, tic);
            var check = q.VerifyAsync(tic).AsTask().Result;
            Assert.AreEqual(true, check);


            var cap2 = fac.GenerateNewAsync().AsTask().Result;
            cap2.Display.Save("abc.png");
            // Human test required. You should input it wrong.
            var humanInputHere2 = "";
            var tic2 = fac.VerifyAndGetTicketAsync(cap2.Id, humanInputHere2).AsTask().Result;
            Assert.AreEqual(null, tic);
        }
    }
}