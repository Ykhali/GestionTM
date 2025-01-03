using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionTM.Models
{
    public class Consultant
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        public int? ValidateurId { get; set; }
        public Validateur Validateur { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
    }
}

