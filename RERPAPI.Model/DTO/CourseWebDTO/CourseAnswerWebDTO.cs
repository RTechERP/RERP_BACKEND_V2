using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO.CourseWebDTO
{
    public class CourseAnswerWebDTO : CourseAnswer
    {
        public bool IsRightAnswer { get; set; }
    }
}
