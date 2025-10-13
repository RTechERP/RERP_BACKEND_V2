using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupRepo:GenericRepo<ProductGroup>
    {
        RTCContext _context = new RTCContext();
        public async Task<int> CreateAsynC(ProductGroup item)
        {
            await _context.ProductGroups.AddAsync(item);
            await _context.SaveChangesAsync();
            return item.ID;
        }
    }
}
