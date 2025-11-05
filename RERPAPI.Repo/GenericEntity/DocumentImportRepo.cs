using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class DocumentImportRepo : GenericRepo<DocumentImport>
    {
        BillDocumentImportRepo _billDocumentImportRepo = new BillDocumentImportRepo();
        #region thêm chứng từ 
        public async Task NewDocumentImport(int billImportId)
        {
            var listID = GetAll(x => x.IsDeleted == false);
            foreach (var item in listID)
            {
                BillDocumentImport billDocumentImport = new BillDocumentImport
                {
                    BillImportID = billImportId,
                    DocumentImportID = item.ID,
                    DocumentStatus = 0,
                    CreatedDate = DateTime.Now,
                    Note = ""
                };
                await _billDocumentImportRepo.CreateAsync(billDocumentImport);
            }

        }
        #endregion
    }
}
