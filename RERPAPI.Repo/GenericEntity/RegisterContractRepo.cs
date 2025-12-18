using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class RegisterContractRepo : GenericRepo<RegisterContract>
    {
        public RegisterContractRepo(CurrentUser currentUser) : base(currentUser)
        {

        }
        public bool Validate(
         RegisterContract data,
         out string message)
        {
            string projectCode = string.Empty;
            message = string.Empty;

            // Kiểm tra null
            if (data == null)
            {
                message = "Không có dữ liệu. Vui lòng kiểm tra lại!";
                return false;
            }

            if (data.TaxCompanyID <= 0)
            {
                message = "Vui lòng nhập tên công ty!";
                return false;
            }
            if (data.ContractTypeID <= 0)
            {
                message = "Vui lòng nhập loại hồ sơ!";
                return false;
            }

            if (!data.RegistedDate.HasValue)
            {
                message = "Vui lòng nhập ngày đăng ký!";
                return false;
            }

            if (data.EmployeeReciveID <= 0)
            {
                message = "Vui lòng nhập người nhận!";
                return false;
            }

            if (data.DocumentTypeID <= 0)
            {
                message = "Vui lòng nhập loại văn bản!";
                return false;
            }

            if (data.DocumentQuantity <= 0)
            {
                message = "Vui lòng nhập số lượng bản!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.DocumentName))
            {
                message = "Vui lòng nhập văn bản!";
                return false;
            }
            return true;
        }

        #region Build Email Body

        /// <summary>
        /// Tạo nội dung email khi có đăng ký MỚI
        /// </summary>
        public string BuildEmailNewContractBody(RegisterContract contract, string registerName, string reciverName, string taxCompanyName, string documentTypeName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; font-size: 13px; line-height: 1.6;'>");
            sb.AppendLine("  <p style='font-weight: bold; color: red;'>[NO REPLY]</p>");
            sb.AppendLine($"  <p>Dear <strong>{reciverName}</strong>,</p>");
            sb.AppendLine("  <div style='margin-top: 20px;'>");
            sb.AppendLine($"    <p>Bạn có <strong style='color: blue;'>đăng ký hợp đồng mới</strong> từ <strong>{registerName}</strong>. Vui lòng xác nhận hoặc hủy đăng ký này.</p>");
            sb.AppendLine("  </div>");

            // Bảng thông tin
            sb.AppendLine("  <table border='1' cellspacing='0' cellpadding='8' style='border-collapse: collapse; width: 100%; margin-top: 20px; font-size: 13px;'>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='width: 30%; font-weight: bold;'>Tên hợp đồng</td>");
            sb.AppendLine($"      <td>{contract.DocumentName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Công ty</td>");
            sb.AppendLine($"      <td>{taxCompanyName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='font-weight: bold;'>Loại hồ sơ</td>");
            sb.AppendLine($"      <td>{documentTypeName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Số lượng bản</td>");
            sb.AppendLine($"      <td>{contract.DocumentQuantity}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='font-weight: bold;'>Ngày đăng ký</td>");
            sb.AppendLine($"      <td>{contract.RegistedDate?.ToString("dd/MM/yyyy")}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Người đăng ký</td>");
            sb.AppendLine($"      <td>{registerName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </table>");

            sb.AppendLine("  <div style='margin-top: 30px;'>");
            sb.AppendLine("    <p>Vui lòng truy cập phần mềm để xác nhận hoặc hủy đăng ký này.</p>");
            sb.AppendLine("    <p>Thanks & Best Regards,<br/>RTC Software Team</p>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }
        #endregion

        #region
        /// <summary>
        /// Tạo nội dung email khi XÁC NHẬN hoặc HỦY
        /// </summary>
        public string BuildEmailApprovalBody(
            RegisterContract contract,
            Employee employeeRegister,
            string approverName,
            string taxCompanyName,
            string documentTypeName,
            string statusText,
            string statusColor,
            string reasonCancel)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Arial, sans-serif; font-size: 13px; line-height: 1.6;'>");
            sb.AppendLine("  <p style='font-weight: bold; color: red;'>[NO REPLY]</p>");
            sb.AppendLine($"  <p>Dear <strong>{employeeRegister.FullName}</strong>,</p>");
            sb.AppendLine("  <div style='margin-top: 20px;'>");
            sb.AppendLine($"    <p>Đăng ký hợp đồng của bạn đã được <strong style='color: {statusColor};'>{statusText}</strong> bởi <strong>{approverName}</strong>.</p>");
            sb.AppendLine("  </div>");

            // Bảng thông tin
            sb.AppendLine("  <table border='1' cellspacing='0' cellpadding='8' style='border-collapse: collapse; width: 100%; margin-top: 20px; font-size: 13px;'>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='width: 30%; font-weight: bold;'>Tên hợp đồng</td>");
            sb.AppendLine($"      <td>{contract.DocumentName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Công ty</td>");
            sb.AppendLine($"      <td>{taxCompanyName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='font-weight: bold;'>Loại hồ sơ</td>");
            sb.AppendLine($"      <td>{documentTypeName}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Số lượng bản</td>");
            sb.AppendLine($"      <td>{contract.DocumentQuantity}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr style='background-color: #f2f2f2;'>");
            sb.AppendLine("      <td style='font-weight: bold;'>Ngày đăng ký</td>");
            sb.AppendLine($"      <td>{contract.RegistedDate?.ToString("dd/MM/yyyy")}</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td style='font-weight: bold;'>Trạng thái</td>");
            sb.AppendLine($"      <td><strong style='color: {statusColor};'>{statusText}</strong></td>");
            sb.AppendLine("    </tr>");

            // Lý do hủy nếu có
            if (!string.IsNullOrWhiteSpace(reasonCancel))
            {
                sb.AppendLine("    <tr style='background-color: #fff9e6;'>");
                sb.AppendLine("      <td style='font-weight: bold; color: red;'>Lý do hủy</td>");
                sb.AppendLine($"      <td style='color: red;'>{reasonCancel}</td>");
                sb.AppendLine("    </tr>");
            }

            sb.AppendLine("  </table>");
            sb.AppendLine("  <div style='margin-top: 30px;'>");
            sb.AppendLine("    <p>Vui lòng truy cập phần mềm để xem chi tiết.</p>");
            sb.AppendLine("    <p>Thanks & Best Regards,<br/>RTC Software Team</p>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }
    }
    #endregion
}


