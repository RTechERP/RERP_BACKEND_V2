using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Text.Json;

namespace RERPAPI.Model.Context
{
    public partial class RTCContext
    {

        public CurrentUser CurrentUser { get; set; } = new CurrentUser();
        public RTCContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Config.ConnectionString);

        public override int SaveChanges()
        {
            //LoginName = _httpContextAccessor.HttpContext?.User?.FindFirst("loginname")?.Value;
            string loginName = CurrentUser.LoginName;
            var entries = ChangeTracker.Entries()
                                            .Where(x => x.Entity != null && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var item in entries)
            {
                var type = item.Entity.GetType();

                var createdBy = type.GetProperty("CreatedBy");
                var createdDate = type.GetProperty("CreatedDate");
                var updatedBy = type.GetProperty("UpdatedBy");
                var updatedDate = type.GetProperty("UpdatedDate");
                var isDeleted = type.GetProperty("IsDeleted");
                var isDelete = type.GetProperty("IsDelete");

                if (item.State == EntityState.Added) //Thêm mới
                {
                    if (createdBy != null && createdBy.CanWrite) createdBy.SetValue(item.Entity, loginName);
                    if (createdDate != null && createdDate.CanWrite) createdDate.SetValue(item.Entity, DateTime.Now);
                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                    if (isDeleted != null && isDeleted.CanWrite) isDeleted.SetValue(item.Entity, false);
                    if (isDelete != null && isDelete.CanWrite) isDelete.SetValue(item.Entity, false);
                }

                if (item.State == EntityState.Modified)
                {
                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                }
            }

            AddAuditLogs();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            string loginName = CurrentUser.LoginName;
            //var claim = _userPermissionService.Claims;
            var entries = ChangeTracker.Entries()
                                            .Where(x => x.Entity != null && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var item in entries)
            {
                var type = item.Entity.GetType();

                var createdBy = type.GetProperty("CreatedBy");
                var createdDate = type.GetProperty("CreatedDate");
                var updatedBy = type.GetProperty("UpdatedBy");
                var updatedDate = type.GetProperty("UpdatedDate");
                var isDeleted = type.GetProperty("IsDeleted");
                var name = type.GetProperty("Name");

                if (item.State == EntityState.Added) //Thêm mới
                {
                    if (createdBy != null && createdBy.CanWrite) createdBy.SetValue(item.Entity, Convert.ToString(createdBy.GetValue(item.Entity)) ?? loginName);
                    if (createdDate != null && createdDate.CanWrite) createdDate.SetValue(item.Entity, DateTime.Now);
                    if (updatedBy != null && updatedBy.CanWrite && updatedBy.GetValue(item.Entity) != null) updatedBy.SetValue(item.Entity, Convert.ToString(updatedBy.GetValue(item.Entity)) ?? loginName);
                    if (name != null && name.CanWrite) name.SetValue(item.Entity, loginName);
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                    if (isDeleted != null && isDeleted.CanWrite) isDeleted.SetValue(item.Entity, false);
                }

                if (item.State == EntityState.Modified)
                {

                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, Convert.ToString(updatedBy.GetValue(item.Entity)) ?? loginName); ;
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                }
            }

            AddAuditLogs();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            var logs = new List<ActivityLog>();
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                var log = new ActivityLog
                {
                    LogTime = DateTime.UtcNow,
                    UserID = CurrentUser.ID, // TODO: lấy từ HttpContext nếu có DI
                    Application = "WEB R-ERP",
                    FormName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(), // Added / Modified / Deleted
                    Details = GetDetails(entry),
                    EmployeeID = CurrentUser.EmployeeID,
                    ControlName = ""
                };


                logs.Add(log);
            }


            //ActivityLogs.AddRange(logs);
        }

        private string GetDetails(EntityEntry entry)
        {
            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            foreach (var prop in entry.OriginalValues.Properties)
            {
                var original = entry.OriginalValues[prop];
                var current = entry.CurrentValues[prop];

                // Chỉ log những cột thay đổi
                if (entry.State == EntityState.Modified && Equals(original, current))
                    continue;

                oldValues[prop.Name] = original;
                newValues[prop.Name] = current;
            }

            var wrapper = new
            {
                Old = new List<Dictionary<string, object?>> { oldValues },
                New = new List<Dictionary<string, object?>> { newValues }
            };

            return System.Text.Json.JsonSerializer.Serialize(wrapper,
                new JsonSerializerOptions { WriteIndented = true });
        }

    }
}