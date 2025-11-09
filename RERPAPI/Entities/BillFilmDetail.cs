using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillFilmDetail
{
    public int ID { get; set; }

    public int? ProductFimID { get; set; }

    public int? BillID { get; set; }

    public int? Qty { get; set; }

    public int? NeededBigBoxQty { get; set; }
}
