using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPriceRequestRepo : GenericRepo<ProjectPartlistPriceRequest>
    {
        EmployeeSendEmailRepo _employeeSendEmailRepo;
        EmployeeRepo _employeeRepo;
        CurrencyRepo _currencyRepo;
        CurrentUser _currentUser;
        public ProjectPartlistPriceRequestRepo(CurrentUser currentUser, EmployeeSendEmailRepo employeeSendEmailRepo, EmployeeRepo employeeRepo, CurrencyRepo currencyRepo) : base(currentUser)
        {
            _employeeSendEmailRepo = employeeSendEmailRepo;
            _employeeRepo = employeeRepo;
            _currencyRepo = currencyRepo;
            _currentUser = currentUser;
        }
        public async Task SaveData(ProjectPartlistPriceRequest item)
        {

            if (item.ID > 0)
            {
                await UpdateAsync(item);
            }
            else
            {
                await CreateAsync(item);
            }

        }
        public async Task SendMail(List<MailItemPriceRequestDTO> dataMail)
        {
  
            EmployeeSendEmail sendMail = new EmployeeSendEmail();
            List<Employee> employees = _employeeRepo.GetAll(x=>x.Status==0);
            List<Currency> currencies = _currencyRepo.GetAll(x=>x.IsDeleted==false);

            var grouped = dataMail
                .GroupBy(r => r.EmployeeID)
                .Select(g => new
                {
                    EmployeeID = g.Key,
                    QuoteEmployee = g.Select(x => x.QuoteEmployee).Distinct(),

                    ListQuotePrice = g.Select(x => new
                    {
                        x.ProjectCode,
                        x.ProductCode,
                        x.ProductName,
                        x.Manufacturer,
                        x.Quantity,
                        x.Unit,
                        x.DateRequest,
                        x.Deadline,
                        x.DatePriceQuote,
                        CurrencyType = currencies.FirstOrDefault(c => c.ID == x.CurrencyID)?.Code,
                        x.UnitPrice,
                        x.TotalPrice,
                        x.TotalPriceExchange
                    }).ToList()
                }).ToList();

            foreach (var row in grouped)
            {
                var emp = employees.FirstOrDefault(x => x.ID == row.EmployeeID);

                sendMail.Subject = $"BÁO GIÁ SẢN PHẨM NGÀY: {DateTime.Now:dd/MM/yyyy}";
                sendMail.EmailTo = !string.IsNullOrEmpty(emp?.EmailCongTy) ? emp.EmailCongTy : emp?.EmailCaNhan;
                sendMail.StatusSend = 1;
                sendMail.DateSend = DateTime.Now;
                sendMail.EmployeeID = _currentUser.EmployeeID;
                sendMail.Receiver = row.EmployeeID;
                sendMail.Body = $@"
                    <div style='font-family: Arial; font-size: 14px;'>
                        Dear <b>{emp?.FullName ?? "Không rõ"}</b>,<br/><br/>
                        Nhân viên <b>{string.Join(", ", row.QuoteEmployee)}</b> 
                        đã báo giá danh sách sản phẩm sau:<br/><br/>
                        <table border='1' cellspacing='0' cellpadding='5' style='border-collapse:collapse; width:100%;'>
                            <thead style='background-color:#f2f2f2;'>
                                <tr>
                                    <th>Dự án</th>
                                    <th>Mã sản phẩm</th>
                                    <th>Tên sản phẩm</th>
                                    <th>Hãng</th>
                                    <th>Số lượng / Đơn vị</th>
                                    <th>Đơn giá / Loại tiền</th>
                                    <th>Thành tiền chưa VAT</th>
                                    <th>Thành tiền quy đổi (VND)</th>
                                    <th>Ngày yêu cầu</th>
                                    <th>Deadline</th>
                                    <th>Ngày báo giá</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", row.ListQuotePrice.Select(d => $@"
                                    <tr>
                                        <td>{d.ProjectCode}</td>
                                        <td>{d.ProductCode}</td>
                                        <td>{d.ProductName}</td>
                                        <td>{d.Manufacturer}</td>
                                        <td style='text-align:right;'>{d.Quantity} {d.Unit}</td>
                                        <td style='text-align:right;'>{d.UnitPrice} {d.CurrencyType}</td>
                                        <td style='text-align:right;'>{d.TotalPrice:N2} VND</td>
                                        <td style='text-align:right;'>{d.TotalPriceExchange:N2} VND</td>
                                        <td style='text-align:center;'>{d.DateRequest?.ToString("dd/MM/yyyy HH:mm")}</td>
                                        <td style='text-align:center;'>{d.Deadline?.ToString("dd/MM/yyyy")}</td>
                                        <td style='text-align:center;'>{d.DatePriceQuote?.ToString("dd/MM/yyyy HH:mm")}</td>
                                    </tr>
                                "))}
                            </tbody>
                        </table>
                        <br/>Trân trọng.
                    </div>";

                await _employeeSendEmailRepo.CreateAsync(sendMail);
            }

        }
    }
}
