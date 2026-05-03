using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Domain.Entities
{

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Equipment { get; set; } = null!; 
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}