using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RERPAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Context
{
    public partial class RTCContext
    {
        public RTCContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Config.ConnectionString);
        public override int SaveChanges()
        {

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

                if (item.State == EntityState.Added) //Thêm mới
                {
                    if (createdBy != null && createdBy.CanWrite) createdBy.SetValue(item.Entity, "Admin");
                    if (createdDate != null && createdDate.CanWrite) createdDate.SetValue(item.Entity, DateTime.Now);
                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, "Xuân Lươn");
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                    if (isDeleted != null && isDeleted.CanWrite) isDeleted.SetValue(item.Entity,false);
                }

                if (item.State == EntityState.Modified)
                {
                    if (updatedBy != null && updatedBy.CanWrite) updatedBy.SetValue(item.Entity, "");
                    if (updatedDate != null && updatedDate.CanWrite) updatedDate.SetValue(item.Entity, DateTime.Now);
                }
            }
            return base.SaveChanges();
        }
    }
    }