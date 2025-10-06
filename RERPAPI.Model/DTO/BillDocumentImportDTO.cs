using Microsoft.Data.SqlClient;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class BillDocumentImportDTO
    {
        public BillDocumentImport BillDocuments { get; set; }
        public string? lydo { get; set; }
        public string? note { get; set; }
    }
}
