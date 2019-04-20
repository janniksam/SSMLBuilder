using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSMLBuilder;
using SSMLVerifier;

namespace SSMLBuilderTests
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public async Task ShouldParseCorrectlyString()
        {
            var input = "Hello @Model, welcome to RazorEngine!";
            var result = await SSMLRazorBuilder.BuildFromAsync(input, "templateKey1", "Bla");
            Assert.AreEqual("Hello Bla, welcome to RazorEngine!", result);
        }

        [TestMethod]
        public async Task ShouldParseCorrectlyStream()
        {
            var input = "Hello @Model, welcome to RazorEngine!";
            var byteArray = Encoding.ASCII.GetBytes(input);
            using (var ms = new MemoryStream(byteArray))
            {
                var result = await SSMLRazorBuilder.BuildFromAsync(ms, "templateKey2", "Testuser");
                Assert.AreEqual("Hello Testuser, welcome to RazorEngine!", result);
            }
        }

        [TestMethod]
        public async Task ReturnsValidSsml()
        {
            var templateKey = "SSMLBuilderTests.SSMLViews.TestView.cshtml";
            var assembly = GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream(templateKey);
            var ssmlResult = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new Game { PlayerAmount = 5 });

            var verifier = new Verifier();
            var verificationResult = verifier.Verify(ssmlResult);
            Assert.AreEqual(0, verificationResult.Count());
        }

        [TestMethod]
        public async Task IsPerformanceGoodWith20000Calls()
        {
            var sw = new Stopwatch();
            sw.Start();

            var tasks = new List<Task>();
            for (var i = 0; i < 20000; i++)
            {
                var templateKey = "SSMLBuilderTests.SSMLViews.TestView.cshtml";
                var assembly = GetType().GetTypeInfo().Assembly;
                var resource = assembly.GetManifestResourceStream(templateKey);
                Task task = SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new Game { PlayerAmount = i });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 15000);
        }

        [TestMethod]
        public async Task CachingWorks()
        {
            var sw = new Stopwatch();
            sw.Start();

            var templateKey = "SSMLBuilderTests.SSMLViews.TestView2.cshtml";
            var assembly = GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream(templateKey);
            var result = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new Game { PlayerAmount = 4 });
            result = result.Trim();
            Assert.AreEqual("1", result.Trim());

            resource = assembly.GetManifestResourceStream(templateKey);
            result = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new Game { PlayerAmount = 6 });
            result = result.Trim();
            Assert.AreEqual("2", result);

            resource = assembly.GetManifestResourceStream(templateKey);
            result = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new Game { PlayerAmount = 4 });
            result = result.Trim();
            Assert.AreEqual("1", result);


            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 15000);
        }
    }
}
