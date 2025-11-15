using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectSolutionRepo : GenericRepo<ProjectSolution>
    {
        public ProjectSolutionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetSolutionCode(int projectRequestId)
        {
            var solutions = GetAll(x => x.ProjectRequestID == projectRequestId).ToList();
            int stt = solutions.Count > 0 ? solutions.Max(x => (x.STT ?? 0)) + 1 : 1;
            return $"GP{stt}";
        }
        public bool Validate(ProjectSolution item, out string message)
        {
            message = "";
            if (item.IsDeleted == true && item.ID > 0)
            {
                //if (item.IsActive == true)
                //{
                //    message = $"Giải pháp [{item.CodeSolution}] đang được sử dụng.\nBạn không thể xoá!";
                //    return false;
                //}
                return true; // Nếu IsDeleted = true, chỉ cần kiểm tra lý do xóa
            }
            if (item.ID > 0)
            {
                var existingSolutions = GetAll().Any(x => 
                    x.CodeSolution == item.CodeSolution && 
                    x.ProjectRequestID == item.ProjectRequestID && 
                    x.ID != item.ID && 
                    x.IsDeleted == false);
                if (existingSolutions)
                {
                    message = $"Mã giải pháp {item.CodeSolution} đã tồn tại!";
                    return false;
                }
            }
            // Kiểm tra điều kiện cơ bản
            if (item.ProjectRequestID <= 0)
            {
                message = "Vui lòng chọn Yêu cầu Dự án";
                return false;
            }   
            if (string.IsNullOrEmpty(item.CodeSolution))
            {
                message = "Vui lòng nhập Mã giải pháp";
                return false;
            }
            if (string.IsNullOrEmpty(item.ContentSolution))
            {
                message = "Vui lòng nhập Mã giải pháp";
                return false;
            }
            return true;
        }
        /// <summary>
        /// Validate điều kiện duyệt (status: 1 = báo giá, 2 = PO)
        /// </summary>
        public bool ValidateApprove(int solutionId, bool isApproved, int status, out string message)
        {
            message = "";
            var solution = GetByID(solutionId);
            if (solution == null)
            {
                message = "Không tìm thấy giải pháp.";
                return false;
            }

            // Nếu duyệt PO nhưng chưa có PO
            if (isApproved && status == 2 && solution.StatusSolution == 0)
            {
                message = "Bạn không thể duyệt PO cho giải pháp không có PO!";
                return false;
            }

            if (status == 1) // Xử lý báo giá
            {
                if (isApproved && solution.IsApprovedPrice == true)
                {
                    message = "Giải pháp này đã được duyệt báo giá trước đó.";
                    return false;
                }
                if (!isApproved && solution.IsApprovedPrice == false)
                {
                    message = "Giải pháp này chưa được duyệt báo giá để hủy.";
                    return false;
                }
            }
            else if (status == 2) // Xử lý PO
            {
                if (isApproved && solution.IsApprovedPO == true)
                {
                    message = "Giải pháp này đã được duyệt PO trước đó.";
                    return false;
                }
                if (!isApproved && solution.IsApprovedPO == false)
                {
                    message = "Giải pháp này chưa được duyệt PO để hủy.";
                    return false;
                }
            }
            else
            {
                message = "Trạng thái không hợp lệ.";
                return false;
            }

            return true;
        }
    }
}