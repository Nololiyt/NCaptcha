using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nololiyt.Captcha.AnswerSavers.InMemoryGuidDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.AnswerSavers.InMemoryGuidDictionary.Tests
{
    [TestClass()]
    public class GuidDictionaryStringAnswerSaverTests
    {
        [TestMethod()]
        public void GuidDictionaryStringAnswerSaverTest()
        {
            using var saver = new GuidDictionaryStringAnswerSaver(new TimeSpan(0, 0, 0, 1));
            Assert.AreEqual(new TimeSpan(0, 0, 0, 1), saver.AnswersLifeTime);

            var abc = saver.SaveAsync("abc").AsTask().Result;
            var def = saver.SaveAsync("def").AsTask().Result;
            var ghi = saver.SaveAsync("ghi").AsTask().Result;

            Assert.AreEqual("abc", saver.TryGetAndRemoveAsync(abc).AsTask().Result);

            Assert.AreEqual("def", saver.TryGetAndRemoveAsync(def).AsTask().Result);
            Assert.AreEqual(null, saver.TryGetAndRemoveAsync(def).AsTask().Result);

            Thread.Sleep(new TimeSpan(0, 0, 0, 1));
            Assert.AreEqual(null, saver.TryGetAndRemoveAsync(ghi).AsTask().Result);

            using var saver2 = new GuidDictionaryStringAnswerSaver(new TimeSpan(0, 0, 10, 00));
            List<Task<string>> tasks = new List<Task<string>>();
            for (int i = 0; i < 10000; i++)
            {
                int i2 = i;
                tasks.Add(Task.Run(() => {
                    return saver2.SaveAsync(i2.ToString()).AsTask().Result;
                }));
            }
            Task.WaitAll(tasks.ToArray());

            List<Task> getTasks = new List<Task>();
            for (int i = 0; i < 10000; i++)
            {
                int i2 = i;
                getTasks.Add(Task.Run(() => {
                    var r = saver2.TryGetAndRemoveAsync(tasks[i2].Result).AsTask().Result;
                    Assert.AreEqual(i2.ToString(), r);
                }));
            }
            Task.WaitAll(getTasks.ToArray());
        }
    }
}