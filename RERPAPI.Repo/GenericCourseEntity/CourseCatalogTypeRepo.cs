using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseCatalogTypeRepo : GenericCourseRepo<CourseCatalogType>
    {
        public CourseCatalogTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool ValidateCourseType(CourseCatalogType data, out string message)
        {
            message = string.Empty;

            if (data == null)
            {
                message = $"Không có dữ liệu. Vui lòng kiểm tra lại!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.CourseCatalogTypeCode))
            {
                message = $"Vui lòng nhập mã loại khóa học!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.CourseCatalogTypeName))
            {
                message = $"Vui lòng nhập tên loại khóa học!";
                return false;
            }

            var exits = GetAll(x => x.ID != data.ID && x.IsDeleted == false && x.CourseCatalogTypeCode == data.CourseCatalogTypeCode);
            if (exits.Count > 0)
            {
                message = $"Mã loại khóa học [{data.CourseCatalogTypeCode}] đã được sử dụng!";
                return false;
            }
            return true;
        }
    }
}