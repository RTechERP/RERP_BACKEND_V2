namespace RERPAPI.Model.DTO
{
    public class PartlistImportRequestDTO
    {
        public int ProjectID { get; set; }
        public int ProjectPartListVersionID { get; set; }
        public int ProjectTypeID { get; set; }
        public string? ProjectCode { get; set; }

        /// <summary>
        /// Import dạng phát sinh hay không (nếu là form riêng phát sinh)
        /// </summary>
        public bool IsProblem { get; set; }

        public bool? CheckIsStock { get; set; }

        public List<ProjectPartlistImportRowDto>? Items { get; set; } = new();
        public int WarehouseID { get; set; }

        /// <summary>
        /// Danh sách diff (những dòng có sai khác Stock)
        /// FE sẽ thêm trường Choose = "Excel" hoặc "Stock"
        /// </summary>
        public List<PartlistDiffDTO>? Diffs { get; set; } = new();

        public bool? IsConsumable { get; set; }

        /// <summary>
        /// Danh sách ID vật tư cần bảo vệ (đã đặt hàng + cha của chúng).
        /// Khi overwrite, backend sẽ KHÔNG xóa những record này.
        /// </summary>
        public List<int>? ExcludeIds { get; set; } = new();
    }
}