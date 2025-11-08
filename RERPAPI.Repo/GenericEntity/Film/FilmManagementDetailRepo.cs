using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Film
{
    public class FilmManagementDetailRepo : GenericRepo<FilmManagementDetail>
    {
        public FilmManagementDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
