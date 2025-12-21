using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NPOI.SS.Formula.Functions;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Text;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountingContractController : ControllerBase
    {
        private readonly AccountingContractRepo _accountingContractRepo;
        private readonly AccountingContractLogRepo _accountingContractLogRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly SupplierSaleRepo _supplierSaleRepo;
        private readonly CurrentUser _currentUser;
        private readonly AccountingContractTypeRepo _accountingContractTypeRepo;
        public AccountingContractController(AccountingContractRepo accountingContractRepo, AccountingContractLogRepo accountingContractLogRepo, CurrentUser currentUser, CustomerRepo customerRepo, SupplierSaleRepo supplierSaleRepo, AccountingContractTypeRepo accountingContractTypeRepo)
        {
            _accountingContractRepo = accountingContractRepo;
            _accountingContractLogRepo = accountingContractLogRepo;
            _currentUser = currentUser;
            _customerRepo = customerRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _accountingContractTypeRepo = accountingContractTypeRepo;
        }

        [HttpGet("get-accouting-contract-types")]
        public IActionResult GetAccountingContractTypes()
        {
            try
            {
                var data = _accountingContractTypeRepo.GetAll().OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadEmployees()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Keyword", "@Status" },
                                 new object[] { "", 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-contract")]
        public IActionResult LoadContract()
        {
            try
            {
                var list = _accountingContractRepo.GetAll(x => x.IsDelete == false).OrderByDescending(x => x.DateInput).ToList();
                list.Insert(0, new AccountingContract() { ID = -1, ContractNumber = "HĐ cha"});
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var data = _customerRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-suppliers")]
        public IActionResult GetSuppliers()
        {
            try
            {
                var data = _supplierSaleRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-accounting-contracts")] 
        public IActionResult GetAccountingContracts(int page, int size, DateTime dateStart, DateTime dateEnd, int customerId, int supplierId, int isReceivedContract, int isComingExpired, string keyword = "")
        {
            try
            {
                int company = 0;
                int group = 0;
                int type = 0;
                int employeeId = 0;
                int isReceivedContractIndex = isReceivedContract - 1;
                int isComingExpiredIndex = isComingExpired - 1;
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetAccountingContract", 
                    new string[] { "@DateStart", "@DateEnd", "@Company", "@ContractGroup", "@AccountingContractTypeID", "@CustomerID", "@SupplierSaleID",
                                   "@EmployeeID", "@IsReceivedContract","@IsComingExpired", "@Keyword", "@PageNumber", "@PageSize"},
                    new object[] { dateStart, dateEnd, company, group, type, customerId, supplierId, employeeId, isReceivedContractIndex, isComingExpiredIndex, keyword, page, size });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-accounting-contracts-file")]
        public IActionResult GetAccountingContractFile(int accountingContractId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetAccountingContractFile",
                    new string[] { "@AccountingContractID" },
                    new object[] { accountingContractId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("approval")]
        public IActionResult Approval(AccountingContractApprovalDTO dto)
        {
            try
            {
                string isApprovedText = dto.IsApproved ? "duyệt" : "hủy duyệt";
                if(dto.approvalContractIds == null || dto.approvalContractIds.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Chưa có hợp đồng nào để {isApprovedText}"));
                }
                foreach (int id in dto.approvalContractIds)
                {
                    if(id <= 0)
                    {
                        continue;
                    }
                    AccountingContract model = _accountingContractRepo.GetByID(id);
                    string approved = model.IsApproved == true ? "Duyệt hợp đồng" : "Hủy/Chưa duyệt hợp đồng";

                    AccountingContractLog log = new AccountingContractLog()
                    {
                        AccountingContractID = id,
                        DateLog = DateTime.Now,
                        IsApproved = dto.IsApproved,
                        ContentLog = $"TÌNH TRẠNG HỢP ĐỒNG:\n" +
                                        $"từ {approved}\n" +
                                        $"thành {isApprovedText} hợp đồng"
                    };
                    _accountingContractLogRepo.Create(log);

                    model.IsApproved = dto.IsApproved;
                    _accountingContractRepo.Update(model);
                }

                return Ok(ApiResponseFactory.Success(null, dto.IsApproved ? "Duyệt thành công" : "Hủy duyệt thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-accounting-contract")]
        public IActionResult DeleteAccountingContract(int accountingContractId)
        {
            try
            {
                var model = _accountingContractRepo.GetByID(accountingContractId);
                if (_currentUser.LoginName.Trim() != model.CreatedBy.Trim() || _currentUser.IsAdmin == false )
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa"));
                }    
                if(model.IsApproved == true)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Hợp đồng {model.ContractNumber} đã được duyệt. Vui lòng hủy duyệt trước"));
                }
                model.IsDelete = true;
                _accountingContractRepo.Update(model);
                return Ok(ApiResponseFactory.Success(null, "Xóa hợp đồng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("cancel-contract")]
        public IActionResult CancelContract(int accountingContractId)
        {
            try
            {
                var model = _accountingContractRepo.GetByID(accountingContractId);
                if (model.IsApproved == true)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Hợp đồng {model.ContractNumber} đã được duyệt. Vui lòng hủy duyệt trước"));
                }

                if(model.IsReceivedContract == false)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Hợp đồng {model.ContractNumber} chưa nhận chứng từ. Không thể hủy nhận"));
                }    

                AccountingContractLog log = new AccountingContractLog()
                {
                    AccountingContractID = model.ID,
                    DateLog = DateTime.Now,
                    IsReceivedContract = false,
                    ContentLog = $"NGÀY TRẢ HỒ SƠ GỐC:\n" +
                                  $"từ {TextUtils.ToString(model.DateReceived.Value)}\n" +
                                  $"thành \n\n" +
                                  $"NHẬN CHỨNG TỪ GỐC:\n" +
                                  $"từ Đã nhận hồ sơ gốc\n" +
                                  $"thành Huỷ/Chưa nhận hồ sơ gốc"
                };
                _accountingContractLogRepo.Create(log);

                model.DateReceived = null;
                model.IsReceivedContract = false;
                _accountingContractRepo.Update(model);

                return Ok(ApiResponseFactory.Success(null, "Hủy chứng từ hợp đồng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-accounting-contract-detail")]
        public IActionResult GetAccountingContractDetail(int accountingContractId)
        {
            try
            {
                var data = _accountingContractRepo.GetByID(accountingContractId);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveAsync(AccountingContractSaveDTO req)
        {
            Validate(req);
            try
            {
                AccountingContract contract;
                AccountingContract oldContract = null;

                if (req.accountingContract.ID > 0)
                {
                    oldContract = _accountingContractRepo.GetByID(req.accountingContract.ID);
                    if (oldContract == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Hợp đồng không tồn tại"));

                    contract = oldContract;
                }
                else
                {
                    contract = new AccountingContract();
                }

                contract.DateInput = req.accountingContract.DateInput;
                contract.Company = req.accountingContract.Company;
                contract.ContractGroup = req.accountingContract.ContractGroup;
                contract.Unit = req.accountingContract.Unit?.Trim().ToUpper();

                contract.AccountingContractTypeID = req.accountingContract.AccountingContractTypeID;
                contract.CustomerID = req.accountingContract.CustomerID;
                contract.SupplierSaleID = req.accountingContract.SupplierSaleID;

                contract.ContractNumber = req.accountingContract.ContractNumber?.Trim();
                contract.ContractValue = req.accountingContract.ContractValue;

                contract.DateExpired = req.accountingContract.DateExpired;
                contract.DateIsApprovedGroup = req.accountingContract.DateIsApprovedGroup;

                contract.EmployeeID = req.accountingContract.EmployeeID;
                contract.ContractContent = req.accountingContract.ContractContent?.Trim();
                contract.ContentPayment = req.accountingContract.ContentPayment?.Trim();
                contract.Note = req.accountingContract.Note?.Trim();

                contract.ParentID = req.accountingContract.ParentID <= 0 ? 0 : req.accountingContract.ParentID;
                contract.DateReceived = req.accountingContract.DateReceived;
                contract.QuantityDocument = req.accountingContract.QuantityDocument;
                contract.IsReceivedContract = req.accountingContract.DateReceived.HasValue;
                contract.DateContract = req.accountingContract.DateContract;

                // SAVE
                if (req.accountingContract.ID > 0)
                {
                    await _accountingContractRepo.UpdateAsync(contract);
                    SaveLog(oldContract, contract);
                }
                else
                {
                    contract.ID = await _accountingContractRepo.CreateAsync(contract);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        private void SaveLog(AccountingContract oldC, AccountingContract newC)
        {
            var compare = _accountingContractRepo.DeepEquals(oldC, newC);
            bool equal = (bool)compare.GetType().GetProperty("equal").GetValue(compare);

            if (equal) return;

            var props = (List<string>)compare.GetType().GetProperty("property").GetValue(compare);
            var content = new StringBuilder();

            foreach (var prop in props)
            {
                if (prop == "Note") continue;

                content.AppendLine($"{prop.ToUpper()}:");
                content.AppendLine($"từ {oldC.GetType().GetProperty(prop)?.GetValue(oldC)}");
                content.AppendLine($"thành {newC.GetType().GetProperty(prop)?.GetValue(newC)}");
                content.AppendLine();
            }

            _accountingContractLogRepo.Create(new AccountingContractLog
            {
                AccountingContractID = newC.ID,
                DateLog = DateTime.Now,
                IsReceivedContract = newC.IsReceivedContract,
                ContentLog = content.ToString()
            });
        }


        private void Validate(AccountingContractSaveDTO req)
        {
            var c = req.accountingContract;
            if (req.accountingContract.Company <= 0)
                throw new Exception("Vui lòng nhập Công ty");

            if (req.accountingContract.ContractGroup <= 0)
                throw new Exception("Vui lòng nhập Phân loại HĐ chính");

            if (req.accountingContract.AccountingContractTypeID <= 0)
                throw new Exception("Vui lòng nhập Loại HĐ");

            if (req.accountingContract.ContractGroup == 1 && (!req.accountingContract.SupplierSaleID.HasValue || req.accountingContract.SupplierSaleID <= 0))
                throw new Exception("Vui lòng nhập Nhà cung cấp");

            if (req.accountingContract.ContractGroup == 2 && (!req.accountingContract.CustomerID.HasValue || req.accountingContract.CustomerID <= 0))
                throw new Exception("Vui lòng nhập Khách hàng");

            if (string.IsNullOrWhiteSpace(req.accountingContract.ContractNumber))
                throw new Exception("Vui lòng nhập Số HĐ/PL");

            if (req.accountingContract.EmployeeID <= 0)
                throw new Exception("Vui lòng nhập NV phụ trách");

            if (string.IsNullOrWhiteSpace(req.accountingContract.ContractContent))
                throw new Exception("Vui lòng nhập Nội dung HĐ");

            if (string.IsNullOrWhiteSpace(req.accountingContract.ContentPayment))
                throw new Exception("Vui lòng nhập Nội dung thanh toán");
            //Validate theo loại hợp đồng
            var contractType = _accountingContractTypeRepo.GetByID(c.AccountingContractTypeID ?? 0);
            if (contractType == null)
                throw new Exception("Loại hợp đồng không tồn tại");

            if (contractType.IsContractValue == true)
            {
                if (c.ContractValue <= 0)
                    throw new Exception("Vui lòng nhập Giá trị HĐ");

                if (string.IsNullOrWhiteSpace(c.Unit))
                    throw new Exception("Vui lòng nhập ĐVT");
            }

            // Validate nhận chứng từ
            if (c.DateReceived.HasValue)
            {
                if (c.QuantityDocument <= 0)
                    throw new Exception("Vui lòng nhập SL hồ sơ");
            }

            // Validate update cần NOTE
            if (c.ID > 0)
            {
                var oldContract = _accountingContractRepo.GetByID(c.ID);
                if (oldContract != null)
                {
                    var compare = _accountingContractRepo.DeepEquals(oldContract, c);
                    bool equal = (bool)compare.GetType().GetProperty("equal").GetValue(compare);

                    if (!equal && string.IsNullOrWhiteSpace(c.Note))
                    {
                        throw new Exception("Bạn đã thay đổi thông tin. Vui lòng nhập Nội dung thay đổi");
                    }
                }
            }
        }

        public class AccountingContractApprovalDTO
        {
            public bool IsApproved { get; set; }
            public List<int>? approvalContractIds { get; set; }
        }

        public class AccountingContractSaveDTO
        {
            public AccountingContract accountingContract { get; set; }


        }
    }
}
