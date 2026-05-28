using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.Entities.ESL
{
    public class ESLTestTable
    {
        public int ID { get; set; }
        public string TestTableName { get; set; }
        public string Barcode { get; set; }
        public int TableSide { get; set; }
        public int? NumberOfSides { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
