using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.TB
{
    public class CategoriesRepo : GenericRepo<Category>
    {
        public CategoriesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}