using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class MeetingTypeRepo : GenericRepo<MeetingType>
    {
        public MeetingTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool Validate(MeetingType entity, out string message)
        {
            if(entity.IsDeleted == true)
            {
                message = "Trường hợp xóa!";
                return true;
            }
            message = "";
            if (string.IsNullOrEmpty(entity.TypeName))
            {
                message = "Tên loại cuộc họp không được để trống!";
                return false;
            }
            if (entity.TypeCode == "")
            {
                message = "Mã loại cuộc họp không được để trống!";
                return false;
            }
            if (entity.GroupID == null)
            {
                message = "Loại cuộc họp không được để trống!";
                return false;
            }

            return true;
        }
        public bool CheckTypeCodeExists(string typeCode, int? id = null)
        {
            try
            {
                var query = GetAll(f => f.TypeCode.ToUpper() == typeCode.ToUpper() && f.IsDeleted != true);
                if (id.HasValue)
                {
                    query = query.Where(f => f.ID != id.Value).ToList();
                }
                return query.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã loại cuộc họp: {ex.Message}", ex);
            }
        }
    }
}
