using System;
using System.Collections.Generic;
using System.Text;

namespace RasputinTMFaNotesService.models
{
    public class CreateNoteRequest
    {
        public Guid SessionID { get; set; }
        public Guid UserID { get; set; }
        public string Notes { get; set; }
    }
}
