using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FirmRepo : GenericRepo<Firm>
    {
        public FirmRepo(CurrentUser currentUser) : base(currentUser)
        {
            FirmRepo FirmRepo;
        }
        FirmRepo FirmRepo1;
        public bool CheckFirmCodeExists(string firmCode, int? id = null)
        {
            try
            {

                var query = GetAll(f => (f.FirmCode ?? "").ToUpper() == firmCode.ToUpper() && f.IsDelete != true);


                if (id.HasValue)
                {
                    query = query.Where(f => f.ID != id.Value).ToList();
                }

                return query.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã hãng: {ex.Message}", ex);
            }
        }
    }
}
