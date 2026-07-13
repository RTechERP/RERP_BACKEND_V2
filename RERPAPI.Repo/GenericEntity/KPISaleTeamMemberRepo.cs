using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class KPISaleTeamMemberRepo : GenericRepo<KPISaleTeamMember>
    {
        public KPISaleTeamMemberRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}