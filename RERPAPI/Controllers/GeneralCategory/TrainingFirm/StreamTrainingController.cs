using Microsoft.AspNetCore.Mvc;
using RERPAPI.Repo.GenericEntity.MakerTrainingFirm;

namespace RERPAPI.Controllers.GeneralCategory.TrainingFirm
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamTrainingController : Controller
    {
        private readonly MakerTrainingVideoLinkRepo _makerTrainingVideoLinkRepo;

        public StreamTrainingController( MakerTrainingVideoLinkRepo makerTrainingVideoLinkRepo)
        {
            _makerTrainingVideoLinkRepo = makerTrainingVideoLinkRepo;
        }

        [HttpGet("stream-maker-training/{videoId}")]
        public IActionResult StreamByMakerTrainingVideo(int videoId)
        {
            var video = _makerTrainingVideoLinkRepo.GetByID(videoId);
            if (video == null || string.IsNullOrEmpty(video.UrlVideo))
                return NotFound();

            var path = video.UrlVideo;
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
