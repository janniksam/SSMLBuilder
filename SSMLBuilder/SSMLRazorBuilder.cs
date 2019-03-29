using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Templating;

namespace SSMLBuilder
{
    /// <summary>
    /// Builder to compile and run a SSML razor view
    /// </summary>
    public static class SSMLRazorBuilder
    {
        /// <summary>
        /// Compiles and renders a razor view and returns the rendered SSML string
        /// </summary>
        /// <param name="parsableTemplate">The razor template to be compiled and rendered</param>
        /// <param name="templateKey">The template key, that is used cache the compiled view with</param>
        /// <param name="model">The model you want to give your razor view</param>
        /// <param name="viewBag">The view bag, you want to give your razor view</param>
        /// <returns>The rendered razor view</returns>
        public static string BuildFrom(string parsableTemplate, string templateKey, object model = null, DynamicViewBag viewBag = null)
        {
            var regex = new Regex("^(\\s*@inherits.*)$", RegexOptions.Multiline);
            parsableTemplate = regex.Replace(parsableTemplate, string.Empty);

            var result =
                Engine.Razor.RunCompile(parsableTemplate, templateKey, null, model, viewBag);

            return result;
        }

        /// <summary>
        /// Compiles and renders a razor view and returns the rendered SSML string
        /// </summary>
        /// <param name="parsableTemplateStream">The razor template to be parsed</param>
        /// <param name="templateKey">The template key, that is used cache the compiled view with</param>
        /// <param name="model">The model you want to give your razor view</param>
        /// <param name="viewBag">The view bag, you want to give your razor view</param>
        /// <returns>The rendered razor view</returns>
        public static async Task<string> BuildFromAsync(Stream parsableTemplateStream, string templateKey, object model = null, DynamicViewBag viewBag = null)
        {
            string input;
            using (var sr = new StreamReader(parsableTemplateStream))
            {
                input = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
            
            return BuildFrom(input, templateKey, model, viewBag);
        }
    }
}
