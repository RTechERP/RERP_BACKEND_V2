using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
namespace RERPAPI.Repo.GenericEntity.TB
{
    public class ProductsRepo : GenericRepo<Product>
    {
        public ProductsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
