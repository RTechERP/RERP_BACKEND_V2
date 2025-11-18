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
    public class AGVBillExportRepo : GenericRepo<AGVBillExport>
    {
        public AGVBillExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetBillCode(int billtype)
        {
            string billCode = "";
            DateTime billDate = DateTime.Now;

            string preCode = "PXK";

            List<AGVBillExport> billExports = GetAll().Where(x => (x.BillCode ?? "").Contains(billDate.ToString("yyMMdd"))).ToList();

            var listCode = billExports.Select(x => new
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


        public APIResponse Validate(AGVBillExportDTO billExport)
        {
            try
            {

                if (billExport.BillType <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Loại phiếu!");
                }

                if (!billExport.BillDate.HasValue)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Ngày tạo phiếu!");
                }

                if (string.IsNullOrWhiteSpace(billExport.BillCode))
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng nhập Mã phiếu!");
                }
                else
                {
                    var billImports = GetAll(x => x.BillCode.Trim().ToLower() == billExport.BillCode.Trim().ToLower() &&
                                                    x.IsDeleted != true &&
                                                    x.ID != billExport.ID);

                    if (billImports.Count() > 0)
                    {
                        return ApiResponseFactory.Fail(null, $"Mã phiếu [{billExport.BillCode}] đã tồn tại. Vui lòng kiểm tra lại!", billImports);
                    }
                }

                if (billExport.EmployeeReceiverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người nhận!");
                }

                if (billExport.EmployeeDeliverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người giao!");
                }

                if (billExport.SupplierSaleID <= 0 && billExport.CustomerID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Nhà cung cấp hoặc Khách hàng!");
                }

                if (billExport.ApproverID <= 0)
                {
                    return ApiResponseFactory.Fail(null, "Vui lòng chọn Người duyệt!");
                }


                //Check chi tiết phiếu xuất
                foreach (var detail in billExport.AGVBillExportDetails)
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
