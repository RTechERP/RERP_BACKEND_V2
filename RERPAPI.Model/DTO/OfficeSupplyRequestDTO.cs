    using RERPAPI.Model.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace RERPAPI.Model.DTO
    {
        public class OfficeSupplyRequestDTO
        {
            public OfficeSupplyRequest1? officeSupplyRequest { get; set; }
            public List<OfficeSupplyRequestsDetail>? officeSupplyRequestsDetails { get; set; }
    }
    }
