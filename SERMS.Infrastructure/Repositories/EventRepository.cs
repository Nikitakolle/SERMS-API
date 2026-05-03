using Microsoft.EntityFrameworkCore;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;
using SERMS.Infrastructure.Context;
using SERMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERMS.Infrastructure.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Event>> GetEventByRoomIdAsync (int roomId)
        {
            return await _context.Events
                 .Where(e => e.RoomId == roomId)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventByOrganizerIdAsync(int organizedId)
        {
            return await _context.Events
                .Where(e => e.OrganizerId == organizedId)
                .ToListAsync();
        }
        public async Task<List<Event>> GetConflictingEventsAsync(int roomId, DateTime start, DateTime end)
        {
            return await _context.Events
                .Where(e => e.RoomId == roomId &&
                            start < e.EndTime &&
                            end > e.StartTime)
                .ToListAsync();
        }
    }
}
