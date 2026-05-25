using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSRatingTicketRepo : GenericRepo<FiveSRatingTicket>
    {
        public FiveSRatingTicketRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<List<FiveSRatingTicket>> GetBySessionIDAsync(int sessionId)
        {
            return await table
                .Where(x => x.Rating5SID == sessionId && x.IsDeleted != true)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<string> GenerateTicketCodeAsync(int sessionId)
        {
            int count = await table.Where(x => x.Rating5SID == sessionId).CountAsync();
            return $"P_{sessionId:D4}_{count + 1:D3}";
        }
    }
}
