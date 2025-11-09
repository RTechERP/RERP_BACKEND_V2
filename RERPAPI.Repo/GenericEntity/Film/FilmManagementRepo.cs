using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Film
{
    public class FilmManagementRepo:GenericRepo<FilmManagement>
    {
        public FilmManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public int getMAXSTT()
        {
            int maxSTT = GetAll().Max(x => x.STT) ?? 0;
            return maxSTT;
        }
    }
}
