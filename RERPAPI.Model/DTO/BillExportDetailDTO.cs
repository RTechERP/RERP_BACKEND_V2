﻿using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillExportDetailDTO: BillExportDetail
    {
        public List<int>? InventoryProjectIDs { get; set; }
    }
}
