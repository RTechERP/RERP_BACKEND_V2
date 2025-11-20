using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistVersionRepo : GenericRepo<ProjectPartListVersion>
    {
        private ProjectTypeRepo _projectTypeRepo;


        public ProjectPartlistVersionRepo(CurrentUser currentUser, ProjectTypeRepo projectTypeRepo) : base(currentUser)
        {
            _projectTypeRepo = projectTypeRepo;
        }


        public ProjectPartlistVersionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(ProjectPartListVersion item, out string message)
        {
            message = "";
            ProjectPartListVersion oldVersion = GetByID(item.ID);
            if (oldVersion.IsApproved != item.IsApproved)
            {
                return true;
            }
            if (item.IsDeleted == true && item.ID > 0)
            {
                if (string.IsNullOrEmpty(item.ReasonDeleted))
                {
                    message = "Vui lòng nhập lý do xóa";
                    return false;
                }
                if (item.IsActive == true)
                {
                    message = $"Phiên bản [{item.Code}] đang được sử dụng.\nBạn không thể xoá!";
                    return false;
                }
                if (item.IsApproved == true)
                {
                    message = $"Phiên bản [{item.Code}] đã được phê duyệt.\nBạn không thể xoá!";
                    return false;
                }
                return true; // Nếu IsDeleted = true, chỉ cần kiểm tra lý do xóa
            }
            // Kiểm tra điều kiện cơ bản
            if (item.ID < 0 || item.CreatedDate.HasValue)
            {
                if (item.ProjectTypeID <= 0)
                {
                    message = "Vui lòng chọn Danh mục";
                    return false;
                }
                if (item.ProjectSolutionID <= 0)
                {
                    message = "Vui lòng chọn Giải pháp";
                    return false;
                }
            }

            if (string.IsNullOrEmpty(item.Code))
            {
                message = "Vui lòng nhập Mã phiên bản";
                return false;
            }
            if (item.StatusVersion <= 0)
            {
                message = "Vui lòng nhập Trạng thái";
                return false;
            }
            if (string.IsNullOrEmpty(item.DescriptionVersion))
            {
                message = "Vui lòng nhập Mô tả";
                return false;
            }

            // Nếu IsActive = true thì phải đảm bảo không có phiên bản khác đã active cùng ProjectType + Solution + Status
            if (item.IsActive == true)
            {
                List<ProjectPartListVersion> projectPartListVersions = GetAll(
                    x => x.ID != item.ID
                         && x.ProjectTypeID == item.ProjectTypeID
                         && x.ProjectSolutionID == item.ProjectSolutionID
                         && x.IsActive == true
                         && x.StatusVersion == item.StatusVersion
                         && x.IsDeleted == false
                );

                if (projectPartListVersions.Count > 0)
                {
                    ProjectType current = _projectTypeRepo.GetByID(item.ProjectTypeID ?? 0);
                    message = $"Danh mục [{current.ProjectTypeName}] đã có phiên bản khác được sử dụng.\nVui lòng kiểm tra lại!";
                    return false;
                }
            }
            if (item.StatusVersion == 2)
            {
                var existsPO = GetAll()
                .Any(x => x.ProjectTypeID == item.ProjectTypeID
                 && x.ProjectSolutionID == item.ProjectSolutionID
                 && x.StatusVersion == 2
                 && x.IsDeleted == false
                 && x.ID != item.ID);

                if (existsPO)
                {
                    var current = _projectTypeRepo.GetByID(item.ProjectTypeID ?? 0);
                    message = $"Danh mục [{current.ProjectTypeName}] đã có phiên bản PO!";
                    return false;
                }
            }
            return true;
        }
        public bool ValidateApprove(ProjectPartListVersion version, out string message)
        {
            message = "";
            ProjectPartListVersion oldVersion = GetByID(version.ID);

            if (version == null || oldVersion.ID <= 0)
            {
                message = "Không tìm thấy phiên bản.";
                return false;
            }

            if (version.IsApproved == true && oldVersion.IsApproved == true)
            {
                message = "Phiên bản này đã được duyệt trước đó.";
                return false;
            }

            if (version.IsApproved == false && oldVersion.IsApproved == false)
            {
                message = "Phiên bản này chưa được duyệt để hủy.";
                return false;
            }

            return true;
        }
    }
}