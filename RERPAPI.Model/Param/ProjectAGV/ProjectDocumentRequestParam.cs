using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RERPAPI.Model.Param
{
    public class ProjectDocumentRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int ProjectID { get; set; } = 0;
        public byte Type { get; set; } = 0;
        public string? Keyword { get; set; } = "";


        public (string[] Names, object?[] Values) ToSqlParams()
        {
       
            DateTime? ds = DateStart?.Date;
            DateTime? de = DateEnd?.Date.AddDays(1).AddTicks(-1);

       
            if (ds.HasValue && de.HasValue && ds > de)
            {
                var t = ds; ds = de; de = t;
            }

            var kw = (Keyword ?? string.Empty).Trim();

            return (
                new[] { "@DateStart", "@DateEnd", "@ProjectID", "@Type", "@Keyword" },
                new object?[] { ds, de, ProjectID, Type, kw }
            );
        }
    }
}
