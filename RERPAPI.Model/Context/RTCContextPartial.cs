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
                var deleteFlag = type.GetProperty("DeleteFlag");

                if (item.State == EntityState.Added) //Thêm mới
                {
                    if (createdBy != null && createdBy.CanWrite) createdBy.SetValue(item.Entity, loginName);
                    if (createdDate != null && createdDate.CanWrite) createdDate.SetValue(item.Entity, DateTime.Now);
                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
                    //if (name != null && name.CanWrite) name.SetValue(item.Entity, loginName);
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                    //if (isDeleted != null && isDeleted.CanWrite) isDeleted.SetValue(item.Entity, false);
                    //if (isDelete != null && isDelete.CanWrite) isDelete.SetValue(item.Entity, false);
                    if (isDeleted != null && isDeleted.CanWrite)
                    {
                        var propType = isDeleted.PropertyType;

                        if (propType == typeof(bool) || propType == typeof(bool?))
                            isDeleted.SetValue(item.Entity, false);
                        else if (propType == typeof(int) || propType == typeof(int?))
                            isDeleted.SetValue(item.Entity, 0);
                    }

                    if (isDelete != null && isDelete.CanWrite)
                    {
                        var propType = isDelete.PropertyType;

                        if (propType == typeof(bool) || propType == typeof(bool?))
                            isDelete.SetValue(item.Entity, false);
                        else if (propType == typeof(int) || propType == typeof(int?))
                            isDelete.SetValue(item.Entity, 0);
                    }

                    if (deleteFlag != null && deleteFlag.CanWrite)
                    {
                        var propType = deleteFlag.PropertyType;
                        if (propType == typeof(bool) || propType == typeof(bool?))
                            deleteFlag.SetValue(item.Entity, false);
                        else if (propType == typeof(int) || propType == typeof(int?))
                            deleteFlag.SetValue(item.Entity, 0);
                    }


                }

                //if (item.State == EntityState.Modified)
                //{
                //    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
                //    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                //}

                if (item.State == EntityState.Modified)
                {

                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
                    if (updatedDate != null && updatedDate.CanWrite)
                    {
                        var updatedDateValue = updatedDate.GetValue(item.Entity);
                        updatedDate.SetValue(item.Entity, updatedDateValue == null ? DateTime.Now : updatedDateValue);
                    }
                }
            }

            AddAuditLogs();
            return base.SaveChanges();
        }

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        string loginName = CurrentUser.LoginName;
        //        var entries = ChangeTracker.Entries()
        //                                   .Where(x => x.Entity != null && (x.State == EntityState.Added || x.State == EntityState.Modified));

        //        foreach (var item in entries)
        //        {
        //            var type = item.Entity.GetType();

        //            var createdBy = type.GetProperty("CreatedBy");
        //            var createdDate = type.GetProperty("CreatedDate");
        //            var updatedBy = type.GetProperty("UpdatedBy");
        //            var updatedDate = type.GetProperty("UpdatedDate");

        //            var isDeleted = type.GetProperty("IsDeleted");
        //            var isDelete = type.GetProperty("IsDelete");
        //            var deleteFlag = type.GetProperty("DeleteFlag");

        //            if (item.State == EntityState.Added) //Thêm mới
        //            {
        //                if (createdBy != null && createdBy.CanWrite) createdBy.SetValue(item.Entity, loginName);
        //                if (createdDate != null && createdDate.CanWrite) createdDate.SetValue(item.Entity, DateTime.Now);
        //                if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
        //                if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
        //                //if (isDeleted != null && isDeleted.CanWrite) isDeleted.SetValue(item.Entity, false);
        //                //if (isDelete != null && isDelete.CanWrite) isDelete.SetValue(item.Entity, false);

        //                if (isDeleted != null && isDeleted.CanWrite)
        //                {
        //                    var propType = isDeleted.PropertyType;

        //                    if (propType == typeof(bool) || propType == typeof(bool?))
        //                        isDeleted.SetValue(item.Entity, false);
        //                    else if (propType == typeof(int) || propType == typeof(int?))
        //                        isDeleted.SetValue(item.Entity, 0);
        //                }

        //                if (isDelete != null && isDelete.CanWrite)
        //                {
        //                    var propType = isDelete.PropertyType;

        //                    if (propType == typeof(bool) || propType == typeof(bool?))
        //                        isDelete.SetValue(item.Entity, false);
        //                    else if (propType == typeof(int) || propType == typeof(int?))
        //                        isDelete.SetValue(item.Entity, 0);
        //                }

        //                if (deleteFlag != null && deleteFlag.CanWrite)
        //                {
        //                    var propType = deleteFlag.PropertyType;
        //                    if (propType == typeof(bool) || propType == typeof(bool?))
        //                        deleteFlag.SetValue(item.Entity, false);
        //                    else if (propType == typeof(int) || propType == typeof(int?))
        //                        deleteFlag.SetValue(item.Entity, 0);
        //                }
        //            }

        //            if (item.State == EntityState.Modified)
        //            {

        //                if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, loginName);
        //                if (updatedDate != null && updatedDate.CanWrite)
        //                {
        //                    var updatedDateValue = updatedDate.GetValue(item.Entity);
        //                    updatedDate.SetValue(item.Entity, updatedDateValue == null ? DateTime.Now : updatedDateValue);
        //                }
        //            }
        //        }
        //        var hasAttendance = entries.Any(e => e.Entity != null && e.Entity.GetType().Name == "EmployeeAttendance");
        //        //AddAuditLogs();

        //        if (!hasAttendance)
        //        {
        //            AddAuditLogs();
        //        }
        //        return await base.SaveChangesAsync(cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.ToString());
        //    }
        //}
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var now = DateTime.Now;
                var loginName = CurrentUser?.LoginName ?? "SYSTEM";

                // 1. Lấy danh sách entity cần xử lý (materialize ngay tránh bị thay đổi sau)
                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity != null &&
                               (e.State == EntityState.Added || e.State == EntityState.Modified))
                    .ToList();

                // 2. Set audit field (KHÔNG dùng reflection nhiều lần)
                foreach (var entry in entries)
                {
                    var entity = entry.Entity;
                    var type = entity.GetType();

                    // cache property (giảm reflection cost)
                    var createdBy = type.GetProperty("CreatedBy");
                    var createdDate = type.GetProperty("CreatedDate");
                    var updatedBy = type.GetProperty("UpdatedBy");
                    var updatedDate = type.GetProperty("UpdatedDate");

                    if (entry.State == EntityState.Added)
                    {
                        createdBy?.SetValue(entity, loginName);
                        createdDate?.SetValue(entity, now);

                        updatedBy?.SetValue(entity, loginName);
                        updatedDate?.SetValue(entity, now);

                        SetDefaultDeleteFlag(entity, type);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        updatedBy?.SetValue(entity, loginName);

                        if (updatedDate != null)
                        {
                            var val = updatedDate.GetValue(entity);
                            if (val == null)
                                updatedDate.SetValue(entity, now);
                        }
                    }
                }

                // 3. Tạo audit log NHƯNG KHÔNG add vào DbContext ngay
                var logs = BuildAuditLogs();

                // 4. Save entity chính trước (transaction ngắn lại)
                var result = await base.SaveChangesAsync(cancellationToken);

                // 5. Save log riêng (tránh lock + timeout)
                if (logs.Count > 0)
                {
                    await SaveAuditLogsAsync(logs);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"SaveChanges failed: {ex.Message}", ex);
            }
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
                if (entry.Entity.GetType().Name == "ActivityLog") continue;

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

            ActivityLogs.AddRange(logs);
        }
        private void SetDefaultDeleteFlag(object entity, Type type)
        {
            var props = new[] { "IsDeleted", "IsDelete", "DeleteFlag" };

            foreach (var name in props)
            {
                var prop = type.GetProperty(name);
                if (prop == null || !prop.CanWrite) continue;

                var t = prop.PropertyType;

                if (t == typeof(bool) || t == typeof(bool?))
                    prop.SetValue(entity, false);
                else if (t == typeof(int) || t == typeof(int?))
                    prop.SetValue(entity, 0);
            }
        }
        private List<ActivityLog> BuildAuditLogs()
        {
            return ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .Where(e => e.Entity.GetType().Name != "ActivityLog")
                .Select(e => new ActivityLog
                {
                    LogTime = DateTime.UtcNow,
                    UserID = CurrentUser.ID,
                    EmployeeID = CurrentUser.EmployeeID,
                    Application = "WEB R-ERP",
                    FormName = e.Entity.GetType().Name,
                    Action = e.State.ToString(),
                    Details = GetDetails(e),
                    ControlName = ""
                })
                .ToList();
        }
        private async Task SaveAuditLogsAsync(List<ActivityLog> logs)
        {
            try
            {
                ActivityLogs.AddRange(logs);
                await base.SaveChangesAsync();
            }
            catch
            {
                // ❗ Không throw để tránh fail main flow
                // Có thể log file nếu cần
            }
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
                if (entry.State == EntityState.Modified && Equals(original, current) && prop.Name != "ID")
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