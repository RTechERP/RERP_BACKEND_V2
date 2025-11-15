using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCDetailRequestBuyRepo : GenericRepo<PONCCDetailRequestBuy>
    {

        public PONCCDetailRequestBuyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
