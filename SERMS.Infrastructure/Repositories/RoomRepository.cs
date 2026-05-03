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
    public class RoomRepository: GenericRepository<Room>, IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

