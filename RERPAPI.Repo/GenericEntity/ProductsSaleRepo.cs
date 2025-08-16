using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductsSaleRepo: GenericRepo<ProductSale>
    {
        RTCContext _context = new RTCContext();
        public async Task<int> CreateAsynC(ProductSale item)
        {
            await _context.ProductSales.AddAsync(item);
            await _context.SaveChangesAsync();
            return item.ID;
        }

    }
}
