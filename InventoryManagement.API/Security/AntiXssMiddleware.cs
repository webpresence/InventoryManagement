using Ganss.Xss;
using System.Text;

namespace InventoryManagement.API.Security
{
    /// <summary>
    /// Middleware to sanitize incoming requests to prevent Cross-Site Scripting (XSS) attacks.
    /// </summary>
    public class AntiXssMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntiXssMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public AntiXssMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware operation to sanitize the request body.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            // Enable buffering to allow the request body to be read by model binders later.
            httpContext.Request.EnableBuffering();

            // Use StreamReader with leaveOpen: true to leave the stream open after disposing.
            using (var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var rawContent = await streamReader.ReadToEndAsync();
                var sanitizer = new HtmlSanitizer();
                var sanitizedContent = sanitizer.Sanitize(rawContent);

                if (rawContent != sanitizedContent)
                {
                    throw new BadHttpRequestException("XSS injection detected from middleware.");
                }
            }

            // Rewind the stream for the next middleware in the pipeline.
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);

            await _next.Invoke(httpContext);
        }
    }
}
