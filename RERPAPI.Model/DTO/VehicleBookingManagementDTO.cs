using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class VehicleBookingManagementDTO : VehicleBookingManagement
    {
        //public int ID { get; set; }

        //public int VehicleManagementID { get; set; }

        //public int EmployeeID { get; set; }
        //public int ApprovedTBP { get; set; }

        //public string BookerVehicles { get; set; }

        //public string DepartureAddress { get; set; }

        //public DateTime? DepartureDate { get; set; }

        //public int Category { get; set; }

        //public int Status { get; set; }

        //public string CompanyNameArrives { get; set; }

        //public string Province { get; set; }

        //public string SpecificDestinationAddress { get; set; }

        //public DateTime? TimeNeedPresent { get; set; }

        //public DateTime? TimeReturn { get; set; }

        //public string NameVehicleCharge { get; set; }

        //public string LicensePlate { get; set; }

        //public string DriverName { get; set; }

        //public string DriverPhoneNumber { get; set; }

        //public string CreatedBy { get; set; }

        //public DateTime? CreatedDate { get; set; }

        //public string UpdatedBy { get; set; }

        //public DateTime? UpdatedDate { get; set; }

        //public string PhoneNumber { get; set; }

        //public int PassengerEmployeeID { get; set; }

        //public string PassengerCode { get; set; }

        //public string PassengerName { get; set; }

        //public string PassengerDepartment { get; set; }

        //public string PassengerPhoneNumber { get; set; }

        //public string Note { get; set; }

        //public int ReceiverEmployeeID { get; set; }

        //public string ReceiverCode { get; set; }

        //public string ReceiverName { get; set; }

        //public string ReceiverPhoneNumber { get; set; }

        //public string PackageName { get; set; }

        //public string DeliverName { get; set; }

        //public string DeliverPhoneNumber { get; set; }

        //public bool IsApprovedTBP { get; set; }
        //public string DecilineApprove { get; set; }
        //public string ReasonDeciline { get; set; }

        //public string ProblemArises { get; set; }

        //public bool IsProblemArises { get; set; }

        //public bool IsCancel { get; set; }

        //public bool IsSend { get; set; }

        //public int STT { get; set; }

        //public string DepartureDateText { get; set; }
        public string VehicleInformation { get; set; }
        public string CategoryText { get; set; }
        public string StatusText { get; set; }
        public string ApprovedTBPText { get; set; }
        public DateTime DepartureDateText { get; set; }
        public string DepartureAddressText { get; set; }
        public string ProjectFullName { get; set; }
        public List<VehicleBookingManagement> EmployeeAttaches { get; set; }
        public string VehicleTypeText { get; set; }
    }
}
