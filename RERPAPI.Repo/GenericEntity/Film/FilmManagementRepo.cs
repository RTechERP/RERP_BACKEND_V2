using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Film
{
    public class FilmManagementRepo : GenericRepo<FilmManagement>
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
