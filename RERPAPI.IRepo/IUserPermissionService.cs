namespace RERPAPI.IRepo
{
    public interface IUserPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string permission);

        Dictionary<string, string> GetClaims();

        Dictionary<string, string> Claims { get; set; }
    }
}