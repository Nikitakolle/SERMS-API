using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Domain.Entities
{
   
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

       
        public int RoomId { get; set; }
        public Room? Room { get; set; }   

        public int OrganizerId { get; set; }
        public User? Organizer { get; set; } 

       
        public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
    }
}
