using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CongnitveManager
{
    public class CelebrityService
    {
        public ContentResult MakeAnalysisCelebrity(Stream image)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b58ed4d995d84869b57e55755f6604ea");

            string uri = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?details=celebrities&visualFeatures=Categories,Description,Color";

            HttpResponseMessage response;

            var buffer = GetImageAsByteArray(image);

            using (var content = new ByteArrayContent(buffer))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = client.PostAsync(uri, content).Result;

                string contentString = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<RootCelebrityObject>(contentString);
                return new ContentResult { celebrity = result.categories.FirstOrDefault().detail.celebrities.FirstOrDefault(), tags = result.description.tags };
            }
        }
         byte[] GetImageAsByteArray(Stream image)
        {
            BinaryReader binaryReader = new BinaryReader(image);
            return binaryReader.ReadBytes((int)image.Length);
        }

        public string GetCelebrity(Celebrity celebrity)
        {
            if (celebrity != null)
            {
                return celebrity.name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
