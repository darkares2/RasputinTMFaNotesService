using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace RasputinTMFaNotesService.models
{
    public class Note : TableEntity
    {
        public Note(Guid sessionID, Guid userID, string notes)
        {
            this.PartitionKey = "p1";
            this.RowKey = Guid.NewGuid().ToString();
            this.SessionID = sessionID;
            this.UserID = userID;
            this.Notes = notes;
        }
        public Note() { }

        public Guid? SessionID { get; set; }
        public Guid? UserID { get; set; }
        public string Notes { get; set; }

        public Guid NoteID { get { return Guid.Parse(RowKey); } }

        public static explicit operator Note(TableResult v)
        {
            DynamicTableEntity entity = (DynamicTableEntity)v.Result;
            Note SessionProfile = new Note();
            SessionProfile.PartitionKey = entity.PartitionKey;
            SessionProfile.RowKey = entity.RowKey;
            SessionProfile.Timestamp = entity.Timestamp;
            SessionProfile.ETag = entity.ETag;
            SessionProfile.SessionID = entity.Properties["SessionID"].GuidValue;
            if (entity.Properties.ContainsKey("UserID"))
                SessionProfile.UserID = entity.Properties["UserID"].GuidValue;
            SessionProfile.Notes = entity.Properties["Notes"].StringValue;

            return SessionProfile;
        }
    }
}
