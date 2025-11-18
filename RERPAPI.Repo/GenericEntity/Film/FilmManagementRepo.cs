    using RERPAPI.Model.DTO;
    using RERPAPI.Model.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace RERPAPI.Repo.GenericEntity.Film
    {
        public class FilmManagementRepo:GenericRepo<FilmManagement>
        {
            public FilmManagementRepo(CurrentUser currentUser) : base(currentUser)
            {
            }

            public int getMAXSTT()
            {
                int maxSTT = GetAll().Max(x => x.STT) ?? 0;
                return maxSTT;
            }
            public bool Validate(FilmManagement item, out string message)
            {
                message = "";
                bool exists = GetAll().Any(x => x.Code == item.Code && x.ID != item.ID && x.IsDeleted != true);
                if (exists)
                {
                    message = $"Mã phim  {item.Code}  đã tồn tại";
                    return false;
                }
            if (item.Code == null||item.Code=="")
            {
                message = "Mã phim không được để trống";
                return false;
            }
            return true;
            }
        }
    }
