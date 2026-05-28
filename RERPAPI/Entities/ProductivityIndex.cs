using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProductivityIndex
{
    public int ID { get; set; }

    public decimal? SalesMinorAccPrescale { get; set; }

    public decimal? SalePrescaleBigAccount { get; set; }

    public decimal? SalePCB { get; set; }

    public decimal? SaleVisionID { get; set; }

    public decimal? SaleBigAccountVisionID { get; set; }

    public decimal? SaleOther { get; set; }

    public decimal? POAmountPrescaleMinorAcc { get; set; }

    public decimal? POBigAccountFilm { get; set; }

    public decimal? POAmountPCB { get; set; }

    public decimal? POVisionIDMinorAcc { get; set; }

    public decimal? POBigAccountVisionID { get; set; }

    public decimal? POOther { get; set; }

    public decimal? TotalPOAmount { get; set; }

    public decimal? CallDSKHOther { get; set; }

    public decimal? CallDSKHBigAccount { get; set; }

    public decimal? DemoTest { get; set; }

    public decimal? VisitCustomerOther { get; set; }

    public decimal? VisitCustomerBigAccount { get; set; }

    public decimal? NumberCompanyBuy { get; set; }

    public decimal? NumberPO { get; set; }

    public decimal? NumberQuatation { get; set; }

    public decimal? NewAccount { get; set; }

    public decimal? TotalPerformance { get; set; }
}
