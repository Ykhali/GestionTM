using System.Collections.Generic;

namespace GestionTM.Models
{
    public class Timesheet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Submitted, Accepted, Refused

        public int ConsultantId { get; set; }
        public Consultant Consultant { get; set; }
        public ICollection<TimesheetLine> TimesheetLines { get; set; }
    }
}

