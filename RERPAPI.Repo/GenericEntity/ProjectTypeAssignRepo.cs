using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeAssignRepo : GenericRepo<ProjectTypeAssign>
    {

        public ProjectTypeAssignRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
