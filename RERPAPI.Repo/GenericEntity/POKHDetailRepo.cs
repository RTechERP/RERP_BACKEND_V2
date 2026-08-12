using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHDetailRepo : GenericRepo<POKHDetail>
    {
        public POKHDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool CheckSpecialCode(string productCode, string specialCode)
        {
            var data = SQLHelper<dynamic>.ProcedureToList(
                "spCheckSpecialCode",
                new string[] { "@ProductCode", "@SpecialCode" },
                new object[] { productCode, specialCode }
            );

            if (data == null || data.Count == 0)
                return false;

            var result = data[0];

            if (result == null || result.Count == 0)
                return false;

            return Convert.ToBoolean(result[0].IsDuplicate);
        }
    }
}