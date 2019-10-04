using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace espriapi.guldur
{
    public static class HttpTriggerCSharp
    {
        [FunctionName("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string method = req.Query["method"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            method = method ?? data?.method;

            if (method != null && method == "guldur")
            {
                using (FileStream fs = File.OpenRead(context.FunctionAppDirectory + "/espriler.json"))
                {
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        var esprilerData = JsonConvert.DeserializeObject<List<Joke>>(temp.GetString(b));
                        var rndCount = new Random();
                        var rndIndex = rndCount.Next(0, esprilerData.Count);
                        return (ActionResult)new JsonResult(esprilerData[rndIndex]);
                    }
                }

            }else
            {
                return new BadRequestObjectResult("method parametresini guldur olarak doldurun");
            }

            return new BadRequestObjectResult("Kim bilir ne hatası");
            
        }
    }
}
