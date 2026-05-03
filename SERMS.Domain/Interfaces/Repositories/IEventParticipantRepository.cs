using SERMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Domain.Interfaces.Repositories
{
    public interface IEventParticipantRepository
    {
        IQueryable<EventParticipant> GetQueryable();
        Task AddAsync(EventParticipant entity);
        Task<bool> ExistsAsync(int eventId, int userId);
        Task<IEnumerable<EventParticipant>> GetByEventIdAsync(int eventId);
        Task<IEnumerable<EventParticipant>> GetByUserIdAsync(int userId);
    }
}
