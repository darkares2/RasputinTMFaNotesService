using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using RasputinTMFaNotesService;
using RasputinTMFaNotesService.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RasputinTMFaNoteServiceTests
{
    public class GetNoteTests
    {

        private Stream Serialize(object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            return new MemoryStream(Encoding.Default.GetBytes(jsonString));
        }

        [Fact]
        public async Task GetNoteBySessionID()
        {
            //Arrange
            var context = new DefaultHttpContext();
            var request = context.Request;
            Guid userID = Guid.NewGuid();
            var qs = new Dictionary<string, StringValues>
            {
                { "SessionID", userID.ToString() }
            };
            request.Query = new QueryCollection(qs);
            var iLoggerMock = new Mock<ILogger>();
            var tblNoteMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            Note session1 = new Note() { RowKey = Guid.NewGuid().ToString(), SessionID = Guid.NewGuid(), Notes = "This is a note" };
            Note session2 = new Note() { RowKey = Guid.NewGuid().ToString(), SessionID = Guid.NewGuid(), Notes = "More notes" };
            List<Note> sessions = new List<Note>() { session1, session2 };
            var resultMock = new Mock<TableQuerySegment<Note>>(sessions);
            tblNoteMock.Setup(_ => _.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<Note>>(), It.IsAny<TableContinuationToken>())).ReturnsAsync(resultMock.Object);

            // Act
            OkObjectResult result = (OkObjectResult)await GetNote.Run(request, tblNoteMock.Object , iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Note[] sessionResult = (Note[])JsonConvert.DeserializeObject((string)result.Value, typeof(Note[]));
            Assert.Equal(2, sessionResult.Length);
            Assert.Equal(session1.SessionID, sessionResult[0].SessionID);
            Assert.Equal(session1.Notes, sessionResult[0].Notes);
            Assert.Equal(session2.SessionID, sessionResult[1].SessionID);
            Assert.Equal(session2.Notes, sessionResult[1].Notes);
        }

        [Fact]
        public async Task GetNoteBySessionIDNotClosed()
        {
            //Arrange
            var context = new DefaultHttpContext();
            var request = context.Request;
            Guid userID = Guid.NewGuid();
            var qs = new Dictionary<string, StringValues>
            {
                { "SessionID", userID.ToString() }
            };
            request.Query = new QueryCollection(qs);
            var iLoggerMock = new Mock<ILogger>();
            var tblNoteMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            Note session1 = new Note() { RowKey = Guid.NewGuid().ToString(), SessionID = Guid.NewGuid(), Notes = "Some notes" };
            Note session2 = new Note() { RowKey = Guid.NewGuid().ToString(), SessionID = Guid.NewGuid(), Notes = "Some more notes" };
            Note session3 = new Note() { RowKey = Guid.NewGuid().ToString(), SessionID = Guid.NewGuid(), Notes = "Even more notes" };
            List<Note> sessions = new List<Note>() { session1, session2 };
            var resultMock = new Mock<TableQuerySegment<Note>>(sessions);
            tblNoteMock.Setup(_ => _.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery<Note>>(), It.IsAny<TableContinuationToken>())).ReturnsAsync(resultMock.Object);

            // Act
            OkObjectResult result = (OkObjectResult)await GetNote.Run(request, tblNoteMock.Object, iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Note[] sessionResult = (Note[])JsonConvert.DeserializeObject((string)result.Value, typeof(Note[]));
            Assert.Equal(2, sessionResult.Length);
            Assert.Equal(session1.SessionID, sessionResult[0].SessionID);
            Assert.Equal(session1.Notes, sessionResult[0].Notes);
            Assert.Equal(session2.SessionID, sessionResult[1].SessionID);
            Assert.Equal(session2.Notes, sessionResult[1].Notes);
        }
    }
}
