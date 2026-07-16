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
            int? createdByEmployeeID,
            int? projectId = null,
            int? versionId = null)
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

            // Log to unified ProjectPartListHistoryLog
            try
            {
                ProjectPartListVersion version = null;
                if (projectId == null || versionId == null)
                {
                    if (partListID.HasValue)
                    {
                        var pjPartList = db.ProjectPartLists.FirstOrDefault(x => x.ID == partListID.Value);
                        if (pjPartList != null)
                        {
                            versionId ??= pjPartList.ProjectPartListVersionID;
                            if (versionId.HasValue)
                            {
                                version = db.ProjectPartListVersions.FirstOrDefault(x => x.ID == versionId.Value);
                                projectId ??= version?.ProjectID;
                            }
                        }
                    }
                }
                else if (versionId.HasValue)
                {
                    version = db.ProjectPartListVersions.FirstOrDefault(x => x.ID == versionId.Value);
                }

                if (projectId.HasValue)
                {
                    string contentWithVersion = contentLog;
                    if (version == null && versionId.HasValue)
                    {
                        version = db.ProjectPartListVersions.FirstOrDefault(x => x.ID == versionId.Value);
                    }
                    var projectTypeName = db.ProjectTypes.FirstOrDefault(x => x.ID == version.ProjectTypeID);
                    if (version != null)
                    {
                        string typeStr = version.StatusVersion == 2 ? "PO" : "GP";
                        contentWithVersion += $" [Phiên bản: {version.Code} ({typeStr}) - {projectTypeName.ProjectTypeName}]";
                    }

                    var historyLog = new ProjectPartListHistoryLog
                    {
                        ProjectID = projectId,
                        ProjectPartListVersionID = versionId,
                        ProjectPartListID = partListID,
                        ActionType = actionType,
                        ContentLog = contentWithVersion,
                        CreatedBy = createdBy,
                        CreatedByEmployeeID = createdByEmployeeID,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    db.ProjectPartListHistoryLogs.Add(historyLog);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Prevent log failures from blocking normal transactions
            }
        }

        public string BuildPartListDiff(ProjectPartList oldData, ProjectPartList newData)
        {
            var changes = new System.Collections.Generic.List<string>();
            if (oldData.TT != newData.TT) changes.Add($"- Thứ tự (TT): {oldData.TT} → {newData.TT}");
            if (oldData.ProductCode != newData.ProductCode) changes.Add($"- Mã SP: {oldData.ProductCode} → {newData.ProductCode}");
            if (oldData.GroupMaterial != newData.GroupMaterial) changes.Add($"- Tên vật tư: {oldData.GroupMaterial} → {newData.GroupMaterial}");
            if (oldData.QtyFull != newData.QtyFull) changes.Add($"- Số lượng: {oldData.QtyFull} → {newData.QtyFull}");
            if (oldData.QtyMin != newData.QtyMin) changes.Add($"- SL tối thiểu: {oldData.QtyMin} → {newData.QtyMin}");
            if (oldData.Unit != newData.Unit) changes.Add($"- Đơn vị: {oldData.Unit} → {newData.Unit}");
            if (oldData.Manufacturer != newData.Manufacturer) changes.Add($"- Hãng SX: {oldData.Manufacturer} → {newData.Manufacturer}");
            if (oldData.Model != newData.Model) changes.Add($"- Model: {oldData.Model} → {newData.Model}");
            if (oldData.SpecialCode != newData.SpecialCode) changes.Add($"- Mã đặc biệt: {oldData.SpecialCode} → {newData.SpecialCode}");
            if (oldData.ReasonProblem != newData.ReasonProblem) changes.Add($"- Lý do: {oldData.ReasonProblem} → {newData.ReasonProblem}");
            if (oldData.Note != newData.Note) changes.Add($"- Ghi chú: {oldData.Note} → {newData.Note}");
            return changes.Count > 0 ? string.Join("\n", changes) : null;
        }
    }
}
