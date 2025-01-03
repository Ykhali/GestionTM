using System.Collections.Generic;

namespace GestionTM.Models
{
    public class Validateur
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Consultant> Consultants { get; set; } = new HashSet<Consultant>();
    }
}

