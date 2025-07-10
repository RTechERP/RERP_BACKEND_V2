using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Context
{
    public partial class RTCContext
    {
        //private readonly IUserPermissionService _userPermissionService;
        public RTCContext()
        {

        }

        //public RTCContext(IUserPermissionService userPermissionService)
        //{
        //    _userPermissionService = userPermissionService;
        //}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Config.ConnectionString);


        public override int SaveChanges()
        {
            //string loginName = _userPermissionService.GetClaims().GetValueOrDefault("loginname") ?? "";
            string loginName = "";

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
    }
}
