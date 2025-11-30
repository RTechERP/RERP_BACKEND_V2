using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.KPISALE
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BonusCoefficientController : ControllerBase
    {
        private readonly GroupSalesUserRepo _groupSalesUserRepo;
        private readonly UserRepo _userRepo;
        private readonly BonusRuleIndexRepo _bonusRuleIndexRepo;
        private readonly SaleUserTypeRepo _saleUserTypeRepo;
        private readonly SalesPerformanceRankingRepo _salesPerformanceRankingRepo;
        public BonusCoefficientController(GroupSalesUserRepo groupSalesUserRepo, UserRepo userRepo, BonusRuleIndexRepo bonusRuleIndexRepo, SaleUserTypeRepo saleUserTypeRepo, SalesPerformanceRankingRepo salesPerformanceRankingRepo)
        {
            _groupSalesUserRepo = groupSalesUserRepo;
            _userRepo = userRepo;
            _bonusRuleIndexRepo = bonusRuleIndexRepo;
            _saleUserTypeRepo = saleUserTypeRepo;
            _salesPerformanceRankingRepo = salesPerformanceRankingRepo;
        }

        [HttpGet("get-users-group-sales")]

        public IActionResult loadUsers()
        {
            try
            {
                var users = _userRepo.GetAll();
                var groupSales = _groupSalesUserRepo.GetAll();

                var result = from g in groupSales
                             join u in users on g.UserID equals u.ID
                             select new
                             {
                                 u.FullName,
                                 u.ID,
                                 g.SaleUserTypeID
                             };
                return Ok(ApiResponseFactory.Success(result,""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-bonus-coefficient")]
        public IActionResult loadBonusCoefficient(int quarter, int year, int groupId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetBonusCoefficient",
                     new string[] { "@Quy", "@Year", "@GroupID" },
                     new object[] { quarter, year, groupId });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-bonus-rules")]
        public IActionResult loadBonusRules(int groupId)
        {
            try
            {
                var bonusRules = _bonusRuleIndexRepo.GetAll();
                var saleUserTypes = _saleUserTypeRepo.GetAll();
                var data = from b in bonusRules
                           join s in saleUserTypes on b.SaleUserTypeID equals s.ID
                           where b.GroupSalesID == groupId
                           select new
                           {
                                b.PercentBonus,
                                s.SaleUserTypeCode,
                           };
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-bonus-coefficient")]
        public async Task<IActionResult> saveBonusCoefficient([FromBody] List<SalesPerformanceRanking> request)
        {
            try
            {
                foreach (SalesPerformanceRanking s in request)
                {
                    SalesPerformanceRanking model = await _salesPerformanceRankingRepo.GetByIDAsync(s.ID) ?? new SalesPerformanceRanking();
                    model.BonusSales = s.BonusSales;
                    model.BonusAcc = s.BonusAcc;
                    model.BonusAdd = s.BonusAdd;
                    model.BonusRank = s.BonusRank;
                    model.TotalBonus = s.TotalBonus;
                    model.UserID = s.UserID;
                    model.Quy = s.Quy;
                    model.Year  = DateTime.Now.Year;
                    if (model.ID > 0)
                    {
                        await _salesPerformanceRankingRepo.UpdateAsync(model);
                    }
                    else
                    {
                        await _salesPerformanceRankingRepo.CreateAsync(model);
                    }
                }    
                return Ok(ApiResponseFactory.Success(null, "Save successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
