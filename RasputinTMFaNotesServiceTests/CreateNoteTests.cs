using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RasputinTMFaNotesService;
using RasputinTMFaNotesService.models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RasputinTMFaNoteServiceTests
{
    public class CreateNoteTests
    {

        private Stream Serialize(object value)
        {
            var jsonString = JsonConvert.SerializeObject(value);
            return new MemoryStream(Encoding.Default.GetBytes(jsonString));
        }

        [Fact]
        public async Task CreateNoteNew()
        {
            //Arrange
            CreateNoteRequest createNoteRequest = new CreateNoteRequest() { SessionID = Guid.NewGuid(), Notes = "Some new notes" };
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = Serialize(createNoteRequest);
            var iLoggerMock = new Mock<ILogger>();
            var tblNoteMock = new Mock<CloudTable>(new Uri("https://fake.com"), null);
            TableOperation operation = null;
            tblNoteMock.Setup(_ => _.ExecuteAsync(It.IsAny<TableOperation>()))
                    .Callback<TableOperation>((obj) => operation = obj);

            // Act
            OkObjectResult result = (OkObjectResult)await CreateNote.Run(request, tblNoteMock.Object , iLoggerMock.Object);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Note sessionResult = (Note)JsonConvert.DeserializeObject((string)result.Value, typeof(Note));
            Assert.Equal(createNoteRequest.SessionID, sessionResult.SessionID);
            tblNoteMock.Verify(_ => _.ExecuteAsync(It.IsAny<TableOperation>()), Times.Exactly(1));
            Assert.NotNull(operation.Entity);
            Assert.Equal(createNoteRequest.SessionID, ((Note)operation.Entity).SessionID);
            Assert.Equal(createNoteRequest.Notes, ((Note)operation.Entity).Notes);
        }
    }
}
