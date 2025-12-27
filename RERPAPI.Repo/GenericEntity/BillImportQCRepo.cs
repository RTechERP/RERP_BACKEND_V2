using Microsoft.AspNetCore.Http;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportQCRepo : GenericRepo<BillImportQC>
    {
        EmployeeRepo _employeeRepo;
        ProductSaleRepo _productSaleRepo;
        ProjectRepo _projectRepo;
        EmployeeSendEmailRepo _employeeSendEmailRepo;
        CurrentUser _currentUser;
        public BillImportQCRepo(
            CurrentUser currentUser, 
            EmployeeRepo employeeRepo, 
            ProductSaleRepo productSaleRepo, 
            ProjectRepo projectRepo,
            EmployeeSendEmailRepo employeeSendEmailRepo
            ) : base(currentUser)
        {
            _employeeRepo = employeeRepo;
            _productSaleRepo = productSaleRepo;
            _projectRepo = projectRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
            _currentUser = currentUser;
        }

        public List<T>? DeserializeList<T>(IFormCollection form, string key)
        {
            if (!form.TryGetValue(key, out var value))
                return null;

            return JsonSerializer.Deserialize<List<T>>(
                value!,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        public List<BillImportQCFileDTO> ExtractFiles(IFormCollection form, string fileKey, int fileType)
        {
            var result = new List<BillImportQCFileDTO>();

            var files = form.Files
                .Where(f => f.Name == fileKey)
                .ToList();

            for (int i = 0; i < files.Count; i++)
            {
                var metaKey = $"{fileKey}_meta_{i}";

                if (!form.TryGetValue(metaKey, out var meta))
                    continue;

                var metaObj = JsonSerializer.Deserialize<BillImportQCFileDTO>(
                    meta!,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                result.Add(new BillImportQCFileDTO
                {
                    FileName = files[i].FileName,
                    FileSize = files[i].Length,
                    ContentType = files[i].ContentType,
                    BillImportQCDetailId = metaObj!.BillImportQCDetailId,
                    FileType = fileType,
                    File = files[i] 
                });
            }

            return result;
        }

        public async void SetInforEmail(string emailCCs, string emailEmRequest, int receivedEmID, List<BillImportQCDetail> dtr, string emRequestName, DateTime deadline) 
        {
            string subject = $"YÊU CẦU QC SẢN PHẨM";
            string leaderFullName = _employeeRepo.GetByID(receivedEmID).FullName;

            StringBuilder tableContent = new StringBuilder();

            foreach (var data in dtr)
            {
                var product = _productSaleRepo.GetByID(Convert.ToInt32(data.ProductSaleID));
                var project = _projectRepo.GetByID(Convert.ToInt32(data.ProjectID));

                string Note = TextUtils.ToString(data.Note);
                decimal quantity = TextUtils.ToInt32(Convert.ToInt32(data.Quantity));

                tableContent.Append($"<tr style=\"border: 1px solid black;\">");
                tableContent.Append($"<td style=\"border: 1px solid black; padding: 8px;\">{project.ProjectCode}</td>");
                tableContent.Append($"<td style=\"border: 1px solid black; padding: 8px;\">{product.ProductCode}</td>");
                tableContent.Append($"<td style=\"border: 1px solid black; padding: 8px;\">{product.ProductName}</td>");
                tableContent.Append($"<td style=\"border: 1px solid black; padding: 8px;\">{quantity}</td>");
                tableContent.Append($"<td style=\"border: 1px solid black; padding: 8px;\">{Note}</td>");
                tableContent.Append("</tr>");
            }

            string body = $@"
				<div>
					<p style=""font-weight: bold; color: red;"">[NO REPLY]</p>
					<p> Dear anh {leaderFullName}</p>
					<p> Nhân viên mua {emRequestName} yêu cầu QC {dtr.Count()} sản phẩm sau: </p>
					<p style=""font-weight: bold; color: red;""> Deadline {deadline.Date}</p>
				</div>
				<div style=""margin-top: 30px;"">
					<table style=""border-collapse: collapse; width: 100%;"">
						<tr style=""border: 1px solid black;"">
							<th style=""border: 1px solid black; padding: 8px;"">Dự án</th>
							<th style=""border: 1px solid black; padding: 8px;"">Mã sản phẩm</th>
							<th style=""border: 1px solid black; padding: 8px;"">Tên sản phẩm</th>
							<th style=""border: 1px solid black; padding: 8px;"">Số lượng</th>
							<th style=""border: 1px solid black; padding: 8px;"">Ghi chú</th>
						</tr>
						{tableContent}
					</table>
				</div>
				<div style=""margin-top: 30px;"">
					<p> Thanks </p>
					<p> {emRequestName} </p>
				</div>";

            _employeeSendEmailRepo.SendMail(
                _currentUser.EmployeeID,
                receivedEmID,
                subject,
                body,
                emailCCs
            );
        }
    }
}
