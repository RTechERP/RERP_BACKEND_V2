using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerPartsRepo : GenericRepo<CustomerPart>
    {
        public CustomerPartsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
