using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System.Text.RegularExpressions;

using RERPAPI.Model.Param;
using System.Globalization;
using System.Text;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListRepo : GenericRepo<ProjectPartList>
    {
        private readonly ProjectPartlistPriceRequestRepo _priceRepo;
        private readonly ProjectPartlistPurchaseRequestRepo _purchaseRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly ProjectPartlistVersionRepo _versionRepo;
        private readonly UnitCountRepo _unitCountRepo;

        public ProjectPartListRepo(CurrentUser currentUser, ProjectPartlistPriceRequestRepo projectPartlistPriceRequestRepo, ProjectPartlistPurchaseRequestRepo projectPartlistPurchaseRequestRepo, ProductSaleRepo productSaleRepo, ProjectPartlistVersionRepo projectPartlistVersionRepo, UnitCountRepo unitCountRepo) : base(currentUser)
        {
            _priceRepo = projectPartlistPriceRequestRepo;
            _purchaseRepo = projectPartlistPurchaseRequestRepo;
            _productSaleRepo = productSaleRepo;
            _versionRepo = projectPartlistVersionRepo;
            _unitCountRepo = unitCountRepo;
        }

        public int getSTT(int projectVersionID)
        {
            List<ProjectPartList> listPartList = GetAll(x => x.ProjectPartListVersionID == projectVersionID);
            int stt = listPartList.Count <= 0 ? 1 : listPartList.Max(a => a.STT ?? 0) + 1;
            return stt;
        }
        public int GetParentID(string tt, int projectTypeId, int versionId)
        {
            if (string.IsNullOrWhiteSpace(tt))
                return 0;

            tt = tt.Trim();

            // TT cấp gốc → không có cha
            if (!tt.Contains("."))
                return 0;

            // Tách theo mọi mức
            var parts = tt.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
                return 0;

            // TT cha (ví dụ: 3.4.2 → 3.4)
            string parentTT = string.Join(".", parts.Take(parts.Length - 1));

            var parent = GetAll(x =>
                x.TT != null &&
                x.TT.Trim() == parentTT &&
                x.ProjectPartListVersionID == versionId &&
                (x.ProjectTypeID ?? 0) == projectTypeId &&
                x.IsDeleted != true
            ).FirstOrDefault();

            return parent?.ID ?? 0;
        }
        public bool Validate(ProjectPartList item, out string message)
        {
            message = string.Empty;
            //check đã có yc báo giá chưa
            if (item.ID > 0)
            {
                if (item.IsDeleted == true) //Xóa
                {
                    if (item.IsApprovedPurchase == true)
                    {
                        message = $"Thiết bị mã [{item.ProductCode}] đã yêu cầu mua. Bạn không thể xóa.\nVui lòng liên hệ nhân viên mua hàng hoặc PM để hủy YÊU CẦU MUA HÀNG trước";
                        return false;
                    }
                    if (item.IsApprovedTBP == true)
                    {
                        message = $"Vật tư TT [{item.STT}] đã được TBP duyệt.\nVui lòng huỷ duyệt trước!";
                        return false;
                    }
                    if (item.ReasonDeleted == null || string.IsNullOrWhiteSpace((item.ReasonDeleted ?? "").Trim()))
                    {
                        message = $"Vui lòng nhập Lý do xóa!";
                        return false;
                    }
                    return true;
                }
                else if (item.IsProblem == true)
                {
                    if (string.IsNullOrWhiteSpace(item.ReasonProblem))
                    {
                        message = "Vật tư phát sinh phải có Lý do phát sinh!";
                        return false;
                    }
                    return true;
                }
                else if (item.IsApprovedTBP == true)
                {
                    string errorMessage = string.Empty;
                    if (!ValidateApproveTBP(item, true, out errorMessage))
                    {
                        message = errorMessage;
                        return false;
                    }
                    return true;
                }
                else if (item.IsApprovedPurchase == true)
                {
                    string errorMessage = string.Empty;
                    if (!ValidateApprovePurchase(item, true, out errorMessage))
                    {
                        message = errorMessage;
                        return false;
                    }
                }

                //check đã có yc mua hàng chưa
                var purchaseRequestDeletes = _purchaseRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted == false);
                var purchaseRequestCancels = _purchaseRepo.GetAll(x => x.ProjectPartListID == item.ID && x.StatusRequest != 2);
                if (purchaseRequestDeletes.Count > 0 && purchaseRequestCancels.Count > 0)
                {
                    message = $"Thiết bị mã [{item.ProductCode}] đã yêu cầu mua. Bạn không thể sửa.\nVui lòng liên hệ nhân viên mua hàng hoặc PM để hủy YÊU CẦU MUA HÀNG trước";
                    return false;
                }
                //check yêu cầu báo giá
                ProjectPartlistPriceRequest priceRequest = _priceRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted != true && x.IsCommercialProduct != true).FirstOrDefault() ?? new ProjectPartlistPriceRequest();
                if (priceRequest.ProjectPartListID > 0)
                {
                    if (priceRequest.IsCheckPrice == true)
                    {
                        message = $"Thiết bị mã [{item.ProductCode}] đang được check giá. Bạn không thể sửa.\nVui lòng liên hệ nhân viên mua hàng";
                        return false;
                    }
                    if (priceRequest.StatusRequest == 2)
                    {
                        message = $"Thiết bị mã [{item.ProductCode}] đã báo giá. Bạn không thể sửa.\nVui lòng liên hệ nhân viên mua hàng";
                        return false;
                    }
                    if (priceRequest.StatusRequest == 3)
                    {
                        message = $"Thiết bị mã [{item.ProductCode}] đã hoàn thành báo giá. Bạn không thể sửa.\nVui lòng liên hệ nhân viên mua hàng";
                        return false;
                    }
                }
            }

            //else
            //{
            //    ProjectPartlistPriceRequest priceRequestCheck = _priceRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted != true).FirstOrDefault() ?? new ProjectPartlistPriceRequest();
            //    if (priceRequestCheck.ProjectPartListID > 0 && priceRequestCheck.IsCheckPrice == true)
            //    {
            //        message = $"Phòng mua đang check giá sản phẩm Stt [{item.STT}].\nBạn không thể Huỷ Y/c báo giá!";
            //        return false;
            //    }
            //}


            string pattern = @"^[^àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴ]+$";
            Regex regex = new Regex(pattern);
            if (item.ProjectID <= 0)
            {
                message = "Vui lòng nhập Dự án!";
                return false;
            }
            if (item.ProjectPartListVersionID <= 0)
            {
                message = "Vui lòng nhập Phiên bản!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(item.TT))
            {
                message = "Vui lòng nhập TT!";
                return false;
            }
            else
            {
                List<ProjectPartList> projectPartLists = GetAll(x => x.ProjectPartListVersionID == item.ProjectPartListVersionID && (x.TT ?? "").Trim() == (item.TT ?? "").Trim() && x.ID != item.ID && x.IsDeleted != true && x.IsProblem != true);
                if (projectPartLists.Count > 0 && item.IsProblem == false)
                {
                    message = $"TT [{item.TT}] đã tồn tại.\nVui lòng kiểm tra lại!";
                    return false;
                }
                //check cùng TT giữa các danh mục phát sinh
                List<ProjectPartList> partLists = GetAll(x => x.ProjectPartListVersionID == item.ProjectPartListVersionID && (x.TT ?? "").Trim() == (item.TT ?? "").Trim() && x.ID != item.ID && x.IsDeleted != true && x.IsProblem == true);
                if (partLists.Count > 0 && item.IsProblem == false)
                {
                    message = $"TT [{item.TT}] là mục phát sinh đã tồn tại .\nVui lòng kiểm tra lại!";
                    return false;
                }

            }
            if (!string.IsNullOrWhiteSpace(item.SpecialCode))
            {
                var specialCode = GetAll(x => x.SpecialCode == item.SpecialCode && x.ID == item.ID && x.IsDeleted != true);
                if (specialCode.Count > 0)
                {
                    message = $"Mã đặc biệt [{item.SpecialCode}] đã tồn tại .\nVui lòng kiểm tra lại!";
                    return false;
                }
            }
            List<ProjectPartList> listChilds = GetAll(x => x.IsDeleted != true && x.ParentID == item.ParentID);
            if (listChilds.Count < 0)
            {
                if (string.IsNullOrEmpty((item.ProductCode ?? "").Trim()))
                {
                    message = "Vui lòng nhập Mã thiết bị!";
                    return false;
                }
                else
                {
                    bool isCheck = regex.IsMatch((item.ProductCode ?? "").Trim());
                    if (!isCheck)
                    {
                        message = "Mã thiết bị không được chứa ký tự tiếng Việt!";
                        return false;
                    }
                }
                if (string.IsNullOrEmpty((item.GroupMaterial ?? "").Trim()))
                {
                    message = "Vui lòng nhập Tên thiết bị!";
                    return false;
                }
                if (item.IsProblem == true && string.IsNullOrWhiteSpace((item.ReasonProblem ?? "").Trim()))
                {
                    message = "Vui lòng nhập Lý do phát sinh!";
                    return false;
                }
            }
            //ProductSale productSale = _productSaleRepo.GetAll(x => x.ProductCode.Trim() == item.ProductCode && x.IsDeleted == false).FirstOrDefault() ?? new ProductSale();

            //if (productSale.ID > 0 && productSale.IsFix==true)
            //{

            //}
            var productSale = _productSaleRepo.GetAll(x => x.IsDeleted != true && x.ProductCode == item.ProductCode).FirstOrDefault();
            if (productSale != null && productSale.ID > 0 && productSale.IsFix == true)
            {
                List<string> errors = new List<string>();
                if (UnicodeConverterService.ConvertUnicode((item.GroupMaterial ?? "").ToLower(), 1) != UnicodeConverterService.ConvertUnicode((productSale.ProductName ?? "").ToLower(), 1))
                {
                    errors.Add($"\nTên thiết bị (tích xanh: [{productSale.ProductName}], hiện tại: [{item.GroupMaterial}])");
                }
                if (UnicodeConverterService.ConvertUnicode((item.Manufacturer ?? "").ToLower(), 1) != UnicodeConverterService.ConvertUnicode((productSale.Maker ?? "").ToLower(), 1))
                {
                    errors.Add($"\nHãng sản xuất (tích xanh: [{productSale.Maker}], hiện tại: [{item.Manufacturer}])");
                }
                if (UnicodeConverterService.ConvertUnicode((item.Unit ?? "").ToLower(), 1) != UnicodeConverterService.ConvertUnicode((productSale.Unit ?? "").ToLower(), 1))
                {
                    errors.Add($"\nĐơn vị (tích xanh: [{productSale.Unit}], hiện tại: [{item.Unit}])");
                }
                if (errors.Any())
                {
                    message = $"Mã thiết bị {item.ProductCode} đã có TÍCH XANH.\n" +
                        $"Các trường không khớp:\n {string.Join("\n", errors)}\n\n";
                    return false;
                }
            }
            return true;
        }
        public bool ValidateApproveTBP(ProjectPartList partlist, bool isApproved, out string message)
        {
            message = string.Empty;
            string isAprrovedText = isApproved == true ? "Duyệt" : "Hủy duyệt";
            if (partlist.ID <= 0)
            {
                message = "Vui lòng chọn vật tư!";
                return false;
            }
            ProjectPartList partlistexist = GetByID(partlist.ID);

            ProjectPartListVersion version = _versionRepo.GetByID(partlistexist.ProjectPartListVersionID ?? 0);
            if (version.IsActive == false || version.IsActive == null)
            {
                message = $"Vui lòng chọn sử dụng phiên bản [{version.Code}] trước!";
                return false;
            }
            if (partlist.IsDeleted == true)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] đã bị xóa!";
                return false;
            }
            if (!isApproved && partlist.IsApprovedPurchase == true)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] đã được Yêu cầu mua!";
                return false;
            }
            //validate product sale
            //List<ProductSale> prdSale = _productSaleRepo.GetAll(x => x.ProductCode == partlist.ProductCode && x.IsDeleted == false);
            //if (prdSale.Count <= 0)
            //{
            //    message = $"Không thể duyệt tích xanh vì sản phẩm [{partlist.ProductCode}] không có trong kho sale!";
            //    return false;
            //}
            //var fixedProduct = prdSale.FirstOrDefault(x => (x.IsFix ?? true));
            //if (fixedProduct != null)
            //{
            //    List<string> errors = new List<string>();
            //    string productNameConvert = UnicodeConverterService.ConvertUnicode((fixedProduct.ProductName ?? "").ToLower(), 1);
            //    string makerConvert = UnicodeConverterService.ConvertUnicode((fixedProduct.Maker ?? "").ToLower(), 1);
            //    string unitConvert = UnicodeConverterService.ConvertUnicode((fixedProduct.Unit ?? "").ToLower(), 1);
            //    if (productNameConvert != UnicodeConverterService.ConvertUnicode((partlist.GroupMaterial ?? "").ToLower(), 1))
            //    {
            //        errors.Add($"\nMã sản phẩm (tích xanh: [{fixedProduct.ProductName}], hiện tại: [{partlist.GroupMaterial}])");
            //        return false;
            //    }
            //    if (makerConvert != UnicodeConverterService.ConvertUnicode((partlist.Manufacturer ?? "").ToLower(), 1))
            //    {
            //        errors.Add($"\nNhà sản xuất (tích xanh: [{fixedProduct.Maker}], hiện tại: [{partlist.Manufacturer}])");
            //    }
            //    if (unitConvert != UnicodeConverterService.ConvertUnicode((partlist.Unit ?? "").ToLower(), 1))
            //    {
            //        errors.Add($"\nĐơn vị (tích xanh: [{fixedProduct.Unit}], hiện tại: [{partlist.Unit}])");
            //    }
            //    if (errors.Any())
            //    {
            //        message = $"Sản phẩm có mã [{partlist.ProductCode}] đã có tích xanh.\nCác trường không khớp: {string.Join(" ", errors)}. \nVui lòng kiểm tra lại.";
            //        return false;
            //    }
            //}
            /* string errorsMessage = string.Empty;
             if (!ValidateProduct(partlist, out errorsMessage))
             {
                 message = errorsMessage;
                 return false;
             }*/
            return true;
        }
        public bool ValidateProduct(ProjectPartList partlist, out string message)
        {
            message = string.Empty;

            string productCode = (partlist.ProductCode ?? "").Trim();

            // 1. Lấy bản ghi sale theo ProductCode
            var prdSale = _productSaleRepo.GetAll(x =>
                (x.ProductCode ?? "").Trim() == productCode &&
                x.IsDeleted != true
            );

            if (prdSale == null || prdSale.Count == 0)
            {
                message = $"Không thể duyệt tích xanh vì sản phẩm [{productCode}] không có trong kho sale!";
                return false;
            }

            // 2. Tìm bản ghi đã FIX
            var fixedProduct = prdSale.FirstOrDefault(x => x.IsFix == true);
            if (fixedProduct == null)
            {
                // Nếu chưa Fix thì OK
                return true;
            }

            // 3. So sánh dữ liệu
            List<string> errors = new List<string>();

            string fixedName = Normalize(fixedProduct.ProductName);
            string fixedMaker = Normalize(fixedProduct.Maker);
            string fixedUnit = Normalize(fixedProduct.Unit);

            string currentName = Normalize(partlist.GroupMaterial);
            string currentMaker = Normalize(partlist.Manufacturer);
            string currentUnit = Normalize(partlist.Unit);

            // Product Name
            if (fixedName != currentName)
            {
                errors.Add($"\nMã sản phẩm (tích xanh: [{fixedProduct.ProductName}], hiện tại: [{partlist.GroupMaterial}])");
            }

            // Maker
            if (fixedMaker != currentMaker)
            {
                errors.Add($"\nNhà sản xuất (tích xanh: [{fixedProduct.Maker}], hiện tại: [{partlist.Manufacturer}])");
            }

            // Unit
            if (fixedUnit != currentUnit)
            {
                errors.Add($"\nĐơn vị (tích xanh: [{fixedProduct.Unit}], hiện tại: [{partlist.Unit}])");
            }

            // 4. Nếu có lỗi -> trả message
            if (errors.Count > 0)
            {
                message =
                    $"Sản phẩm có mã [{productCode}] đã được tích xanh.\n" +
                    $"Các trường không khớp:{string.Join("", errors)}\n" +
                    $"Vui lòng kiểm tra lại.";
                return false;
            }

            return true;
        }

        // Helper Normalize
        private string Normalize(string value)
        {
            value = (value ?? "").Trim().ToLower();
            return UnicodeConverterService.ConvertUnicode(value, 1);
        }
        public bool ValidateApprovePurchase(ProjectPartList partlist, bool isApproved, out string message)
        {
            message = string.Empty;
            string isAprrovedText = isApproved == true ? "Yêu cầu mua" : "Hủy yêu cầu mua";
            if (partlist.ID <= 0)
            {
                message = "Vui lòng chọn vật tư!";
                return false;
            }
            if (isApproved)
            {
                string errorMessage = string.Empty;
                if (!CheckValidate(partlist, out errorMessage))
                {
                    message = errorMessage;
                    return false;
                }
            }
            if (partlist.IsDeleted == true)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] đã bị xóa!";
                return false;
            }
            if (isApproved && partlist.IsApprovedPurchase == true)
            {
                message = $"Vật tư thứ tự [{partlist.STT}] đã được Y/c mua.\nVui lòng kiểm tra lại!";
                return false;
            }
            if (isApproved && partlist.IsApprovedTBP == false)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] chưa được TBP duyệt!";
                return false;
            }
            if (isApproved && partlist.IsApprovedTBPNewCode == false && partlist.IsNewCode == true)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] chưa được TBP duyệt mới!";
                return false;
            }
            return true;
        }
        //validate khi yêu cầu báo giá hoặc yêu cầu mua hàng
        public bool CheckValidate(ProjectPartList item, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty((item.ProductCode ?? "").Trim()))
            {
                message = $"[Mã thiết bị] có số thứ tự [{item.STT}] không được trống!\nVui lòng kiểm tra lại!";
                return false;
            }
            if (string.IsNullOrEmpty((item.GroupMaterial ?? "").Trim()))
            {
                message = $"[Tên vật tư] có số thứ tự [{item.STT}] không được trống!\nVui lòng kiểm tra lại!";
                return false;
            }
            if (string.IsNullOrEmpty(item.Manufacturer))
            {
                message = $"[Hãng SX] có số thứ tự [{item.STT}] không được trống!\nVui lòng kiểm tra lại!";
                return false;
            }
            if (item.QtyMin == null || item.QtyMin <= 0)
            {
                message = $"[Số lượng / 1 máy] có số thứ tự [{item.STT}] phải lớn hơn 0!\nVui lòng kiểm tra lại!";
                return false;
            }
            if (item.QtyFull == null || item.QtyFull <= 0)
            {
                message = $"[Số lượng tổng] có số thứ tự [{item.STT}] phải lớn hơn 0!\nVui lòng kiểm tra lại!";
                return false;
            }
            return true;
        }
        //y/c mua 
        public async void UpdatePurchaseRequest(List<ProjectPartList> listPartlists)
        {
            foreach (var item in listPartlists)
            {
                if (item.ID <= 0) continue;

                List<ProjectPartlistPurchaseRequest> requests = _purchaseRepo.GetAll(x => x.ProjectPartListID == item.ID && x.IsDeleted == false);
                ProjectPartlistPurchaseRequest request = requests.FirstOrDefault();
                request = request == null ? new ProjectPartlistPurchaseRequest() : request;

                request.ProjectPartListID = item.ID;
                request.EmployeeID = item.EmployeeID;
                request.ProductCode = item.ProductCode;
                request.ProductName = item.GroupMaterial;
                request.StatusRequest = item.Status;
                request.DateRequest = item.RequestDate;
                request.DateReturnExpected = item.ExpectedReturnDate;
                request.Quantity = item.QtyFull;
                request.SupplierSaleID = item.SupplierSaleID;
                request.UnitMoney = item.UnitMoney;
                request.Quantity = item.QtyFull;
                request.UnitPrice = item.PriceOrder;
                request.TotalPrice = item.TotalPriceOrder;

                UnitCount unit = _unitCountRepo.GetAll(x => x.UnitName == item.Unit.Trim()).FirstOrDefault();
                if (unit != null)
                {
                    request.UnitCountID = unit.ID;
                }
                if (request.ID <= 0)
                {
                    await _purchaseRepo.CreateAsync(request);
                }
                else
                {
                    if (request.StatusRequest > 2) continue;
                    if (requests.Count > 0) { }
                    await _purchaseRepo.UpdateAsync(request);
                }
            }
        }


        #region validate import excel
        public bool ValidateImportExcel(PartlistImportRequestDTO request, out string message)
        {
            message = string.Empty;

            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                message = "Không có dữ liệu để import!";
                return false;
            }

            // Regex: TT chỉ gồm số và dấu chấm
            Regex regexTT = new Regex(@"^-?[\d\.]+$");

            // Regex: Mã KHÔNG chứa ký tự tiếng Việt
            string pattern = @"^[^àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴ]+$";
            Regex regexCode = new Regex(pattern);

            // Lấy danh sách TT hợp lệ trong file
            var rowsHaveTT = request.Items
                .Where(x => !string.IsNullOrWhiteSpace(x.TT))
                .ToList();

            if (!rowsHaveTT.Any())
            {
                message = "Không có TT hợp lệ trong file Excel!";
                return false;
            }

            // Chuẩn hóa TT
            foreach (var row in rowsHaveTT)
            {
                row.TT = row.TT.Trim();
                if (!regexTT.IsMatch(row.TT))
                {
                    message = $"TT [{row.TT}] không đúng định dạng (chỉ được chứa số và dấu chấm)!";
                    return false;
                }
            }

            var ttAll = rowsHaveTT.Select(x => x.TT).ToList();

            // 1. TT trùng trong file Excel
            var dupInFile = ttAll
                .GroupBy(x => x)
                .FirstOrDefault(g => g.Count() > 1);

            if (dupInFile != null)
            {
                message = $"TT [{dupInFile.Key}] bị trùng trong file Excel!\nVui lòng kiểm tra lại!";
                return false;
            }

            // 2. TT trùng trong DB cho version này (không tính IsProblem = true)
            var existTTInDb = GetAll(x =>
                x.ProjectPartListVersionID == request.ProjectPartListVersionID
                && x.IsDeleted != true
                && (x.IsProblem == false || x.IsProblem == null)
                && x.TT != null
                && ttAll.Contains(x.TT.Trim())
            );

            if (existTTInDb.Any())
            {
                var tt = existTTInDb.First().TT;
                message = $"TT [{tt}] đã tồn tại trong phiên bản này.\nVui lòng kiểm tra lại!";
                return false;
            }

            // 3. Xác định node CHA: TT nào xuất hiện làm prefix của TT khác
            var parentTTs = new HashSet<string>();
            foreach (var tt in ttAll)
            {
                if (!tt.Contains(".")) continue;

                var parts = tt.Split('.');
                if (parts.Length <= 1) continue;
                var parentTT = string.Join(".", parts.Take(parts.Length - 1));
                parentTTs.Add(parentTT);
            }

            // 4. Validate từng dòng
            foreach (var row in rowsHaveTT)
            {
                string tt = row.TT;
                bool isParent = parentTTs.Contains(tt); // TT xuất hiện làm cha của node khác

                // Độ sâu TT: 1, 1.1, 1.1.1
                int depth = tt.Split('.').Length;

                // Rule WinForm: depth >= 3 và GroupMaterial == ProductCode thì lỗi
                if (depth >= 3 &&
                    !string.IsNullOrWhiteSpace(row.GroupMaterial) &&
                    !string.IsNullOrWhiteSpace(row.ProductCode) &&
                    row.GroupMaterial.Trim().Equals(row.ProductCode.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    message = $"[Tên vật tư] có số thứ tự [{tt}] đã bị trùng với [Mã thiết bị]!";
                    return false;
                }

                // Nếu là node LÁ (không phải cha) → bắt buộc nhập đủ
                if (!isParent)
                {
                    // Mã thiết bị
                    if (string.IsNullOrWhiteSpace(row.ProductCode))
                    {
                        message = $"[Mã thiết bị] có số thứ tự [{tt}] không được trống!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    if (!regexCode.IsMatch(row.ProductCode.Trim()))
                    {
                        message = $"[Mã thiết bị] có số thứ tự [{tt}] không được chứa ký tự tiếng Việt!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    // Tên thiết bị
                    if (string.IsNullOrWhiteSpace(row.GroupMaterial))
                    {
                        message = $"[Tên thiết bị] có số thứ tự [{tt}] không được trống!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    // Hãng SX
                    if (string.IsNullOrWhiteSpace(row.Manufacturer))
                    {
                        message = $"[Hãng SX] có số thứ tự [{tt}] không được trống!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    // Số lượng / 1 máy
                    if (row.QtyMin == null || row.QtyMin <= 0)
                    {
                        message = $"[Số lượng / 1 máy] có số thứ tự [{tt}] phải lớn hơn 0!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    // Số lượng tổng
                    if (row.QtyFull == null || row.QtyFull <= 0)
                    {
                        message = $"[Số lượng tổng] có số thứ tự [{tt}] phải lớn hơn 0!\nVui lòng kiểm tra lại!";
                        return false;
                    }

                    // Vật tư phát sinh phải có lý do
                    if (request.IsProblem && string.IsNullOrWhiteSpace(row.ReasonProblem))
                    {
                        message = $"Vật tư phát sinh có số thứ tự [{tt}] phải có Lý do phát sinh!";
                        return false;
                    }
                }

                // 5. Validate theo mã có TÍCH XANH (ProductSale.IsFix == true)
                if (!string.IsNullOrWhiteSpace(row.ProductCode))
                {
                    string code = row.ProductCode.Trim();

                    var productSale = _productSaleRepo.GetAll(x =>
                        x.IsDeleted != true &&
                        x.ProductCode == code &&
                        x.IsFix == true
                    ).FirstOrDefault();

                    if (productSale != null && productSale.ID > 0)
                    {
                        List<string> errors = new List<string>();

                        string nameSale = UnicodeConverterService.ConvertUnicode((productSale.ProductName ?? "").ToLower(), 1);
                        string nameExcel = UnicodeConverterService.ConvertUnicode((row.GroupMaterial ?? "").ToLower(), 1);

                        if (nameSale != nameExcel)
                        {
                            errors.Add($"\nTên thiết bị (tích xanh: [{productSale.ProductName}], hiện tại: [{row.GroupMaterial}])");
                        }

                        string makerSale = UnicodeConverterService.ConvertUnicode((productSale.Maker ?? "").ToLower(), 1);
                        string makerExcel = UnicodeConverterService.ConvertUnicode((row.Manufacturer ?? "").ToLower(), 1);

                        if (makerSale != makerExcel)
                        {
                            errors.Add($"\nHãng sản xuất (tích xanh: [{productSale.Maker}], hiện tại: [{row.Manufacturer}])");
                        }

                        string unitSale = UnicodeConverterService.ConvertUnicode((productSale.Unit ?? "").ToLower(), 1);
                        string unitExcel = UnicodeConverterService.ConvertUnicode((row.Unit ?? "").ToLower(), 1);

                        if (unitSale != unitExcel)
                        {
                            errors.Add($"\nĐơn vị (tích xanh: [{productSale.Unit}], hiện tại: [{row.Unit}])");
                        }

                        if (errors.Any())
                        {
                            message = $"Mã thiết bị {row.ProductCode} đã có TÍCH XANH.\n" +
                                      $"Các trường không khớp: {string.Join("", errors)}\n\nVui lòng kiểm tra lại!";
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion
        public class PartlistValidateResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; } = "";

            // Chuyển dtError → List<PartlistDiffDto>
            //public List<PartlistDiffDTO> Diffs { get; set; } = new();
        }
        Regex regex = new Regex(@"^-?[\d\.]+$");

        /// <summary>
        /// Chuyển tiếng Việt sang không dấu (tương đương TextUtils.ConvertUnicode(..., 1))
        /// </summary>
        public static string ConvertUnicode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string result = sb.ToString().Normalize(NormalizationForm.FormC);

            // Xử lý đ, Đ
            result = result.Replace("đ", "d").Replace("Đ", "D");

            return result;
        }
        public PartlistValidateResult Validate2(PartlistImportRequestDTO request)
        {
            var result = new PartlistValidateResult
            {
                IsValid = true
            };

            string pattern = @"^[^àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴ]+$";
            Regex regex = new Regex(pattern);
            Regex regexStt = new Regex(@"^-?[\d\.]+$");

            List<string> listSTT = new();
            List<string> listSttAll = new();

            // 1. Thu thập STT
            foreach (var item in request.Items)
            {
                if (string.IsNullOrEmpty(item.TT)) continue;

                listSttAll.Add(item.TT);

                if (!item.TT.Contains(".")) continue;
                if (!regexStt.IsMatch(item.TT)) continue;

                var root = item.TT[..item.TT.LastIndexOf(".")];

                if (!listSTT.Contains(root))
                    listSTT.Add(root);
            }

            // 2. Validate từng dòng
            foreach (var item in request.Items)
            {
                string stt = item.TT;
                string groupMaterial = item.GroupMaterial;
                string productCode = item.ProductCode;
                string manufacturer = item.Manufacturer;

                decimal qtyMin = (decimal)(item.QtyMin ?? 0);
                decimal qtyFull = (decimal)(item.QtyFull ?? 0);
                string unit = item.Unit;

                if (string.IsNullOrEmpty(stt)) continue;
                if (!regexStt.IsMatch(stt)) continue;

                // Check STT duplicate
                if (listSttAll.Count(x => x == stt) > 1)
                {
                    return Fail(result, $"TT [{stt}] đã tồn tại.\nVui lòng kiểm tra lại!");
                }

                bool isLeaf = !listSTT.Contains(stt); // Leaf node

                // ============= RULE WINFORMS: Vật tư phát sinh phải có lý do =============
                if (request.IsProblem == true && isLeaf)
                {
                    if (string.IsNullOrWhiteSpace(item.ReasonProblem))
                    {
                        return Fail(result, $"Vật tư phát sinh có số thứ tự [{stt}] phải có Lý do phát sinh!");
                    }
                }

                // Validate dòng leaf
                if (isLeaf)
                {
                    if (string.IsNullOrWhiteSpace(productCode))
                        return Fail(result, $"Vui lòng nhập Mã thiết bị!.\n(TT: {stt})");

                    if (!regex.IsMatch(productCode))
                        return Fail(result, $"Mã thiết bị không được chứa ký tự tiếng Việt!.\n(TT: {stt})");

                    if (string.IsNullOrWhiteSpace(groupMaterial))
                        return Fail(result, $"Vui lòng nhập Tên thiết bị!.\n(TT: {stt})");

                    if (string.IsNullOrWhiteSpace(manufacturer))
                        return Fail(result, $"Vui lòng nhập Hãng!.\n(TT: {stt})");

                    if (qtyMin <= 0)
                        return Fail(result, $"Vui lòng nhập Số lượng / 1 máy (Phải > 0)!.\n(TT: {stt})");

                    if (qtyFull <= 0)
                        return Fail(result, $"Vui lòng nhập Số lượng tổng (Phải > 0)!.\n(TT: {stt})");

                    // 5. Check với Stock (IsFix = true)
                    var fixedProduct = _productSaleRepo
                        .GetAll(x => x.ProductCode == productCode && x.IsFix == true && x.IsDeleted == false)
                        .FirstOrDefault();

                    if (fixedProduct != null)
                    {
                        string excelGroup = ConvertUnicode(groupMaterial.ToLower());
                        string excelManufacturer = ConvertUnicode(manufacturer.ToLower());
                        string excelUnit = ConvertUnicode(unit.ToLower());

                        string stockGroup = ConvertUnicode(fixedProduct.ProductName.ToLower());
                        string stockManufacturer = ConvertUnicode(fixedProduct.Maker.ToLower());
                        string stockUnit = ConvertUnicode(fixedProduct.Unit.ToLower());

                        if (excelGroup != stockGroup ||
                            excelManufacturer != stockManufacturer ||
                            excelUnit != stockUnit)
                        {
                            // Thêm vào DIFF LIST
                            //result.Diffs.Add(new PartlistDiffDTO
                            //{
                            //    ProductSaleId = fixedProduct.ID,
                            //    ProductCode = productCode,

                            //    GroupMaterialPartlist = groupMaterial,
                            //    GroupMaterialStock = fixedProduct.ProductName,

                            //    ManufacturerPartlist = manufacturer,
                            //    ManufacturerStock = fixedProduct.Maker,

                            //    UnitPartlist = unit,
                            //    UnitStock = fixedProduct.Unit,

                            //    IsFix = fixedProduct.IsFix ?? true
                            //});
                        }
                    }
                }
            }

            //if (result.Diffs.Any())
            //{
            //    result.IsValid = false;
            //    result.Message = "Có sự khác nhau giữa Partlist và dữ liệu tích xanh trong kho.";
            //}

            return result;
        }

        private PartlistValidateResult Fail(PartlistValidateResult res, string msg)
        {
            res.IsValid = false;
            res.Message = msg;
            return res;
        }
        public bool Validate1(PartlistImportRequestDTO request, out string message)
        {
            message = string.Empty;
            if (request.CheckIsStock == true) return true;
            if (request.ProjectPartListVersionID <= 0)
            {
                message = "Vui lòng nhập phiên bản!";
                return false;
            }
            if (!request.Items[0].OrderCode.Equals(request.ProjectCode))
            {
                message = "Không đúng mã dự án ";
                return false;
            }
            foreach (var item in request.Items)
            {
                if (string.IsNullOrEmpty(item.TT)) continue;
                if (!this.regex.IsMatch(item.TT)) continue;

                List<string> isParent = item.TT.Split('.').ToList();
                if (isParent.Count <= 3)
                {
                    if (item.GroupMaterial == item.ProductCode)
                    {
                        message = $"[Tên vật tư] có số thứ tự [{item.TT}] đã bị trùng với [mã thiết bị]!";
                        return false;
                    }
                }
            }
            return true;
        }
        public bool ValidateIsFix(ProjectPartlistDTO request, out string message)
        {
            message = string.Empty;

            return true;
        }

        string[] unitNames = new string[] { "m", "mét" };
        public bool ValidateKeep(ProjectPartListExportDTO partList , int wareHouseID, out string productNewCode)
        {
            productNewCode = string.Empty;
            if (partList == null) return false;
            string unitName = partList.Unit;
            if (unitNames.Contains(unitName.Trim().ToLower())) return true;

            int billExportDetailID = 0;
            int productID = partList.ProductID ;
            int projectID = partList.ProjectID ;
            //int pokhDetailID = 0;
            decimal remainQuantity = partList.RemainQuantity;
            decimal quantityReturn = partList.QuantityReturn ;
            decimal qtyFull = partList.QtyFull ;

            if (remainQuantity <= 0) return false;
            if (quantityReturn <= 0) return false;

            decimal totalQty = (quantityReturn >= qtyFull) ? remainQuantity : Math.Min(remainQuantity, quantityReturn);
            int pokhDetailID = 0;
            
            string productCode = partList.ProductNewCode ?? "";
            string projectCode = partList.ProjectCode ?? " ";
          

            // Lấy tồn kho theo sp, project, POKH
            var ds = SQLHelper<dynamic>.ProcedureToList("spGetInventoryProjectImportExport",
                new string[] { "@WarehouseID", "@ProductID", "@ProjectID", "@POKHDetailID", "@BillExportDetailID" },
                new object[] { wareHouseID, productID,projectID,pokhDetailID, billExportDetailID });

            var inventoryProjects = ds[0];
            var dtImport = ds[1];
            var dtExport = ds[2];
            var dtStock = ds[3];

            decimal totalQuantityKeep = inventoryProjects.Count > 0 ? Convert.ToDecimal(inventoryProjects[0].TotalQuantity) : 0; 
            decimal totalQuantityLast = dtStock.Count > 0 ? Convert.ToDecimal(dtStock[0].TotalQuantityLast) : 0;
            decimal totalImport = dtImport.Count > 0 ? Convert.ToDecimal(dtImport[0].TotalImport) : 0;
            decimal totalExport = dtExport.Count > 0 ? Convert.ToDecimal(dtExport[0].TotalExport) : 0;

            decimal totalQuantityRemain = Math.Max(totalImport - totalExport, 0);

            decimal totalStock = Math.Max(totalQuantityKeep, 0) + totalQuantityRemain + Math.Max(totalQuantityLast, 0);
            if(totalQty > totalStock)
            {
                productNewCode = productCode;
                return false;
            }

            return true;
        }

    }

}