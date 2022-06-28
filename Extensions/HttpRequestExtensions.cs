using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShopifySharp.Infrastructure;

namespace ShopifyAppKyle.Extensions 
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Reads the request stream into a string, while simultaneously ensuring request handlers further down the pipeline can still use the request stream without it being disposed or stuck at the end.
        /// </summary>
        public static async Task<string> ReadRawBodyAsync(this HttpRequest req)
        {
            // Request streams by default can only be read once. This method enables multiple reads for this request.
            req.EnableBuffering();

            var stream = req.Body;
            string rawBody;

            // While the stream can now be rewound, using streamreader will automatically dispose it which we don't want.
            // Instead we need to copy the stream to another temporary stream that can be safely disposed without disposing the request.
            await using (var buffer = new MemoryStream())
            {
                await stream.CopyToAsync(buffer);

                // Move the buffer position back to 0. If we don't do this, 
                // the streamreader will just read an empty string.
                buffer.Position = 0;

                using (var reader = new StreamReader(buffer))
                {
                    rawBody = await reader.ReadToEndAsync();
                }
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return rawBody;
        }

        public static async Task<T> DeserializeBodyAsync<T>(this HttpRequest req, JsonSerializerSettings serializerSettings = null)
        {
            if (serializerSettings == null)
            {
                return Serializer.Deserialize<T>(await req.ReadRawBodyAsync());
            }

            return JsonConvert.DeserializeObject<T>(await req.ReadRawBodyAsync(), serializerSettings);
        }
    }
}
