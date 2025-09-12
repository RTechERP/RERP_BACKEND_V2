using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectDocumentsRepo: GenericRepo<ProjectDocuments>
    {
        public bool ValidateForSave(ProjectDocuments item, out string message)
        {
            message = string.Empty;
            if (item == null)
            {
                message = "Dữ liệu rỗng.";
                return false;
            }

      
            item.Name = item.Name?.Trim();
            item.FilePath = item.FilePath?.Trim();
            item.Version = item.Version?.Trim();
            item.CreateBy = string.IsNullOrWhiteSpace(item.CreateBy) ? null : item.CreateBy.Trim();
            item.UpdatedBy = string.IsNullOrWhiteSpace(item.UpdatedBy) ? null : item.UpdatedBy.Trim();

            // Các kiểm tra bắt buộc
            if (item.ProjectID <= 0)
            {
                message = "Vui lòng chọn dự án (ProjectID).";
                return false;
            }
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                message = "Vui lòng nhập tên tài liệu (Name).";
                return false;
            }
            if (item.Type <= 0 || item.Type > 3)
            {
                message = "Vui lòng chọn loại tài liệu hợp lệ (Type = 1: charter, 2: requirements, 3: design).";
                return false;
            }
            if (string.IsNullOrWhiteSpace(item.FilePath))
            {
                message = "Vui lòng nhập đường dẫn tệp (FilePath).";
                return false;
            }
            if (string.IsNullOrWhiteSpace(item.Version))
            {
                message = "Vui lòng nhập phiên bản (Version).";
                return false;
            }
            if (item.Size.HasValue && item.Size.Value < 0)
            {
                message = "Kích thước tệp (Size) không hợp lệ.";
                return false;
            }

            return true;
        }

   
        public void PrepareForSave(ProjectDocuments item, bool isInsert)
        {
            // Chuẩn hoá chuỗi
            item.Name = item.Name?.Trim();
            item.FilePath = item.FilePath?.Trim();
            item.Version = item.Version?.Trim();
            item.CreateBy = string.IsNullOrWhiteSpace(item.CreateBy) ? null : item.CreateBy.Trim();
            item.UpdatedBy = string.IsNullOrWhiteSpace(item.UpdatedBy) ? null : item.UpdatedBy.Trim();

         
            if (isInsert)
            {
                item.CreateDate = DateTime.Now;
     
                if (item.IsDeleted == null) item.IsDeleted = false;
            }

            item.UpdatedDate = DateTime.Now;
        }
    }
}
