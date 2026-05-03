using SERMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Domain.Interfaces.Repositories
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<IEnumerable<Event>> GetEventByRoomIdAsync(int roomId);
        Task<IEnumerable<Event>> GetEventByOrganizerIdAsync(int OrganizerId);
        Task<List<Event>> GetConflictingEventsAsync(int roomId, DateTime start, DateTime end);
    }
}
