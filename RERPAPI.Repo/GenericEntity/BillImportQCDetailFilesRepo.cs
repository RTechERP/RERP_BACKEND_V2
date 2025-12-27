using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportQCDetailFilesRepo : GenericRepo<BillImportQCDetailFile>
    {
        BillImportQCDetailFilesRepo _billImportQCDetailFilesRepo;
        public BillImportQCDetailFilesRepo(CurrentUser currentUser) : base(currentUser)
        {
            _billImportQCDetailFilesRepo = this;
        }

        public async Task UploadFiles(List<BillImportQCFileDTO> filesDto, int id, string pathUpload)
        {
            foreach (var file in filesDto)
            {
                if (file.File == null) continue;

                var fileOrder = _billImportQCDetailFilesRepo
                    .GetAll(x =>
                        x.FileName == file.FileName &&
                        x.BillImportQCDetailID == id &&
                        x.FileType == file.FileType
                    )
                    .FirstOrDefault() ?? new BillImportQCDetailFile();

                fileOrder.BillImportQCDetailID = id;
                fileOrder.FileName = file.FileName;
                fileOrder.ServerPath = pathUpload;
                fileOrder.FileType = file.FileType;

                if (!Directory.Exists(pathUpload))
                {
                    Directory.CreateDirectory(pathUpload);
                }

                var fullPath = Path.Combine(pathUpload, file.FileName!);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.File.CopyToAsync(stream);
                }

                if (System.IO.File.Exists(fullPath))
                {
                    if(fileOrder.ID > 0)
                    {
                        await _billImportQCDetailFilesRepo.UpdateAsync(fileOrder);
                    }
                    else await _billImportQCDetailFilesRepo.CreateAsync(fileOrder);
                }

            }
        }
    }
}
