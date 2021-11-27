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
    public static class CreateNote
    {
        [FunctionName("CreateNote")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("tblNotes")] CloudTable tblNote,
            ILogger log)
        {
            log.LogInformation("CreateNote called.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateNoteRequest data = (CreateNoteRequest)JsonConvert.DeserializeObject(requestBody, typeof(CreateNoteRequest));

            Note session = await NoteService.InsertNote(log, tblNote, data.SessionID, data.Notes);

            string responseMessage = JsonConvert.SerializeObject(session);
            return new OkObjectResult(responseMessage);
        }
    }
}
