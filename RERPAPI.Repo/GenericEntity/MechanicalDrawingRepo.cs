using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class MechanicalDrawingRepo : GenericRepo<MechanicalDrawing>
    {
        public MechanicalDrawingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}