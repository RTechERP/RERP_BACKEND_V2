using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PinResetTokenRepo : GenericRepo<PinResetToken>
    {
        public PinResetTokenRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
