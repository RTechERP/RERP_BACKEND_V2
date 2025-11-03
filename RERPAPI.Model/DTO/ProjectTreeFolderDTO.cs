using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class ProjectTreeFolderDTO
    {
        public int ID { get; set; }

        public string? FolderName { get; set; }

        public int? ParentID { get; set; }

        public int? ProjectTypeID { get; set; }
    }
}