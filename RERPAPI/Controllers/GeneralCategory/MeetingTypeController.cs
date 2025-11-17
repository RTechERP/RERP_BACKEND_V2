using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingTypeController : ControllerBase
    {
        private readonly MeetingTypeRepo _meetingtypeRepo;

        public MeetingTypeController(MeetingTypeRepo meetingtypeRepo)
        {
            _meetingtypeRepo = meetingtypeRepo;
        }

      
    }
}