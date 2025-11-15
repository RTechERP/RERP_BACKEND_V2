using RERPAPI.Model.Context;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleRepo : GenericRepo<ProductSale>
    {
        public ProductSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
