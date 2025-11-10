using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CurrencyRepo : GenericRepo<Currency>
    {
        public CurrencyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool CheckExist(Currency c)
        {
            var exist = GetAll(x => x.Code.ToUpper() == c.Code.ToUpper() && x.IsDeleted == false && x.ID != c.ID);
            if (exist.Any())
            {
                return true;
            }
            return false;
        }
    }
}
