using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductFilm
{
    public int ID { get; set; }

    public int? ManufactureID { get; set; }

    public int? StockLocationID { get; set; }

    public int? StockID { get; set; }

    public int? ParentID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public decimal? Height { get; set; }

    public decimal? Width { get; set; }

    public decimal? Area { get; set; }

    public decimal? PcsPerBox { get; set; }

    public decimal? InventoryNumber { get; set; }

    public string? Description { get; set; }
}
