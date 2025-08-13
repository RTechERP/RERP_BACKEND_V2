using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Context
{
    public class RTCContextWithAccessor:RTCContext
    {
        public RTCContextWithAccessor(DbContextOptions<RTCContext> options,IHttpContextAccessor accessor) : base(options, accessor)
        {

        }
    }
}
