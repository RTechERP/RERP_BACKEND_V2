using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListLogRepo : GenericRepo<ProjectPartListLog>
    {
        public ProjectPartListLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Ghi một bản ghi log thao tác vật tư partlist.
        /// </summary>
        /// <param name="partListID">ID vật tư (ProjectPartList.ID)</param>
        /// <param name="actionType">Loại thao tác (Thêm mới, Cập nhật, Xóa mềm, Duyệt TBP...)</param>
        /// <param name="contentLog">Nội dung chi tiết, bao gồm diff field nếu có</param>
        /// <param name="createdBy">Tên đăng nhập người thực hiện</param>
        /// <param name="createdByEmployeeID">Mã nhân viên thực hiện</param>
        public async Task AddLog(
            int? partListID,
            string actionType,
            string contentLog,
            string createdBy,
            int? createdByEmployeeID)
        {
            var log = new ProjectPartListLog
            {
                ProjectPartListID = partListID,
                ActionType = actionType,
                ContentLog = contentLog,
                CreatedBy = createdBy,
                CreatedByEmployeeID = createdByEmployeeID,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            await CreateAsync(log);
        }
    }
}
