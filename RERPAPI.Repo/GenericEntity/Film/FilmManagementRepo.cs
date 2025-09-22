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
        public int getMAXSTT()
        {
            int maxSTT = GetAll().Max(x => x.STT) ?? 0;
            return maxSTT;
        }
    }
}
