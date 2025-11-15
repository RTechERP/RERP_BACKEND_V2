using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Warehouses.AGV;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVBillImportRepo : GenericRepo<AGVBillImport>
    {
        public AGVBillImportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetBillCode(int billtype)
        {
            string billCode = "";

            DateTime billDate = DateTime.Now;

            string preCode = "PNK";
            if (billtype == 3) preCode = "PT";
            List<AGVBillImport> billImports = GetAll().Where(x => (x.BillCode ?? "").Contains(billDate.ToString("yyMMdd"))).ToList();

            var listCode = billImports.Select(x => new
            {
                ID = x.ID,
                Code = x.BillCode,
                STT = string.IsNullOrWhiteSpace(x.BillCode) ? 0 : Convert.ToInt32(x.BillCode.Substring(x.BillCode.Length - 3)),
            }).ToList();

            string numberCodeText = "000";
            int numberCode = listCode.Count <= 0 ? 0 : listCode.Max(x => x.STT);
            numberCodeText = (++numberCode).ToString();
            while (numberCodeText.Length < 3)
            {
                numberCodeText = "0" + numberCodeText;
            }

            billCode = $"{preCode}{billDate.ToString("yyMMdd")}{numberCodeText}";

            return billCode;
        }


        public APIResponse Validate(AGVBillImportDTO billImport)
        {
            try
            {

                if (billImport.BillType <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Loại phiếu!");
                }

                if (!billImport.BillDate.HasValue)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Ngày tạo phiếu!");
                }

                if (string.IsNullOrWhiteSpace(billImport.BillCode))
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Mã phiếu!");
                }
                else
                {
                    var billImports = GetAll(x => x.BillCode.Trim().ToLower() == billImport.BillCode.Trim().ToLower() &&
                                                    x.IsDeleted != true &&
                                                    x.ID != billImport.ID);

                    if (billImports.Count() > 0)
                    {
                        return ApiResponseFactory.Fail(null, $"Mã phiếu [{billImport.BillCode}] đã tồn tại. Vui lòng kiểm tra lại!", billImports);
                    }
                }

                if (billImport.EmployeeReceiverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người nhận!");
                }

                if (billImport.EmployeeDeliverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người giao!");
                }

                if (billImport.SupplierSaleID <= 0 && billImport.CustomerID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Nhà cung cấp hoặc Khách hàng!");
                }

                if (billImport.ApproverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người duyệt!");
                }


                //Check chi tiết phiếu nhập
                foreach (var detail in billImport.AGVBillImportDetails)
                {
                    if (detail.AGVProductID <= 0)
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng chọn Sản phẩm!");
                    }

                    if (detail.Quantity <= 0)
                    {
                        return ApiResponseFactory.Fail(null, "Vui lòng nhập Số lượng!");
                    }
                }

                return ApiResponseFactory.Success(null, "");
            }
            catch (Exception ex)
            {
                return ApiResponseFactory.Fail(ex, ex.Message);
            }
        }
    }
}
