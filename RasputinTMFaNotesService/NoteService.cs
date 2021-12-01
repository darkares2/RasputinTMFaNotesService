using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using RasputinTMFaNotesService.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RasputinTMFaNotesService
{
    public static class NoteService
    {
        public static async Task<Note> InsertNote(ILogger log, CloudTable tblNote, Guid sessionID, Guid userID, string notes)
        {
            Note session = new Note(sessionID, userID, notes);
            TableOperation operation = TableOperation.Insert(session);
            await tblNote.ExecuteAsync(operation);
            return session;
        }

        public static async Task<Note[]> FindSessionNotes(ILogger log, CloudTable tblNote, Guid sessionID)
        {
            log.LogInformation($"FindSessionNotes by user {sessionID}");
            List<Note> result = new List<Note>();
            TableQuery<Note> query = new TableQuery<Note>().Where(TableQuery.GenerateFilterConditionForGuid("SessionID", QueryComparisons.Equal, sessionID));
            TableContinuationToken continuationToken = null;
            try
            {
                do
                {
                    var page = await tblNote.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = page.ContinuationToken;
                    result.AddRange(page.Results);
                } while (continuationToken != null);
                return result.OrderBy( _ => _.Timestamp).ToArray();
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "FindSessionNotes");
                return null;
            }
        }
    }
}
