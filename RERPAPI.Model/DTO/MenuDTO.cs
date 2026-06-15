using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class MenuDTO : Menu
    {
        public List<Menu> MenuChilds { get; set; }
    }
}