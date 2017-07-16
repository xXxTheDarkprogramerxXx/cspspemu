﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CSPspEmu.Hle.Formats.Archive;
using CSPspEmu.Resources;
using CSharpUtils;
using CSharpUtils.Extensions;

namespace CSPspEmu.Core.Tests.Hle.Formats.Archive
{
    [TestFixture]
    public class ZipTest
    {
        [Test]
        public void TestUncompressedZip()
        {
            var Zip = new ZipArchive();
            Zip.Load("../../../TestInput/UncompressedZip.zip");
            foreach (var Entry in Zip)
            {
                Console.Error.WriteLine(Entry);
            }
        }

        [Test]
        public void TestCompressedZip()
        {
            var Zip = new ZipArchive();
            Zip.Load("../../../TestInput/UncompressedZip.zip");
            var ExpectedString =
                "ffmpeg -i test.pmf -vcodec copy -an test.h264\nffmpeg -i test.pmf -acodec copy -vn test.ac3p";
            var ResultString = Zip["demux.bat"].OpenUncompressedStream().ReadAllContentsAsString(fromStart: false);
            Assert.AreEqual(ExpectedString, ResultString);
        }

        [Test]
        public void TestZip2()
        {
            var Zip = new ZipArchive();
            Zip.Load(ResourceArchive.GetFlash0ZipFileStream());
            foreach (var Entry in Zip)
            {
                Console.Error.WriteLine(Entry);
            }
        }
    }
}