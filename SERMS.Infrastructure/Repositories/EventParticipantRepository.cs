using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SERMS.Domain;
using Microsoft.EntityFrameworkCore;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;
using SERMS.Infrastructure.Context;

namespace SERMS.Infrastructure.Repositories
{
    public class EventParticipantRepository :IEventParticipantRepository
    {
        private readonly AppDbContext _context;
        public EventParticipantRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EventParticipant entity)
        {
            await _context.EventParticipants.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int eventId, int userId)
        {
            return await _context.EventParticipants
                .AnyAsync(x => x.EventId == eventId && x.UserId == userId);
        }


        public async Task<IEnumerable<EventParticipant>> GetByEventIdAsync(int eventId)
        {
            return await _context.EventParticipants
                .Where(x => x.EventId == eventId)
                .ToListAsync();
        }
        public async Task<IEnumerable<EventParticipant>> GetByUserIdAsync(int userId)
        {
            return await _context.EventParticipants
                .Where(x => x.UserId == userId)
                .ToListAsync();

        }
         public IQueryable<EventParticipant> GetQueryable()
        {
            return _context.Set<EventParticipant>().AsQueryable();
        }
    
    }
}
