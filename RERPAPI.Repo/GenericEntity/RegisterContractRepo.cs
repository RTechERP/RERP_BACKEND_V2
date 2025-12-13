using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterContractRepo : GenericRepo<RegisterContract>
    {
        public RegisterContractRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
