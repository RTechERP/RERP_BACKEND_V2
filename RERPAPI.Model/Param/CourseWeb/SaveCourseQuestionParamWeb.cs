using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.CourseWebDTO;
using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.CourseWeb
{
    public partial class SaveCourseQuestionParamWeb
    {
        public int ExamType { get; set; }
        public CourseQuestion Question { get; set; } = new CourseQuestion();
        public List<CourseAnswerWebDTO> Answers { get; set; } = new List<CourseAnswerWebDTO>();
        public List<int> DeleteAnswerIds { get; set; } = new List<int>();
    }
}
