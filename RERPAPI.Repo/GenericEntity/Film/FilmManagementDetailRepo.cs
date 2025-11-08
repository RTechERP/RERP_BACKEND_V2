using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Film
{
    public class FilmManagementDetailRepo : GenericRepo<FilmManagementDetail>
    {
        public FilmManagementDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
