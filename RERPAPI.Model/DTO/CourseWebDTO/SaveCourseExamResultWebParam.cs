using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.CourseWebDTO
{
    public partial class SaveCourseExamResultWebParam
    {
        public int CourseId { get; set; }
        public int ExamType { get; set; }
        public CourseExamResult CourseExamResult { get; set; } = new CourseExamResult();
    }
}
