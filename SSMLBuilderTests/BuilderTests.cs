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
        public void ShouldParseCorrectlyString()
        {
            var input = "Hello @Model.Name, welcome to RazorEngine!";
            var result = SSMLRazorBuilder.BuildFrom(input, "templateKey1", new {Name = "Bla"});
            Assert.AreEqual("Hello Bla, welcome to RazorEngine!", result);
        }

        [TestMethod]
        public async Task ShouldParseCorrectlyStream()
        {
            var input = "Hello @Model.Name, welcome to RazorEngine!";
            var byteArray = Encoding.ASCII.GetBytes(input);
            using (var ms = new MemoryStream(byteArray))
            {
                var result = await SSMLRazorBuilder.BuildFromAsync(ms, "templateKey2", new {Name = "Bla"});
                Assert.AreEqual("Hello Bla, welcome to RazorEngine!", result);
            }
        }

        [TestMethod]
        public async Task ReturnsValidSsml()
        {
            var templateKey = "SSMLBuilderTests.SSMLViews.TestView.cshtml";
            var assembly = GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream(templateKey);
            var ssmlResult = await SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new { PlayerAmount = 5 });

            var verifier = new Verifier();
            var verificationResult = verifier.Verify(ssmlResult);
            Assert.AreEqual(VerificationState.Valid, verificationResult.State);
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
                Task task = SSMLRazorBuilder.BuildFromAsync(resource, templateKey, new {PlayerAmount = 5});
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 15000);
        }
    }
}
