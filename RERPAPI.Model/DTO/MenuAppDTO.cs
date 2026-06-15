using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class MenuAppDTO : MenuApp
    {
        public List<MenuAppUserGroupLink> MenuAppUserGroupLinks { get; set; } = new List<MenuAppUserGroupLink>();
    }
}