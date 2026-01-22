using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Param.HRM.Course
{
    public class SaveCourseParam
    {
        public int ID { get; set; }

        public int? STT { get; set; }

        public string? Code { get; set; }

        public string? NameCourse { get; set; }

        public int? CourseCatalogID { get; set; }

        public bool? DeleteFlag { get; set; }

//        public int? FileCourseID { get; set; }

//        public bool? IsPractice { get; set; }

        public decimal? QuestionCount { get; set; }

        public decimal? QuestionDuration { get; set; }

        public decimal? LeadTime { get; set; }

        public int? CourseCopyID { get; set; }

        public int? CourseTypeID { get; set; }

        public List<int>? IdeaIDs { get; set; }
    }
}
