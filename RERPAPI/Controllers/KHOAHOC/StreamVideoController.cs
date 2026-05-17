using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamVideoController : ControllerBase
    {
        private readonly CourseLessonRepo _courseLessonRepo;
        public StreamVideoController(CourseLessonRepo courseLessonRepo)
        {
            _courseLessonRepo = courseLessonRepo;
        }

        [HttpGet("stream/{lessonId}")]
        public IActionResult StreamByLesson(int lessonId)
        {
            var lesson = _courseLessonRepo.GetByID(lessonId);
            if (lesson == null || string.IsNullOrEmpty(lesson.VideoURL))
                return NotFound();

            var path = lesson.VideoURL;
            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(
                path,
                "video/mp4",
                enableRangeProcessing: true
            );
        }
    }
}
