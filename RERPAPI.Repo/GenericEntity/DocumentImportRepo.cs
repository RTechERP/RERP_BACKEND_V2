using RERPAPI.Model.DTO;
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
        BillDocumentImportRepo _billDocumentImportRepo;


        public DocumentImportRepo(CurrentUser currentUser, BillDocumentImportRepo billDocumentImportRepo) : base(currentUser)
        {
            _billDocumentImportRepo = billDocumentImportRepo;
        }
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
