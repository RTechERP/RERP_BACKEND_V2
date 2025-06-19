using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class CustomerPartSaveModel
    {
        public List<CustomerPart> AddedParts { get; set; } = new List<CustomerPart>();
        public List<CustomerPart> UpdatedParts { get; set; } = new List<CustomerPart>();
        public List<int> DeletedPartIds { get; set; } = new List<int>();
    }
}
