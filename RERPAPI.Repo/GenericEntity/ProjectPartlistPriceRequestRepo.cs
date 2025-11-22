using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPriceRequestRepo : GenericRepo<ProjectPartlistPriceRequest>
    {
        public ProjectPartlistPriceRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public async Task SaveData(List<ProjectPartlistPriceRequest> lst)
        {
            foreach (var item in lst)
            {
                if (item.ID > 0)
                {
                    await UpdateAsync(item);
                }
                else
                {
                    await CreateAsync(item);
                }
            }
        }
    }
}
