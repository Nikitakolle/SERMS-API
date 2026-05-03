using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }                     
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; }                  
                                                        
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<EventParticipant> EventParticipations { get; set; } = new List<EventParticipant>();
    }

    public enum Role
    {
        Admin,
        Organizer,
        Participant
    }
}