using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListHistoryLogRepo : GenericRepo<ProjectPartListHistoryLog>
    {
        private readonly ProjectTypeRepo _projectTypeRepo;
        public ProjectPartListHistoryLogRepo(CurrentUser currentUser, ProjectTypeRepo projectTypeRepo) : base(currentUser)
        {
            _projectTypeRepo = projectTypeRepo;
        }

        public async Task AddLog(
            int? projectId,
            int? versionId,
            int? partListID,
            string actionType,
            string contentLog,
            string createdBy,
            int? createdByEmployeeID)
        {
            var log = new ProjectPartListHistoryLog
            {
                ProjectID = projectId,
                ProjectPartListVersionID = versionId,
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

         public async Task<List<object>> GetLogsWithVersionInfo(int? projectId, int? versionId)
        {
            var paramNames = new string[] { "@ProjectID", "@ProjectPartListVersionID" };
            var paramValues = new object[] { projectId ?? (object)DBNull.Value, versionId ?? (object)DBNull.Value };

            List<spGetProjectPartListHistoryLogDTO> list = SQLHelper<spGetProjectPartListHistoryLogDTO>.ProcedureToListModel(
                "spGetProjectPartListHistoryLog",
                paramNames,
                paramValues
            );

            return list.Cast<object>().ToList();
        }

        public string BuildVersionDiff(ProjectPartListVersion oldData, ProjectPartListVersion newData)
        {
            var changes = new System.Collections.Generic.List<string>();
            var projectTypeNameOld = _projectTypeRepo.GetByID(oldData.ProjectTypeID ?? 0);
            var projectTypeNameNew = _projectTypeRepo.GetByID(newData.ProjectTypeID ?? 0);
            if ((oldData.ProjectTypeID != newData.ProjectTypeID)) changes.Add($"- Danh mục: {projectTypeNameOld.ProjectTypeName} → {projectTypeNameNew.ProjectTypeName} ");
            if (oldData.Code != newData.Code) changes.Add($"- Mã phiên bản: {oldData.Code} → {newData.Code}");
            if (oldData.DescriptionVersion != newData.DescriptionVersion) changes.Add($"- Mô tả: {oldData.DescriptionVersion} → {newData.DescriptionVersion}");


            if (oldData.StatusVersion != newData.StatusVersion)
            {
                string oldStatus = oldData.StatusVersion == 2 ? "PO" : "GP";
                string newStatus = newData.StatusVersion == 2 ? "PO" : "GP";
                changes.Add($"- Loại phiên bản: {oldStatus} → {newStatus}");
            }
            if ((oldData.IsActive == true) != (newData.IsActive == true))
            {
                string oldActive = oldData.IsActive == true ? "Sử dụng" : "Không sử dụng";
                string newActive = newData.IsActive == true ? "Sử dụng" : "Không sử dụng";
                changes.Add($"- Trạng thái sử dụng: {oldActive} → {newActive}");
            }
            if ((oldData.IsApproved == true) != (newData.IsApproved == true))
            {
                string oldApp = oldData.IsApproved == true ? "Đã duyệt" : "Chưa duyệt";
                string newApp = newData.IsApproved == true ? "Đã duyệt" : "Hủy duyệt";
                changes.Add($"- Trạng thái duyệt: {oldApp} → {newApp}");
            }
            if ((oldData.IsConsumable == true) != (newData.IsConsumable == true))
            {
                string oldConsumable = oldData.IsConsumable == true ? "Tiêu hao" : "Không tiêu hao";
                string newConsumable = newData.IsConsumable == true ? "Tiêu hao" : "Không tiêu hao";
                changes.Add($"- Trạng thái vật tư tiêu hao: {oldConsumable} → {newConsumable}");
            }
            if ((oldData.IsDeleted == true) != (newData.IsDeleted == true))
            {
                string oldDel = oldData.IsDeleted == true ? "Đã xóa" : "Hoạt động";
                string newDel = newData.IsDeleted == true ? "Đã xóa" : "Hoạt động";
                changes.Add($"- Trạng thái xóa: {oldDel} → {newDel}");
            }
           
            return changes.Count > 0 ? string.Join("\n", changes) : null;
        }

        public string BuildSolutionDiff(ProjectSolution oldData, ProjectSolution newData)
        {
            var changes = new System.Collections.Generic.List<string>();
            if (oldData.CodeSolution != newData.CodeSolution) changes.Add($"- Mã giải pháp: {oldData.CodeSolution} → {newData.CodeSolution}");
            if (oldData.ContentSolution != newData.ContentSolution) changes.Add($"- Nội dung giải pháp: {oldData.ContentSolution} → {newData.ContentSolution}");
            if (oldData.ProjectRequestID != newData.ProjectRequestID) changes.Add($"- ID yêu cầu liên kết: {oldData.ProjectRequestID} → {newData.ProjectRequestID}");
            if ((oldData.IsDeleted == true) != (newData.IsDeleted == true))
            {
                string oldDel = oldData.IsDeleted == true ? "Đã xóa" : "Hoạt động";
                string newDel = newData.IsDeleted == true ? "Đã xóa" : "Hoạt động";
                changes.Add($"- Trạng thái xóa: {oldDel} → {newDel}");
            }
            return changes.Count > 0 ? string.Join("\n", changes) : null;
        }
    }
}
