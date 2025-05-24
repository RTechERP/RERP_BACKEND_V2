using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
   public class ProductSaleRepo:GenericRepo<ProductSale>
    {
        public async Task<PagedResult<ProductSale>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = table.AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductSale>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

    }
}
