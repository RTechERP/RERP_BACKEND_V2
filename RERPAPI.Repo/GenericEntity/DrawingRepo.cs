using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class DrawingRepo : GenericRepo<Drawing>
    {
        public DrawingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}