using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.Film
{
    public class FilmMangementDTO
    {
        public FilmManagement? filmManagement { get; set; }
        public List<FilmManagementDetail>? filmManagementDetails { get; set; }
    }
}
