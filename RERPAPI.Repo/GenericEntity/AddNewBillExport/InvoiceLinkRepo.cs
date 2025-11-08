using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class InvoiceLinkRepo:GenericRepo<InvoiceLink>
    {
        public InvoiceLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        private List<InvoiceDTO> listInvoice = new List<InvoiceDTO>();
        #region  liên kết hóa đơn theo phiếu
        public async Task InvoiceLinkForBillImport(BillImportDetail detail)
        {
            List<InvoiceDTO> lst = listInvoice.Where(p => p.IdMapping == detail.STT).ToList();

            var invoicelink =GetAll(p => p.BillImportDetailID == detail.ID).FirstOrDefault();
            if (invoicelink != null)
            {
                invoicelink.IsDeleted = true;
                await UpdateAsync(invoicelink);
            }
            foreach (InvoiceDTO item in lst)
            {
                foreach (InvoiceLink model in item.Details)
                {
                    model.BillImportDetailID = detail.ID;
                    //InvoiceBO.Instance.Insert(model);
                    await CreateAsync(model);
                }
            }
        }
        #endregion
    }
}
