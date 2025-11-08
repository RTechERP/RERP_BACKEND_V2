using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerRepo : GenericRepo<Customer>
    {

        public CustomerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }

}
