using System.Collections.Generic;

namespace RERPAPI.Model.DTO
{
    public class PermissionSaveItem
    {
        public int ID { get; set; }
        public bool OldValue { get; set; }
        public bool IsChecked { get; set; }
        public int UserGroupRightDistributionID { get; set; }
    }

    public class SaveGroupPermissionRequest
    {
        public int UserGroupID { get; set; }
        public List<PermissionSaveItem> Permissions { get; set; } = new List<PermissionSaveItem>();
    }
}
