using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCRepo : GenericRepo<PONCC>
    {
        private PONCCDetailRepo _pONCCDetailRepo;
        private PONCCRulePayRepo _repoRulePay;
        private DocumentImportPONCCRepo _repoDocImport;
        private PONCCDetailRequestBuyRepo _repoDetailRequest;
        private BillImportDetailRepo _repoBillImport;
        private PONCCDetailLogRepo _repoDetailLog;
        private SupplierSaleRepo _supplierSaleRepo;
        private ProjectPartlistPurchaseRequestRepo _prjPartListRepo;
        CurrentUser _currentUser;

        private string[] ones = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        private string[] tens = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        private string[] thousandsGroups = { "", " thousand", " million", " billion" };

        private readonly string[] ChuSo = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private readonly string[] UNITS = { "", "nghìn", "triệu", "tỉ" };
        private readonly string[] DIGITS = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

        private List<UnitCurrency> UNIT_CURRENCY = new List<UnitCurrency>() {
            new UnitCurrency{Name = "VND", Description = new { nameenglish = "Vietnamese dong", unitenglish = "dong", namevietnamese = "đồng", unitvietnamese = "đồng"} },
            new UnitCurrency{Name = "USD", Description = new { nameenglish = "Dollars", unitenglish = "cent", namevietnamese = "đô", unitvietnamese = "cent"} },
            new UnitCurrency{Name = "EUR", Description = new { nameenglish = "Euro", unitenglish = "cent", namevietnamese = "euro", unitvietnamese = "cent"} },
            new UnitCurrency{Name = "RMB", Description = new { nameenglish = "Renminbi", unitenglish = "bad", namevietnamese = "nhân dân tệ", unitvietnamese = "bad"} },
            new UnitCurrency{Name = "JPY", Description = new { nameenglish = "Japanese yen", unitenglish = "sen", namevietnamese = "yên", unitvietnamese = "sen"} },
        };

        public PONCCRepo(CurrentUser currentUser,
            PONCCDetailRepo pONCCDetailRepo,
            PONCCRulePayRepo pONCCRulePayRepo,
            DocumentImportPONCCRepo documentImportPONCCRepo,
            PONCCDetailRequestBuyRepo pONCCDetailRequestBuyRepo,
            BillImportDetailRepo billImportDetailRepo,
            PONCCDetailLogRepo pONCCDetailLogRepo,
            ProjectPartlistPurchaseRequestRepo prjPartListRepo,
            SupplierSaleRepo supplierSaleRepo) : base(currentUser)
        {

            _currentUser = currentUser;
            _pONCCDetailRepo = pONCCDetailRepo;
            _repoRulePay = pONCCRulePayRepo;
            _repoDocImport = documentImportPONCCRepo;
            _repoDetailRequest = pONCCDetailRequestBuyRepo;
            _repoBillImport = billImportDetailRepo;
            _repoDetailLog = pONCCDetailLogRepo;
            _supplierSaleRepo = supplierSaleRepo;
            _prjPartListRepo = prjPartListRepo;
        }

        public PONCCRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool Validate(PONCCDTO pONCCDTO, out string message)
        {
            message = "";
            string pattern = @"^[a-zA-Z0-9_-]+$";
            Regex regex = new Regex(pattern);
            PONCC model = pONCCDTO.poncc;
            var lstPONCCDetail = pONCCDTO.lstPONCCDetail;

            #region validate cho poncc bao gôm rulepayId
            if (model.ID > 0)
            {
                if (!(model.Status == 0 || model.Status == 5) && !(_currentUser.EmployeeID == 178 || _currentUser.IsAdmin))
                {
                    var oldStatus = model.Status == 0 ? "Đang tiến hành" : "Đã Y/c nhập kho";
                    message = $"Trạng thái PONCC [{oldStatus}]! Không được sửa";
                    return false;
                }
            }

            if (model?.SupplierSaleID <= 0)
            {
                message = "Vui lòng chọn nhà cung cấp!";
                return false;
            }

            if (string.IsNullOrEmpty(model.POCode?.Trim()))
            {
                message = "Vui lòng nhập Mã PO NCC";
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(model.POCode.Trim());
                if (!isCheck)
                {
                    message = "Mã PO NCC chỉ chứa chữ cái tiếng Anh và số!";
                    return false;
                }
            }

            // Kiểm tra số đơn hàng
            if (string.IsNullOrEmpty(model.BillCode?.Trim()))
            {
                message = "Vui lòng nhập Số đơn hàng";
                return false;
            }
            else
            {
                bool isCheck = regex.IsMatch(model.BillCode.Trim());
                if (!isCheck)
                {
                    message = "Số đơn hàng chỉ chứa chữ cái tiếng Anh và số!";
                    return false;
                }
            }

            var existingPO = GetAll(p => p.POCode == model.POCode.Trim() && p.ID != model.ID && p.IsDeleted != true).FirstOrDefault();
            if (existingPO != null)
            {
                message = $"Mã PO NCC [{model.POCode.Trim()}] đã tồn tại!";
                return false;
            }

            var existingBill = GetAll(p => p.BillCode == model.BillCode.Trim() && p.ID != model.ID && p.IsDeleted != true && p.POType == model.POType).FirstOrDefault();
            if (existingBill != null)
            {
                message = $"Số đơn hàng [{model.BillCode.Trim()}] đã tồn tại!";
                return false;
            }

            if (model.EmployeeID <= 0)
            {
                message = "Vui lòng nhập NV mua hàng";
                return false;
            }

            if (pONCCDTO.RulePayID <= 0)
            {
                message = "Vui lòng chọn điều khoản thanh toán!";
                return false;
            }

            // Kiểm tra công ty
            if (model.Company <= 0)
            {
                message = "Vui lòng nhập Công ty";
                return false;
            }

            // Kiểm tra ngày đơn hàng
            if (!model.RequestDate.HasValue)
            {
                message = "Vui lòng nhập Ngày đơn hàng";
                return false;
            }

            // Kiểm tra tổng tiền PO
            if (model.TotalMoneyPO <= 0)
            {
                message = "Tổng tiền PO cần lớn hơn 0. Vui lòng chọn hoặc chỉnh sửa tổng tiền sản phẩm mua!";
                return false;
            }

            // Kiểm tra loại tiền
            if (model.CurrencyID <= 0)
            {
                message = "Vui lòng nhập Loại tiền";
                return false;
            }

            // Kiểm tra ngày giao hàng
            if (!model.DeliveryDate.HasValue)
            {
                message = "Vui lòng nhập Ngày giao hàng";
                return false;
            }
            #endregion

            if (pONCCDTO.lstPrjPartlistPurchaseRequest.Count() > 0)
            {
                if ((bool)pONCCDTO.IsCheckTotalMoneyPO)
                {
                    decimal totalPrice = (decimal)pONCCDTO.poncc.TotalMoneyPO;
                    decimal totalPriceRequest = pONCCDTO.lstPrjPartlistPurchaseRequest.Sum(x =>
                    (TextUtils.ToDecimal(x.Quantity) * TextUtils.ToDecimal(x.UnitPrice)));

                    if (totalPrice > totalPriceRequest)
                    {
                        message = $@"Tổng Thành tiền không được lớn hơn tổng Thành tiền duyệt mua ({totalPriceRequest.ToString("n2")}). 
                                Vui lòng kiểm tra lại!";
                        return false;
                    }
                }
                else
                {
                    foreach (var item in lstPONCCDetail)
                    {
                        int purchaseRequestId = TextUtils.ToInt32(item.ProjectPartlistPurchaseRequestID);
                        ProjectPartlistPurchaseRequest purchaseRequest = _prjPartListRepo.GetByID(purchaseRequestId);
                        if (purchaseRequest == null || purchaseRequest.ID <= 0) continue;

                        decimal unitPrice = TextUtils.ToDecimal(item.UnitPrice);
                        int stt = TextUtils.ToInt32(item.STT);

                        if (unitPrice > purchaseRequest.UnitPrice)
                        {
                            message = $"Đơn giá mua không được lớn hơn đơn giá duyệt mua.\nVui lòng kiểm tra lại (Stt: {stt})";
                            return false;
                        }
                    }
                }
            }

            //check lại sau 
            if ((bool)model.OrderQualityNotMet && string.IsNullOrWhiteSpace(model.ReasonForFailure))
            {
                message = $"Vui lòng nhập lý do không đạt chất lượng!";
                return false;
            }

            return true;
        }

        public bool ValidateApproved(PONCC poncc, out string message)
        {
            message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";

                return false;
            }
            return true;
        }

        public bool ValidateDelete(PONCC poncc, out string message)
        {
            message = "";

            if (poncc == null || poncc.ID <= 0)
            {
                message = "Không tìm thấy thông tin PO!";

                return false;
            }

            // Kiểm tra nếu PO đã được duyệt thì không được xóa
            if (poncc.IsApproved == true)
            {
                message = "PO đã được duyệt, không thể xóa!";

                return false;
            }


            return true;
        }

        //public int GetSTT()
        //{
        //    var maxSTT = GetAll(p => p.IsDeleted != true).Max(p => p.STT);
        //    return maxSTT.HasValue ? maxSTT.Value + 1 : 1;
        //}
        public string GetPOCode(int supplierSaleID)
        {
            string code = "";
            SupplierSale supplier = _supplierSaleRepo.GetByID(supplierSaleID);
            if (supplier != null && supplier.ID > 0)
            {
                code = $"{DateTime.Now.ToString("MMyyyy")}-{supplier.CodeNCC}-";
                string currentCode = string.Empty;
                var data = SQLHelper<dynamic>.ProcedureToList("spGetPOCodeInPONCC",
                    new string[] { "@Value" },
                    new object[] { code });
                var dt = SQLHelper<dynamic>.GetListData(data, 0);
                if (dt.Count > 0)
                    currentCode = (dt[0].CurrentCode ?? "").ToString();
                int stt = 1;
                if (!string.IsNullOrEmpty(currentCode.Trim()))
                {
                    stt = TextUtils.ToInt32(currentCode.Substring(code.Length));
                    stt++;
                }

                string sttText = stt.ToString();
                while (sttText.Length < 3)
                {
                    sttText = $"0{sttText}";
                }
                code += sttText;
            }

            return code;
        }
        public string GetBillCode(PONCC model)
        {
            string code = model.POType == 0 ? "DMH" : "DEMO";

            var listPO = GetAll(x => !string.IsNullOrEmpty(x.BillCode) && x.BillCode.StartsWith(code))
                .Select(x => new
                {
                    ID = x.ID,
                    BillCode = x.BillCode,
                    STT = TextUtils.ToInt32(x.BillCode.Substring(code.Length))
                }).ToList();
            int stt = listPO.Count <= 0 ? 0 : listPO.Max(x => x.STT);
            stt++;

            string sttText = stt.ToString();
            while (sttText.Length < 5)
            {
                sttText = $"0{sttText}";
            }
            code += sttText;

            return code;
        }
        public async Task UpdatePurchaseRequest(int projectPartlistPurchaseRequestID, int supplierSaleID)
        {
            ProjectPartlistPurchaseRequest request = _prjPartListRepo.
                        GetByID(projectPartlistPurchaseRequestID);

            if (request == null) return;
            if (supplierSaleID == request.SupplierSaleID) return;

            request.SupplierSaleID = supplierSaleID;
            if (request.ID > 0)
            {
                await _prjPartListRepo.UpdateAsync(request);
            }
        }
        public async Task UpdateTinhHinhDonHang(BillImportApprovedDTO lstModels, bool isapproved)
        {
            int PONCCID = 0;
            if (lstModels.billImportDetails.Count <= 0) return;
            foreach (var item in lstModels.billImportDetails)
            {
                int PONCCDetailID = item.PONCCDetailID ?? 0;
                if (PONCCDetailID <= 0) return;
                PONCCDetail model = _pONCCDetailRepo.GetByID(PONCCDetailID);
                if (isapproved)
                {
                    model.QtyReal = model.QtyReal + item.Qty;
                    model.Soluongcon = model.QtyRequest - model.QtyReal;
                }
                else
                {
                    model.QtyReal = model.QtyReal - item.Qty;
                    model.Soluongcon = model.QtyRequest - model.QtyReal;
                }
                model.Status = model.Soluongcon == 0 ? 1 : 0;
                PONCCID = model.PONCCID ?? 0;
                await _pONCCDetailRepo.UpdateAsync(model);
                List<PONCCDetail> pONCCDetails = _pONCCDetailRepo.GetAll(x => x.PONCCID == PONCCID);
                if (pONCCDetails.Count <= 0) return;
                PONCC pONCC = GetByID(PONCCID);//Đang tiến hành 0//Hoàn thành 1//Huỷ 2
                if (CheckPONCCDetaila(pONCCDetails))
                {
                    if (pONCC.Status == 0)
                    {
                        pONCC.Status = 1;
                    }
                }
                else
                {
                    pONCC.Status = pONCC.Status == 1 ? 0 : pONCC.Status_Old;
                }
                await UpdateAsync(pONCC);
            }
        }
        private bool CheckPONCCDetaila(List<PONCCDetail> dt)
        {
            int dem = 0;
            for (int i = 0; i < dt.Count; i++)
            {
                dem = dt[i].Soluongcon == 0 ? dem + 1 : dem;

            }
            if (dem == dt.Count) return true;
            return false;
        }



        public string ConvertVietnameseToEnglish(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, System.String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public string ConvertPhoneNumberVietnamese(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return "";
            List<string> listPhoneNumber = new List<string>();
            string[] phoneNumbers = phoneNumber.Split('\n');
            foreach (string number in phoneNumbers)
            {
                if (string.IsNullOrEmpty(number.Trim())) continue;
                string phone = "(+84) " + number.Substring(1);
                listPhoneNumber.Add(phone);
            }

            return string.Join("\n", listPhoneNumber);
        }

        public string ConvertNumberToTextEnglish(decimal number, string currencyType)
        {

            if (number == 0) return "zero";

            string words = "";

            if (number < 0)
            {
                words += "minus ";
                number = Math.Abs(number);
            }

            // Split the number into integer and decimal parts
            long intPart = (long)number;
            int decimalPart = (int)((number - intPart) * 100);

            int thousandsCount = 0;

            // TextUtils integer part to words
            while (intPart > 0)
            {
                if (intPart % 1000 != 0)
                {
                    words = ConvertToWordsUnder1000((int)(intPart % 1000)) + thousandsGroups[thousandsCount] + " " + words;
                }
                intPart /= 1000;
                thousandsCount++;
            }

            var currencyUnit = UNIT_CURRENCY.FirstOrDefault(x => x.Name.ToLower().Trim() == currencyType.ToLower().Trim());
            string unit = "";
            string minUnit = "";
            if (currencyUnit != null)
            {
                var descriptionCurrency = currencyUnit.Description.GetType();
                if (currencyType == "RMB" & decimalPart > 0)
                {
                    minUnit = TextUtils.ToString(descriptionCurrency.GetProperty("nameenglish").GetValue(currencyUnit.Description));
                }
                else if (currencyType == "RMB" & decimalPart <= 0)
                {
                    unit = TextUtils.ToString(descriptionCurrency.GetProperty("nameenglish").GetValue(currencyUnit.Description));
                }
                else
                {
                    unit = TextUtils.ToString(descriptionCurrency.GetProperty("nameenglish").GetValue(currencyUnit.Description));
                    minUnit = TextUtils.ToString(descriptionCurrency.GetProperty("unitenglish").GetValue(currencyUnit.Description));
                }
            }
            words += unit + (decimalPart > 0 ? " " : ",");

            // TextUtils decimal part to words
            if (decimalPart > 0)
            {
                string joinText = currencyType == "RMB" ? "point " : "and ";
                words += joinText + ConvertToWordsUnder1000(decimalPart) + $" {minUnit}.";
            }
            words = words[0].ToString().ToUpper() + words.Substring(1);
            return words;
        }

        private string ConvertToWordsUnder1000(int number)
        {
            string words = "";

            if (number >= 100)
            {
                words += ones[number / 100] + " hundred";
                number %= 100;
            }

            if (number >= 20)
            {
                words += ((words != "") ? " " : "") + tens[number / 10];
                number %= 10;
            }

            if (number > 0)
            {
                words += ((words != "") ? " " : "") + ones[number];
            }

            return words;
        }


        #region Đọc số tiền bằng chữ tiếng việt
        public string ConvertNumberToTextVietNamese(decimal num, string currencyType)
        {
            try
            {
                string numberText = num.ToString();
                string numberMoneyText = "";
                if (!string.IsNullOrEmpty(numberText.Trim()))
                {
                    string decimalPart = "";
                    List<string> number = new List<string>();
                    if (numberText.Contains(','))
                    {
                        string intergerPart = numberText.Substring(0, numberText.LastIndexOf(','));
                        decimalPart = numberText.Substring(numberText.LastIndexOf(',') + 1);
                        string separator = numberText.Substring(numberText.LastIndexOf(','), 1);

                        if (decimalPart == "00")
                        {
                            number.Add(intergerPart);
                        }
                        else
                        {
                            number.AddRange(new[] { intergerPart, separator, decimalPart });
                        }
                    }
                    else
                    {
                        number.Add(numberText);
                    }

                    //string currenyName = "";
                    //string currenyUnit = "";
                    var currencyUnit = UNIT_CURRENCY.FirstOrDefault(x => x.Name == currencyType);
                    string currenyName = "";
                    string currenyUnit = "";
                    if (currencyUnit != null)
                    {
                        var descriptionCurrency = currencyUnit.Description.GetType();
                        if (currencyType == "RMB" & decimalPart != "00")
                        {
                            currenyUnit = TextUtils.ToString(descriptionCurrency.GetProperty("namevietnamese").GetValue(currencyUnit.Description));
                        }
                        else if (currencyType == "RMB" & decimalPart == "00")
                        {
                            currenyName = TextUtils.ToString(descriptionCurrency.GetProperty("namevietnamese").GetValue(currencyUnit.Description));
                        }
                        else
                        {
                            currenyName = TextUtils.ToString(descriptionCurrency.GetProperty("namevietnamese").GetValue(currencyUnit.Description));
                            currenyUnit = TextUtils.ToString(descriptionCurrency.GetProperty("unitvietnamese").GetValue(currencyUnit.Description));
                        }

                        //var descriptionCurrency = currencyUnit.Description.GetType();
                        //currenyName = TextUtils.ToString(descriptionCurrency.GetProperty("namevietnamese").GetValue(currencyUnit.Description));
                        //currenyUnit = TextUtils.ToString(descriptionCurrency.GetProperty("unitvietnamese").GetValue(currencyUnit.Description));
                    }

                    List<string> output = new List<string>();
                    for (int i = 0; i < number.Count; i++)
                    {
                        if (i == 0)
                        {
                            string numtotext = number[i].Trim();
                            output.Add(ConvertNumberToText(number[i]).Trim());
                            //output.Add("đồng");
                            if (!string.IsNullOrEmpty(currenyName))
                            {
                                output.Add(currenyName);
                            }
                        }
                        else if (i == 1)
                        {
                            output.Add(currencyType == "RMB" ? "phẩy" : "và");
                        }
                        else
                        {
                            if (i == 2)
                            {
                                var digits = number[i].ToCharArray();
                                List<string> decimalWords = new List<string>();
                                foreach (var d in digits)
                                {
                                    //decimalWords.Add(ConvertDigitToText(d));

                                    var dd = TextUtils.ToInt64(d);
                                }
                                //decimalWords.Add(DocSo(TextUtils.ToInt64(number[i])));
                                decimalWords.Add(DocSoTu1Den99(TextUtils.ToInt32(number[i])));
                                //for (int j = 0; j < digits.Count(); j++)
                                //{
                                //    decimalWords.Add(ConvertDigitToText(digits[j], j));
                                //    if (digits[0] != '0' && decimalWords.Count() > 0) decimalWords[0] += "mươi";
                                //}
                                output.Add(string.Join(" ", decimalWords).Trim());
                            }
                            else
                            {
                                string numtotext = number[i].Trim();
                                output.Add(ConvertNumberToText(number[i]).Trim());
                            }

                            if (!string.IsNullOrEmpty(currenyUnit))
                            {
                                output.Add(currenyUnit);
                            }
                        }
                    }

                    //if (number.Count == 1) output.Add("chẵn");

                    numberMoneyText = string.Join(" ", output);
                    if (!string.IsNullOrEmpty(numberMoneyText))
                    {
                        numberMoneyText = numberMoneyText[0].ToString().ToUpper() + numberMoneyText.Substring(1);
                    }
                }

                return numberMoneyText + ".";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string ConvertNumberToText(string num)
        {
            try
            {
                var needZeroCount = num.Length % 3;
                if (needZeroCount != 0)
                    needZeroCount = 3 - needZeroCount;

                num = new string('0', needZeroCount) + num;

                var output = new List<string>();
                for (var i = 0; i < num.Length / 3; i++)
                {
                    int a = TextUtils.ToInt32(num.Substring(i * 3, 1));
                    int b = TextUtils.ToInt32(num.Substring(i * 3 + 1, 1));
                    int c = TextUtils.ToInt32(num.Substring(i * 3 + 2, 1));

                    bool isFirstGroup = i == 0 || (a == 0 && b == 0 && c == 0);
                    output.AddRange(ReadThree(a, b, c, !isFirstGroup));

                    if (a != 0 || b != 0 || c != 0)
                    {
                        var unit = UNITS[num.Length / 3 - 1 - i];
                        if (unit != "")
                            output.Add(unit);
                    }
                }

                return string.Join(" ", output);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private List<string> ReadTwo(int b, int c, bool hasHundred)
        {
            var output = new List<string>();

            switch (b)
            {
                case 0:
                    if (hasHundred && c == 0)
                        break;
                    if (hasHundred)
                        output.Add("lẻ");
                    if (c != 0)
                        output.Add(DIGITS[c]);
                    break;

                case 1:
                    output.Add("mười");
                    if (c == 5)
                        output.Add("lăm");
                    else if (c != 0)
                        output.Add(DIGITS[c]);
                    break;

                default:
                    output.Add(DIGITS[b]);
                    output.Add("mươi");
                    if (c == 1)
                        output.Add("mốt");
                    else if (c == 5)
                        output.Add("lăm");
                    else if (c != 0)
                        output.Add(DIGITS[c]);
                    break;
            }

            return output;
        }

        private List<string> ReadThree(int a, int b, int c, bool readZeroHundred)
        {
            var output = new List<string>();

            if (a != 0 || readZeroHundred)
                output.AddRange(new[] { DIGITS[a], "trăm" });

            output.AddRange(ReadTwo(b, c, a != 0 || readZeroHundred));
            return output;
        }

        public string DocSoTu1Den99(int so)
        {
            //if (so < 0 || so > 99)
            //    throw new ArgumentOutOfRangeException(nameof(so), "Chỉ hỗ trợ từ 0 đến 99.");

            if (so < 10)
                return ChuSo[so];

            int chuc = so / 10;
            int donvi = so % 10;

            string kq = "";

            if (chuc == 1)
            {
                kq = "mười";
                if (donvi == 1)
                    kq += " một";
                else if (donvi == 5)
                    kq += " lăm";
                else if (donvi > 0)
                    kq += " " + ChuSo[donvi];
            }
            else
            {
                kq = ChuSo[chuc] + " mươi";

                if (donvi == 1)
                    kq += " mốt";
                else if (donvi == 5)
                    kq += " lăm";
                else if (donvi > 0)
                    kq += " " + ChuSo[donvi];
            }

            return kq.Trim();
        }
        #endregion
    }

    public class UnitCurrency
    {
        public string Name { get; set; } = string.Empty;
        public object Description { get; set; } = string.Empty;
    }
}