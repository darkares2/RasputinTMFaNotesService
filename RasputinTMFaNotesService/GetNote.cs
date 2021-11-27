using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using RasputinTMFaNotesService.models;

namespace RasputinTMFaNotesService
{
    public static class GetNote
    {
        [FunctionName("GetNote")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("tblNotes")] CloudTable tblNote,
            ILogger log)
        {
            log.LogInformation("GetNote called.");

            string responseMessage = null;
            string sessionIDString = req.Query["SessionID"];
            if (sessionIDString != null && !sessionIDString.Equals(""))
            {
                Note[] sessions = await NoteService.FindSessionNotes(log, tblNote, Guid.Parse(sessionIDString));
                responseMessage = JsonConvert.SerializeObject(sessions);

            }

            return new OkObjectResult(responseMessage);
        }
    }
}
