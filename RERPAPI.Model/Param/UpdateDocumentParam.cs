using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class UpdateDocumentParam
    {
        public List<int> idsPONCC {  get; set; }
        public int documentImportID { get; set; }
        public int deliverID { get; set; }
    }
}
