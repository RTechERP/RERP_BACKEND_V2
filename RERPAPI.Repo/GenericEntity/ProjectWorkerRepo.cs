using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectWorkerRepo: GenericRepo <ProjectWorker>
    {
        public ProjectWorkerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public bool checkTTExists(string tt, int? parentID = 0, int? id = null, int versionID=0)
        {
            try
            {
                var query = GetAll(pw => pw.TT!.ToUpper() == tt.ToUpper() && pw.IsDeleted != true && pw.ProjectWorkerVersionID == versionID);
                if (id.HasValue)
                {
                    query = query.Where(pw => pw.ID != id.Value).ToList();
                }
                return query.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra TT: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Tìm ParentID từ TT của con
        /// Nếu không tìm thấy cha → coi node này là gốc (ParentID = 0)
        /// </summary>
        public int FindParentIdByTT(string childTT, int versionID)
        {
            if (string.IsNullOrWhiteSpace(childTT))
                return 0;

            childTT = childTT.Trim();

            // 1. Nếu là gốc (không có dấu chấm) → ParentID = 0
            if (!childTT.Contains("."))
                return 0;

            // 2. Tách phần cha: 3.3 → 3
            var parts = childTT.Split('.');
            if (parts.Length <= 1) return 0;

            string parentTT = string.Join(".", parts.Take(parts.Length - 1));

            // 3. Tìm cha trong dữ liệu
            var parent = GetAll(x =>
                x.TT != null && x.ProjectWorkerVersionID == versionID
                &&
                x.TT.Trim() == parentTT && x.IsDeleted != true).FirstOrDefault();

            // 4. Nếu KHÔNG TÌM THẤY → coi node này là GỐC
            return parent?.ID ?? 0;
        }

    }
}
