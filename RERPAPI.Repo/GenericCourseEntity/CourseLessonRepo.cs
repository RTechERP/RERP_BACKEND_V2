using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseLessonRepo : GenericCourseRepo<CourseLesson>
    {
        public CourseLessonRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string ConvertYoutubeToEmbed(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            // Link dạng youtu.be
            if (url.Contains("youtu.be/"))
            {
                var videoId = url.Split("youtu.be/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }

            // Link dạng youtube.com/watch?v=
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var videoID = query["v"];

            if (string.IsNullOrEmpty(videoID))
                return string.Empty;

            return $"https://www.youtube.com/embed/{videoID}";
        }
    }
}