using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TrainingRegistrationRepo : GenericRepo<TrainingRegistration>
    {
        public string GetNewCode(TrainingRegistration model)
        {
            string prefix = "DT";
            string datePart = model.CreatedDate?.ToString("yyyyMMdd") ?? DateTime.Now.ToString("yyyyMMdd");
            string fullPrefix = $"{prefix}.{datePart}"; // Ví dụ: DT20251013

            // Lấy bản ghi có cùng ngày tạo
            var lastRecord = GetAll(x => x.Code != null && x.Code.StartsWith(fullPrefix))
                             .OrderByDescending(x => x.Code)
                             .FirstOrDefault();

            int newNumber = 1;
            if (lastRecord != null && !string.IsNullOrEmpty(lastRecord.Code))
            {
                // Lấy phần số sau dấu chấm
                string lastNumberStr = lastRecord.Code.Substring(lastRecord.Code.LastIndexOf('.') + 1);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    newNumber = lastNumber + 1;
                }
            }

            // Chỉ có 1 dấu chấm trước phần số
            string newCode = $"{fullPrefix}.{newNumber:D3}";
            return newCode;
        }

    }
}
