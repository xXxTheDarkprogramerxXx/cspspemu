﻿using System;
using System.Threading;
using CSharpUtils;
using NUnit.Framework;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CSharpUtilsTests
{
    [TestFixture]
    public class AsyncTaskTest
    {
        [Test]
        public void SimpleTest()
        {
            var Result = new AsyncTask<String>(delegate() { return "Hello"; });
            Assert.AreEqual("Hello", Result.Result);
        }

        [Test]
        public void Complex1Test()
        {
            var Result = new AsyncTask<String>(delegate()
            {
                Thread.Yield();
                return "Test";
            });
            Assert.IsFalse(Result.Ready);
            Thread.Sleep(2);
            Assert.IsTrue(Result.Ready);
        }

        [Test]
        public void Complex2Test()
        {
            var Result = new AsyncTask<String>(delegate()
            {
                Thread.Yield();
                return "Test";
            });
            Assert.AreEqual("Test", Result.Result);
            Assert.IsTrue(Result.Ready);
        }
    }
}