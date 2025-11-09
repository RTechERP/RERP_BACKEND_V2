using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectWorkerVersionRepo : GenericRepo<ProjectWorkerVersion>
    {
        ProjectTypeRepo _projectTypeRepo;

        public ProjectWorkerVersionRepo(CurrentUser currentUser, ProjectTypeRepo projectTypeRepo) : base(currentUser)
        {
            _projectTypeRepo = projectTypeRepo;
        }

        public string LoadVersionCode(int projectSolutionId, int projectTypeId)
        {
            var maxCode = GetAll(x => x.ProjectSolutionID == projectSolutionId && x.ProjectTypeID == projectTypeId && x.IsDeleted == false)
                .Max(x => x.Code);
            if (string.IsNullOrEmpty(maxCode))
            {
                return "V1";
            }
            else
            {
                int nextCode = int.Parse(maxCode) + 1;
                return "V" + nextCode.ToString();
            }
        }
        public bool Validate(ProjectWorkerVersion item, out string message)
        {
            message = "";
            if (item.IsDeleted == true && item.ID > 0)
            {
                if (item.IsActive == true)
                {
                    message = $"Phiên bản [{item.Code}] đang được sử dụng.\nBạn không thể xoá!";
                    return false;
                }

                return true; // Nếu IsDeleted = true, chỉ cần kiểm tra lý do xóa
            }

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
            if (item.ID > 0)
            {
                var exist = GetAll().Any(x => x.Code == item.Code && x.ProjectSolutionID == item.ProjectSolutionID && x.ProjectTypeID == item.ProjectTypeID && x.StatusVersion == item.StatusVersion && x.ID != item.ID && x.IsDeleted == false);
                if (exist)
                {
                    message = $"Mã phiên bản [{item.Code}] đã tồn tại!";
                    return false;
                }
                if (item.IsActive == true)
                {
                    List<ProjectWorkerVersion> projectWorkerVersions = GetAll(x => x.ProjectSolutionID == item.ProjectSolutionID && x.ProjectTypeID == item.ProjectTypeID && x.StatusVersion == item.StatusVersion && x.IsDeleted == false && x.IsActive == true && x.ID != item.ID).ToList();
                    if (projectWorkerVersions.Any())
                    {
                        var projectType = _projectTypeRepo.GetByID(item.ProjectTypeID ?? 0);
                        message = $"Danh mục [{projectType.ProjectTypeName}] đã có phiên bản khác được sử dụng.\nVui lòng kiểm tra lại!";
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
                        message = $"Danh mục [{_projectTypeRepo.GetByID(item.ProjectTypeID ?? 0).ProjectTypeName}] đã có phiên bản PO!";
                        return false;
                    }
                }
            }
            return true;
        }
        public bool ValidateApprove(ProjectPartListVersion version, bool isApproved, out string message)
        {
            message = "";

            if (version == null)
            {
                message = "Không tìm thấy phiên bản.";
                return false;
            }

            if (isApproved && version.IsApproved == true)
            {
                message = "Phiên bản này đã được duyệt trước đó.";
                return false;
            }

            if (!isApproved && version.IsApproved == false)
            {
                message = "Phiên bản này chưa được duyệt để hủy.";
                return false;
            }

            return true;
        }
    }
}
