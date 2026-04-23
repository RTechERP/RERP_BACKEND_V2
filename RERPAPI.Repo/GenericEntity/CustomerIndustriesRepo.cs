using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerIndustriesRepo:GenericRepo<CustomerIndustry>
    {
        public CustomerIndustriesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> GetNextSTTAsync()
        {
            var maxSTT = await table
                .Where(x => x.IsDeleted != true)
                .MaxAsync(x => (int?)x.STT) ?? 0;

            return maxSTT + 1;
        }
    }
}
