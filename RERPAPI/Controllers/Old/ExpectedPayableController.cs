using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpectedPayableController : ControllerBase
    {
        private CurrencyRepo _currencyRepo;
        private ExpectedPayableRepo _expectedPayableRepo;
        private ExpectedPayableLogRepo _expectedPayableLogRepo;

        public ExpectedPayableController(CurrencyRepo currencyRepo, ExpectedPayableRepo expectedPayableRepo, ExpectedPayableLogRepo expectedPayableLogRepo)
        {
            _currencyRepo = currencyRepo;
            _expectedPayableRepo = expectedPayableRepo;
            _expectedPayableLogRepo = expectedPayableLogRepo;
        }

        //[RequiresPermission(permissionFunction: "expectedPayable_View")]
        [HttpGet("expected-payable")]
        public async Task<IActionResult> getExpectedPayable(DateTime ds, DateTime de, int supplierSaleId = -1, int employeeId = -1, string filterText = "")
        {
            try
            {
                var param = new
                {
                    DateStart = ds.Date,
                    DateEnd = de.Date,
                    @SupplierSaleID = supplierSaleId,
                    @EmployeeID = employeeId,
                    @FilterText = filterText
                };
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetExpectedPayable", param);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[RequiresPermission(permissionFunction: "expectedPayable_Add")]
        [HttpPost("save-expected-payable")]
        [RequiresPermission("N35")]
        public async Task<IActionResult> Save([FromBody] ExpectedPayable model)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                if (model == null) return Ok(ApiResponseFactory.Fail(null, "Dữ liệu lưu không hợp lệ"));

                if (model.ID <= 0)
                {
                    await _expectedPayableRepo.CreateAsync(model);

                    if (model.BillImportID <= 0)
                    {
                        await _expectedPayableLogRepo.AddLog(
                        model.ID,
                        $"{currentUser.FullName} đã thêm mới dự kiến công nợ số {model.InvoiceNumber}",
                                "Thêm mới"
                            );
                    }
                    else
                    {
                        string log = "";

                        if (model.DomesticPayable > 0)
                        {
                            log += $"thay đổi công nợ trong nước từ [0] thành [{model.DomesticPayable}] \n";
                        }

                        if (model.ForeignPayable > 0)
                        {
                            log += $"thay đổi công nợ nước ngoài từ [0] thành [{model.ForeignPayable}] \n";
                        }

                        if (model.ArisingAmount > 0)
                        {
                            log += $"thay đổi tiền hàng phát sinh từ [0] thành [{model.ArisingAmount}] \n";
                        }

                        if (model.OfficeExpense > 0)
                        {
                            log += $"thay đổi chi phí văn phòng từ [0] thành [{model.OfficeExpense}] \n";
                        }

                        if (model.TaxAmount > 0)
                        {
                            log += $"thay đổi thuế từ [0] thành [{model.TaxAmount}] \n";
                        }

                        if (!string.IsNullOrWhiteSpace(model.Note))
                        {
                            log += $"thay đổi ghi chú từ [] thành [{model.Note}] \n";
                        }

                        if (!String.IsNullOrWhiteSpace(log))
                        {
                            await _expectedPayableLogRepo.AddLog(
                            model.ID,
                            $"{currentUser.FullName} đã cập nhật dự kiến công nợ số {model.InvoiceNumber}. Các thay đổi: \n" + log,
                            "Cập nhật"
                        );
                        }
                    }
                }
                else
                {
                    ExpectedPayable oldModel = _expectedPayableRepo.GetByID(model.ID) ?? new ExpectedPayable();
                    string log = _expectedPayableLogRepo.GenerateLog(oldModel, model);

                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        log = $"{currentUser.FullName} đã cập nhật dự kiến công nợ số {model.InvoiceNumber}. Các thay đổi: \n" + log;
                        await _expectedPayableLogRepo.AddLog(
                            model.ID,
                            log,
                            "Cập nhật"
                        );
                    }

                    await _expectedPayableRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(null, "Saved successfully"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[RequiresPermission(permissionFunction: "expectedPayable_Delete")]
        [HttpPost("delete-expected-payable")]
        [RequiresPermission("N35")]
        public async Task<IActionResult> deleted([FromBody] List<ExpectedPayable> models)
        {
            try
            {
                if (models.Count() == 0) return Ok(ApiResponseFactory.Fail(null, "Dữ liệu lưu không hợp lệ"));

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var model in models)
                {
                    var expectedPayable = _expectedPayableRepo.GetByID(model.ID);
                    if (expectedPayable != null)
                    {
                        expectedPayable.IsDeleted = true;
                        await _expectedPayableRepo.UpdateAsync(expectedPayable);
                        await _expectedPayableLogRepo.AddLog(
                            model.ID,
                            $"${currentUser.FullName} đã xóa dự kiến công nợ số {model.InvoiceNumber}",
                            "Xóa"
                        );
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-expected-payables")]
        [RequiresPermission("N35")]
        public async Task<IActionResult> Saves([FromBody] List<ExpectedPayable> models)
        {
            try
            {
                if (models.Count() == 0) return Ok(ApiResponseFactory.Fail(null, "Dữ liệu lưu không hợp lệ"));

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var model in models)
                {
                    if (model.ID <= 0)
                    {
                        await _expectedPayableRepo.CreateAsync(model);

                        if (model.BillImportID <= 0)
                        {
                            await _expectedPayableLogRepo.AddLog(
                            model.ID,
                            $"{currentUser.FullName} đã thêm mới dự kiến công nợ số {model.InvoiceNumber}",
                                    "Thêm mới"
                                );
                        }
                        else
                        {
                            string log = "";

                            if (model.DomesticPayable > 0)
                            {
                                log += $"thay đổi công nợ trong nước từ [0] thành [{model.DomesticPayable}] \n";
                            }

                            if (model.ForeignPayable > 0)
                            {
                                log += $"thay đổi công nợ nước ngoài từ [0] thành [{model.ForeignPayable}] \n";
                            }

                            if (model.ArisingAmount > 0)
                            {
                                log += $"thay đổi tiền hàng phát sinh từ [0] thành [{model.ArisingAmount}] \n";
                            }

                            if (model.OfficeExpense > 0)
                            {
                                log += $"thay đổi chi phí văn phòng từ [0] thành [{model.OfficeExpense}] \n";
                            }

                            if (model.TaxAmount > 0)
                            {
                                log += $"thay đổi thuế từ [0] thành [{model.TaxAmount}] \n";
                            }

                            if (!string.IsNullOrWhiteSpace(model.Note))
                            {
                                log += $"thay đổi ghi chú từ [] thành [{model.Note}] \n";
                            }

                            if (!String.IsNullOrWhiteSpace(log))
                            {
                                await _expectedPayableLogRepo.AddLog(
                                model.ID,
                                $"{currentUser.FullName} đã cập nhật dự kiến công nợ số {model.InvoiceNumber}. Các thay đổi: \n" + log,
                                "Cập nhật"
                            );
                            }
                        }
                    }
                    else
                    {
                        ExpectedPayable oldModel = _expectedPayableRepo.GetByID(model.ID) ?? new ExpectedPayable();
                        string log = _expectedPayableLogRepo.GenerateLog(oldModel, model);

                        if (!string.IsNullOrWhiteSpace(log))
                        {
                            log = $"{currentUser.FullName} đã cập nhật dự kiến công nợ số {model.InvoiceNumber}. Các thay đổi: \n" + log;
                            await _expectedPayableLogRepo.AddLog(
                                model.ID,
                                log,
                                "Cập nhật"
                            );
                        }

                        await _expectedPayableRepo.UpdateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //271[RequiresPermission(permissionFunction: "expectedPayable_ViewLog")]
        [HttpGet("log-activity")]
        public IActionResult GetLogActivity(int expectedPayableId)
        {
            try
            {
                var data = _expectedPayableLogRepo.GetAll().Where(x => x.ExpectedPayableID == expectedPayableId).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}