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
                var esprilerData = JsonConvert.DeserializeObject<List<Joke>>(File.ReadAllText(context.FunctionAppDirectory + "/espriler.json"));
                var rndCount = new Random();
                var rndIndex = rndCount.Next(0, esprilerData.Count);
                return (ActionResult)new JsonResult(esprilerData[rndIndex]);
            }
            else
            {
                return new BadRequestObjectResult("method parametresini guldur olarak doldurun");
            }


        }
    }
}
