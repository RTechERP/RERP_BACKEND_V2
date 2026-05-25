using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RERPAPI.Repo.GenericEntity
{
    public class FcmTokenRepo : GenericRepo<FcmToken>
    {
        NotificationTypeLinkRepo notificationTypeLinkRepo;
        public FcmTokenRepo(CurrentUser currentUser, NotificationTypeLinkRepo notificationTypeLinkRepo) : base(currentUser)
        {
            this.notificationTypeLinkRepo = notificationTypeLinkRepo;
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
        public bool checkNotiUser(int notiID, int user)
        {
            var notiLink = notificationTypeLinkRepo.GetAll(t => t.NotificationTypeID == notiID && t.UserID == user).FirstOrDefault();
            if (notiLink == null)
            {
                return false;
            }
            return notiLink.IsSelected ?? false;
        }
    }
}
