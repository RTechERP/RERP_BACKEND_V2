using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamRepo : GenericRepo<HRRecruitmentExam>
    {
        public HRRecruitmentExamRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool CheckValidate(HRRecruitmentExam item, out string message)
        {
            message = string.Empty;
            //if(item.IsActive==true)
            //{
            //    var exist = GetAll(x => x.IsActive == true && x.IsDeleted != true && x.ID != item.ID && x.HiringRequestID == item.HiringRequestID);
            //    if(exist.Count > 0)
            //    {
            //        message = $"Yêu cầu tuyển dụng đã có bài thi hoạt động. Vui lòng kiểm tra lại!";
            //        return false;
            //    }
            //}
            if (string.IsNullOrEmpty((item.NameExam ?? "").Trim()))
            {
                message = $"Vui lòng nhập tên đề thi!";
                return false;
            }
            if (string.IsNullOrEmpty((item.CodeExam ?? "").Trim()))
            {
                message = $"Vui lòng nhập mã đề thi!";
                return false;
            }
            var checkCode = GetAll(x => x.CodeExam == item.CodeExam && x.ID != item.ID).FirstOrDefault();
            if(checkCode != null)
            {
                message = $"Mã đề thi [{item.CodeExam}] đã tồn tại!";
                return false;
            }
            if (item.Goal == null || item.Goal <= 0)
            {
                message = $"Điểm cần đạt được phải lớn hơn 0!";
                return false;
            }
            if (item.TestTime == null || item.TestTime <= 0)
            {
                message = $"Thời gian làm bài phải lớn hơn 0!";
                return false;
            }
            if (item.ExamType == null)
            {
                message = $"Vui lòng chọn loại đề thi!";
                return false;
            }
            if (item.DepartmentID == null)
            {
                message = $"Vui lòng chọn phòng ban!";
                return false;
            }
            //if (item.HiringRequestID == null)
            //{
            //    message = $"Vui lòng chọn kỳ ứng tuyển!";
            //    return false;
            //}
            return true;
        }
    }
}
