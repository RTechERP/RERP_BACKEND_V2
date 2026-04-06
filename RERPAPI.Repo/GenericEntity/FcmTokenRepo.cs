using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RERPAPI.Repo.GenericEntity
{
    public class FcmTokenRepo : GenericRepo<FcmToken>
    {
        public FcmTokenRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Lấy danh sách FCM token theo employeeID để gửi push notification
        /// </summary>
        public List<string> GetTokensByEmployeeID(int employeeID)
        {
              return table
                .Where(t => t.EmployeeID == employeeID && !string.IsNullOrEmpty(t.Token))
                .Select(t => t.Token!)
                .Distinct()
                .ToList();
        }
    }
}
