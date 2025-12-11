using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class FirmRepo : GenericRepo<Firm>
    {
        public FirmRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
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
        public string GenerateCode(int firmType)
        {
            // 1. Xác định prefix theo FirmType
            string prefix = firmType switch
            {
                2 => "FM",
                3 => "FA",
                _ => "F"
            };

            // 2. Lấy danh sách mã hiện có theo prefix
            var numbers = GetAll(x => x.FirmType == firmType)
                .Select(a =>
                {
                    if (string.IsNullOrWhiteSpace(a.FirmCode)) return 0;

                    // Mã phải bắt đầu bằng prefix
                    if (!a.FirmCode.StartsWith(prefix)) return 0;

                    // Tách phần số phía sau prefix
                    string numberStr = a.FirmCode.Substring(prefix.Length);

                    return int.TryParse(numberStr, out int n) ? n : 0;
                })
                .ToList();

            // 3. Tìm max số hiện có
            int maxNumber = numbers.Any() ? numbers.Max() : 0;

            // 4. Tạo số mới
            int newNumber = maxNumber + 1;

            // 5. Format: FM01, FM02,... FA01,...
            return $"{prefix}{newNumber:D2}";
        }
    }
}
