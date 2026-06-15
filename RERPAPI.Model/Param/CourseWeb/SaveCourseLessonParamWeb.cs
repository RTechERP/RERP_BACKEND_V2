namespace RERPAPI.Model.Param.CourseWeb
{
    public class SaveCourseLessonParamWeb
    {
        public RERPAPI.Model.Entities.RTCCourse.CourseLesson CourseLesson { get; set; }
        public RERPAPI.Model.Entities.RTCCourse.CourseFile? CoursePdf { get; set; }
        public List<RERPAPI.Model.Entities.RTCCourse.CourseFile>? CourseFiles { get; set; }
    }
}