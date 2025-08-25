using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;

namespace RERPAPI.Model.Context
{
    public partial class RTCContext
    {
        //public string LoginName { get; set; } = string.Empty;
        //public Dictionary<string,string> Claim { get; set; }

        //public CurrentUser currentUser = new CurrentUser();
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
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
    }