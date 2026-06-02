using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.Film
{
    public class FilmMangementDTO
    {
        public FilmManagement? filmManagement { get; set; }
        public List<FilmManagementDetail>? filmManagementDetails { get; set; }
    }
}