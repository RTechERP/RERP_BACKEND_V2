using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class CommercialPriceRequestRepo : GenericRepo<CommercialPriceRequest>
    {
        public CommercialPriceRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
