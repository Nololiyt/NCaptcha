using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.TicketFactories.InMemoryGuidDictionary.Tests
{
    [TestClass()]
    public class GuidDictionaryTicketFactoryTests
    {
        [TestMethod()]
        public void GuidDictionaryTicketFactoryTest()
        {
            using var saver = new GuidDictionaryTicketFactory(new TimeSpan(0, 0, 0, 1));
            Assert.AreEqual(new TimeSpan(0, 0, 0, 1), saver.TicketsLifeTime);

            var abc = saver.GenerateNewAsync().AsTask().Result;
            var def = saver.GenerateNewAsync().AsTask().Result;
            var ghi = saver.GenerateNewAsync().AsTask().Result;

            Assert.AreEqual(true, saver.VerifyAsync(abc).AsTask().Result);

            Assert.AreEqual(true, saver.VerifyAsync(def).AsTask().Result);
            Assert.AreEqual(false, saver.VerifyAsync(def).AsTask().Result);

            Thread.Sleep(new TimeSpan(0, 0, 0, 1));
            Assert.AreEqual(false, saver.VerifyAsync(ghi).AsTask().Result);

            using var saver2 = new GuidDictionaryTicketFactory(new TimeSpan(0, 0, 10, 00));
            List<Task<string>> tasks = new List<Task<string>>();
            for (int i = 0; i < 10000; i++)
            {
                int i2 = i;
                tasks.Add(Task.Run(() =>
                {
                    return saver2.GenerateNewAsync().AsTask().Result;
                }));
            }
            Task.WaitAll(tasks.ToArray());

            List<Task> getTasks = new List<Task>();
            for (int i = 0; i < 10000; i++)
            {
                int i2 = i;
                getTasks.Add(Task.Run(() =>
                {
                    var r = saver2.VerifyAsync(tasks[i2].Result).AsTask().Result;
                    Assert.AreEqual(true, r);
                }));
            }
            Task.WaitAll(getTasks.ToArray());

            List<Task> getTasks2 = new List<Task>();
            for (int i = 0; i < 10000; i++)
            {
                int i2 = i;
                getTasks2.Add(Task.Run(() =>
                {
                    var r = saver2.VerifyAsync(tasks[i2].Result).AsTask().Result;
                    Assert.AreEqual(false, r);
                }));
            }
            Task.WaitAll(getTasks2.ToArray());
        }
    }
}