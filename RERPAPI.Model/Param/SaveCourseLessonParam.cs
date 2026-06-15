using RERPAPI.Model.Entities;

namespace RERPAPI.Model.Param
{
    public class SaveCourseLessonParam
    {
        public CourseLesson CourseLesson { get; set; }
        public CourseFile? CoursePdf { get; set; }
        public List<CourseFile>? CourseFiles { get; set; }
    }
}