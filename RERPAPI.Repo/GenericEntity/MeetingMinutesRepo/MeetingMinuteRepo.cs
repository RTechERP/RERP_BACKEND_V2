using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes
{
    public class MeetingMinuteRepo : GenericRepo<MeetingMinute>
    {

        public MeetingMinuteRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
