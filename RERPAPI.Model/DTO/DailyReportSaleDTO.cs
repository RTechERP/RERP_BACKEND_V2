using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class DailyReportSaleDTO
    {
        public int ID { get; set; }

        public int projectId { get; set; }

        public int customerId { get; set; }

        public int? warehouseId { get; set; } = 0;

        public int projectStatusBaseId { get; set; }

        public int userId { get; set; }

        public DateTime dateStart { get; set; }

        public DateTime? dateEnd { get; set; }

        public int firmId { get; set; }

        public int projectTypeId { get; set; }

        public int contactId { get; set; }

        public int groupTypeId { get; set; }

        public int? partId { get; set; }

        public bool bigAccount { get; set; }

        public bool saleOpportunity { get; set; }

        public string content { get; set; }

        public string result { get; set; }

        public string problemBacklog { get; set; }

        public string planNext { get; set; }

        public string productOfCustomer { get; set; }


        //dành cho updateProject 
        public int? projectStatusOld { get; set; } = 0;
        public int? employeeId { get; set; } = 0;
        public DateTime dateStatusLog { get; set; }
    }
}
