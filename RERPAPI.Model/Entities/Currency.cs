using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Currency
{
    public int ID { get; set; }

    /// <summary>
    /// Mã loại tiền
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên tiếng anh
    /// </summary>
    public string? NameEnglist { get; set; }

    /// <summary>
    /// Tên tiếng việt
    /// </summary>
    public string? NameVietNamese { get; set; }

    /// <summary>
    /// Giá trị tối thiểu
    /// </summary>
    public string? MinUnit { get; set; }

    /// <summary>
    /// Tỷ giá
    /// </summary>
    public decimal? CurrencyRate { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Ngày hết hạn
    /// </summary>
    public DateTime? DateExpried { get; set; }

    /// <summary>
    /// Ngày hiệu lực
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Tỷ giá hạn ngạch
    /// </summary>
    public decimal? CurrencyRateOfficialQuota { get; set; }

    /// <summary>
    /// Tỷ giá tiểu ngạch
    /// </summary>
    public decimal? CurrencyRateUnofficialQuota { get; set; }

    /// <summary>
    /// Ngày hết hạn tỷ giá hạn ngạch
    /// </summary>
    public DateTime? DateExpriedOfficialQuota { get; set; }

    /// <summary>
    /// Ngày hết hạn tỷ giá tiểu ngạch
    /// </summary>
    public DateTime? DateExpriedUnofficialQuota { get; set; }

    public bool? IsDeleted { get; set; }
}
