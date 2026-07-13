using System;

namespace RERPAPI.Model.Entities
{
    /// <summary>
    /// Bảng khai báo mẫu bước/công đoạn thực hiện (ProjectGateStepTemplate)
    /// </summary>
    public partial class ProjectGateStepTemplate
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
