using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param
{
    public class SaveCourseLessonParam
    {
        public CourseLesson CourseLesson { get; set; }
        public CourseFile? CoursePdf { get; set; }
        public List<CourseFile>? CourseFiles { get; set; }
    }
}
