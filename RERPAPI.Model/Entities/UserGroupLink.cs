namespace RERPAPI.Model.Entities;

public partial class UserGroupLink
{
    public int ID { get; set; }

    public int? UserGroupID { get; set; }

    public int? UserID { get; set; }
}