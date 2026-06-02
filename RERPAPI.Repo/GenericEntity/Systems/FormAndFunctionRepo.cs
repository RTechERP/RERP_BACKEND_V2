using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Systems
{
    public class FormAndFunctionRepo : GenericRepo<FormAndFunction>
    {
        public FormAndFunctionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}