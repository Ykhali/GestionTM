using System;

namespace GestionTM.Models
{
    public class TimesheetLine
    {
        public int Id { get; set; }
        public string ConsultantName { get; set; }
        public string ValidateurName { get; set; }
        public DateTime Date { get; set; }
        public int HoursWorked { get; set; } // 1 to 8 hours

        public int TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }
    }
}

