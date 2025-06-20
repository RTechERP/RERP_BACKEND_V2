using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class CurrentUser
    {
        public int EmployeeID { get; set; }
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public string Code { get; set; } = "";
        public string FullName { get; set; } = "";
        public string LoginName { get; set; } = "";
        public bool IsAdmin { get; set; }
        public int IsAdminSale { get; set; }
        public int MainViewID { get; set; }
        public string DepartmentName { get; set; } = "";
        public string HeadofDepartment { get; set; } = "";
        public string AnhCBNV { get; set; } = "";
        public string StatusEmployee { get; set; } = "";
        public string StatusUser { get; set; } = "";
        public string PositionName { get; set; } = "";
        public int UserGroupID { get; set; }
        public int PositionID { get; set; }
        public int GioiTinh { get; set; }
        public string PositionCode { get; set; } = "";
        public string DepartmentCode { get; set; } = "";
        public bool IsBusinessCost { get; set; }
        public int IsLeader { get; set; }
        public int TeamOfUser { get; set; }
        public string Permissions { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
