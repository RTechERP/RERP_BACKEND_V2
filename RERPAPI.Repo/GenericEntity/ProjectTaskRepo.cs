using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskRepo : GenericRepo<ProjectTask>
    {

        ProjectRepo _projectRepo;
        ProjectTaskTypeRepo _projectTaskTypeRepo;

        public ProjectTaskRepo(CurrentUser currentUser, ProjectRepo projectRepo, ProjectTaskTypeRepo projectTaskTypeRepo) : base(currentUser)
        {
            _projectRepo = projectRepo;
            _projectTaskTypeRepo = projectTaskTypeRepo;
        }

        public string GenerateProjectItemCode(int projectId)
        {
            try
            {
                var project = _projectRepo.GetByID(projectId);
                if (project.ID <= 0)
                {
                    throw new Exception($"Không có Project nào có ID là :{projectId}");
                }
                var projectItem = GetAll(x => x.Code.StartsWith($"{project.ProjectCode.Trim().ToUpper()}-"));

                string newCode = $"{project.ProjectCode.Trim().ToUpper()}-{projectItem.Count + 1}";
                return newCode.Trim().ToUpper();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}\r\n{ex.ToString()}");
            }
        }

        public string GenerateProjectItemPertionalCode(string employeeCode)
        {
            try
            {
                var projectItem = GetAll(x => x.Code.ToUpper().StartsWith($"{employeeCode.ToUpper().Trim()}-"));
                string newCode = "";
                newCode = $"{employeeCode.Trim().ToUpper()}-{projectItem.Count + 1}";
                return newCode.Trim().ToUpper();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}\r\n{ex.ToString()}");
            }
        }

        public string GenerateProjectTaskCodeTime(int projectTaskType)
        {
            try
            {
                string dateTimeNow = DateTime.Now.ToString("yyMMddhhmmssff");
                var projectTaskTypeItem = _projectTaskTypeRepo.GetByID(projectTaskType);
                string newcode = $"{projectTaskTypeItem.Code}{dateTimeNow}";
                return newcode.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}\r\n{ex.ToString()}");
            }
        }

        //public string UpdateProjectTaskCodeByType(int projectTaskTypeNew, string oldCode)
        //{
        //    var projectTaskTypeNewValue = _projectTaskTypeRepo.GetByID(projectTaskTypeNew);
        //    string newcode = $"{projectTaskTypeNewValue.Code}{oldCode.Trim().Substring(oldCode.Trim().Length - 14,14).Trim()}";
        //    return newcode;
        //}

        public List<ProjectTaskTreeNodeParam> BuldTreeProjectTask(List<spGetProjectTaskTreeParam> list)
        {
            var nodeMap = new Dictionary<int, ProjectTaskTreeNodeParam>();
            var roots = new List<ProjectTaskTreeNodeParam>();


            // Tạo các node và lưu vào map

            foreach (var item in list)
            {
                var node = new ProjectTaskTreeNodeParam
                {
                    Data = item,
                    Leaf = true, // Mặc định là leaf, sẽ cập nhật sau nếu có con
                    Expanded = false,
                    Children = new List<ProjectTaskTreeNodeParam>()
                };
                nodeMap[item.ID] = node;
            }

            // Xây dựng cây

            foreach (var item in list)
            {
                if (item.ParentID.HasValue && nodeMap.ContainsKey(item.ParentID.Value))
                {
                    var parentNode = nodeMap[item.ParentID.Value];
                    parentNode.Children.Add(nodeMap[item.ID]);
                    parentNode.Leaf = false; // Nếu có con thì không phải leaf
                }
                else
                {
                    roots.Add(nodeMap[item.ID]); // Node gốc
                }
            }

            roots = roots.OrderByDescending(x => x.Data.UpdatedDate).ToList();

            return roots;
        }

    } 
}
