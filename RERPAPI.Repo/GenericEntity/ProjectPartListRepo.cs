using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartListRepo : GenericRepo<ProjectPartList>
    {
        private ProjectPartlistPriceRequestRepo _priceRepo = new ProjectPartlistPriceRequestRepo();
        private ProjectPartlistPurchaseRequestRepo _purchaseRepo = new ProjectPartlistPurchaseRequestRepo();
        private ProductSaleRepo _productSaleRepo = new ProductSaleRepo();
        ProjectPartlistVersionRepo _versionRepo = new ProjectPartlistVersionRepo();
        public int getSTT(int projectVersionID)
        {
            List<ProjectPartList> listPartList = GetAll(x => x.ProjectPartListVersionID == projectVersionID);
            int stt = listPartList.Count <= 0 ? 1 : listPartList.Max(a => a.STT ?? 0) + 1;
            return stt;
        }
        public int getParentID(string tt, int typeId, int versionId)
        {
            int parentId = 0;
            if (tt.Contains("."))
            {
                string ttParent = tt.Substring(0, tt.LastIndexOf('.')).Trim();
                ProjectPartList checkParent = GetAll(x => x.TT == ttParent && x.ProjectPartListVersionID == versionId && x.IsDeleted == false).FirstOrDefault() ?? new ProjectPartList();
                if (checkParent.ID > 0) parentId = checkParent.ID;
            }

            return parentId;
        }
        public bool Validate(ProjectPartList item, out string message)
        {
            message = string.Empty;
            //check đã có yc báo giá chưa
            if (item.ID > 0)
            {
                if (item.IsDeleted == true)
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
                    if (item.ReasonDeleted == null || string.IsNullOrWhiteSpace(item.ReasonDeleted.Trim()))
                    {
                        message = $"Vui lòng nhập Lý do xóa!";
                        return false;
                    }
                    return true;
                }
                if (item.IsApprovedTBP == true)
                {
                    string errorMessage = string.Empty;
                    if (!ValidateApproveTBP(item, true, out errorMessage))
                    {
                        message = errorMessage;
                        return false;
                    }
                    return true;
                }
                if (item.IsApprovedPurchase == true)
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
                List<ProjectPartList> projectPartLists = GetAll(x => x.ProjectPartListVersionID == item.ProjectPartListVersionID && x.TT.Trim() == item.TT.Trim() && x.ID != item.ID && x.IsDeleted != true && x.IsProblem != true);
                if (projectPartLists.Count > 0 && item.IsProblem == false)
                {
                    message = $"TT [{item.TT}] đã tồn tại.\nVui lòng kiểm tra lại!";
                    return false;
                }
            }
            List<ProjectPartList> listChilds = GetAll(x => x.IsDeleted != true && x.ParentID == item.ParentID);
            if (listChilds.Count < 0)
            {
                if (string.IsNullOrEmpty(item.ProductCode.Trim()))
                {
                    message = "Vui lòng nhập Mã thiết bị!";
                    return false;
                }
                else
                {
                    bool isCheck = regex.IsMatch(item.ProductCode.Trim());
                    if (!isCheck)
                    {
                        message = "Mã thiết bị không được chứa ký tự tiếng Việt!";
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(item.GroupMaterial.Trim()))
                {
                    message = "Vui lòng nhập Tên thiết bị!";
                    return false;
                }
                if (item.IsProblem == true && string.IsNullOrWhiteSpace(item.ReasonProblem.Trim()))
                {
                    message = "Vui lòng nhập Lý do phát sinh!";
                    return false;
                }
            }
            //ProductSale productSale = _productSaleRepo.GetAll(x => x.ProductCode.Trim() == item.ProductCode && x.IsDeleted == false).FirstOrDefault() ?? new ProductSale();

            //if (productSale.ID > 0 && productSale.IsFix==true)
            //{

            //}
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
            ProjectPartListVersion version = _versionRepo.GetByID(partlist.ProjectPartListVersionID ?? 0);
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
            List<ProductSale> prdSale = _productSaleRepo.GetAll(x => x.ProductCode.Trim() == partlist.ProductCode.Trim() && x.IsDeleted == false);
            if (prdSale.Count <= 0)
            {
                message = $"Không thể duyệt tích xanh vì sản phẩm [{partlist.ProductCode}] không có trong kho sale!";
                return false;
            }
            var fixedProduct = prdSale.FirstOrDefault(x => (x.IsFix ?? true));
            if (fixedProduct != null)
            {
                List<string> errors = new List<string>();
                string productNameConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.ProductName.ToLower(), 1);
                string makerConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.Maker.ToLower(), 1);
                string unitConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.Unit.ToLower(), 1);
                if (productNameConvert != UnicodeConverterService.ConvertUnicode(partlist.GroupMaterial.ToLower(), 1))
                {
                    errors.Add($"\nMã sản phẩm (tích xanh: [{fixedProduct.ProductName}], hiện tại: [{partlist.GroupMaterial}])");
                    return false;
                }
                if (makerConvert != UnicodeConverterService.ConvertUnicode(partlist.Manufacturer.ToLower(), 1))
                {
                    errors.Add($"\nNhà sản xuất (tích xanh: [{fixedProduct.Maker}], hiện tại: [{partlist.Manufacturer}])");
                }
                if (unitConvert != UnicodeConverterService.ConvertUnicode(partlist.Unit.ToLower(), 1))
                {
                    errors.Add($"\nĐơn vị (tích xanh: [{fixedProduct.Unit}], hiện tại: [{partlist.Unit}])");
                }
                if (errors.Any())
                {
                    message = $"Sản phẩm có mã [{partlist.ProductCode}] đã có tích xanh.\nCác trường không khớp: {string.Join(" ", errors)}. \nVui lòng kiểm tra lại.";
                    return false;
                }
            }
            string errorsMessage = string.Empty;
            if (!ValidateProduct(partlist, out errorsMessage))
            {
                message = errorsMessage;
                return false;
            }
            return true;
        }
        public bool ValidateProduct(ProjectPartList partlist, out string message)
        {
            message = string.Empty;
            List<ProductSale> prdSale = _productSaleRepo.GetAll(x => x.ProductCode.Trim() == partlist.ProductCode.Trim() && x.IsDeleted == false);
            if (prdSale.Count <= 0)
            {
                message = $"Không thể duyệt tích xanh vì sản phẩm [{partlist.ProductCode}] không có trong kho sale!";
                return false;
            }
            var fixedProduct = prdSale.FirstOrDefault(x => (x.IsFix ?? true));
            if (fixedProduct != null)
            {
                List<string> errors = new List<string>();
                string productNameConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.ProductName.ToLower(), 1);
                string makerConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.Maker.ToLower(), 1);
                string unitConvert = UnicodeConverterService.ConvertUnicode(fixedProduct.Unit.ToLower(), 1);
                if (productNameConvert != UnicodeConverterService.ConvertUnicode(partlist.GroupMaterial.ToLower(), 1))
                {
                    errors.Add($"\nMã sản phẩm (tích xanh: [{fixedProduct.ProductName}], hiện tại: [{partlist.GroupMaterial}])");
                    return false;
                }
                if (makerConvert != UnicodeConverterService.ConvertUnicode(partlist.Manufacturer.ToLower(), 1))
                {
                    errors.Add($"\nNhà sản xuất (tích xanh: [{fixedProduct.Maker}], hiện tại: [{partlist.Manufacturer}])");
                }
                if (unitConvert != UnicodeConverterService.ConvertUnicode(partlist.Unit.ToLower(), 1))
                {
                    errors.Add($"\nĐơn vị (tích xanh: [{fixedProduct.Unit}], hiện tại: [{partlist.Unit}])");
                }
                if (errors.Any())
                {
                    message = $"Sản phẩm có mã [{partlist.ProductCode}] đã có tích xanh.\nCác trường không khớp: {string.Join(" ", errors)}. \nVui lòng kiểm tra lại.";
                    return false;
                }
            }

            return true;
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
                if (!CheckValidate(partlist,out errorMessage))
                {
                    message = errorMessage;
                    return false;
                }
            }
            if(partlist.IsDeleted == true)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] đã bị xóa!";
                return false;
            }
            if(isApproved && partlist.IsApprovedPurchase == true)
            {
                message = $"Vật tư thứ tự [{partlist.STT}] đã được Y/c mua.\nVui lòng kiểm tra lại!";
                return false;
            }
            if (isApproved && partlist.IsApprovedTBP==false)
            {
                message = $"Không thể {isAprrovedText} vì vật tư thứ tự [{partlist.STT}] chưa được TBP duyệt!";
                return false;
            }
            if(isApproved && partlist.IsApprovedTBPNewCode==false && partlist.IsNewCode==true)
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
            if (string.IsNullOrEmpty(item.ProductCode.Trim()))
            {
                message = $"[Mã thiết bị] có số thứ tự [{item.STT}] không được trống!\nVui lòng kiểm tra lại!";
                return false;
            }
            if (string.IsNullOrEmpty(item.GroupMaterial.Trim()))
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
            if( item.QtyFull == null || item.QtyFull <= 0)
            {
                message = $"[Số lượng tổng] có số thứ tự [{item.STT}] phải lớn hơn 0!\nVui lòng kiểm tra lại!";
                return false;
            }
            return true;
        }
    }
}