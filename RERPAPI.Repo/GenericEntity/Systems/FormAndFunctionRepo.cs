using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class FormAndFunctionRepo : GenericRepo<FormAndFunction>
    {
        public FormAndFunctionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
