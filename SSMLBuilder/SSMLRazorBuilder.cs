using System.IO;
using System.Threading.Tasks;
using RazorLight;

namespace SSMLBuilder
{
    /// <summary>
    /// Builder to compile and run a SSML razor view
    /// </summary>
    public static class SSMLRazorBuilder
    {
        private static RazorLightEngine m_engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();

        /// <summary>
        /// Compiles and renders a razor view and returns the rendered SSML string
        /// </summary>
        /// <param name="parsableTemplate">The razor template to be compiled and rendered</param>
        /// <param name="templateKey">The template key, that is used cache the compiled view with</param>
        /// <param name="model">The model you want to give your razor view</param>
        /// <returns>The rendered razor view</returns>
        public static async Task<string> BuildFromAsync(string parsableTemplate, string templateKey, object model = null)
        {
            var result = await m_engine.CompileRenderAsync(templateKey, parsableTemplate, model)
                .ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Compiles and renders a razor view and returns the rendered SSML string
        /// </summary>
        /// <param name="parsableTemplateStream">The razor template to be parsed</param>
        /// <param name="templateKey">The template key, that is used cache the compiled view with</param>
        /// <param name="model">The model you want to give your razor view</param>
        /// <returns>The rendered razor view</returns>
        public static async Task<string> BuildFromAsync(Stream parsableTemplateStream, string templateKey, object model = null)
        {
            string input;
            using (var sr = new StreamReader(parsableTemplateStream))
            {
                input = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
            
            return await BuildFromAsync(input, templateKey, model).ConfigureAwait(false);
        }
    }
}
