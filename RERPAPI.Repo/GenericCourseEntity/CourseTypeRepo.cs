using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities.RTCCourse;

namespace RERPAPI.Repo.GenericCourseEntity
{
    public class CourseTypeRepo : GenericCourseRepo<CourseType>
    {
        public CourseTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public bool ValidateCourseType(CourseType data, out string message)
        {
            message = string.Empty;

            if (data == null)
            {
                message = $"Không có dữ liệu. Vui lòng kiểm tra lại!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.CourseTypeCode))
            {
                message = $"Vui lòng nhập mã loại khóa học!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.CourseTypeName))
            {
                message = $"Vui lòng nhập tên loại khóa học!";
                return false;
            }

            var exits = GetAll(x => x.ID != data.ID && x.IsDeleted == false && x.CourseTypeCode == data.CourseTypeCode);
            if (exits.Count > 0)
            {
                message = $"Mã loại khóa học [{data.CourseTypeCode}] đã được sử dụng!";
                return false;
            }
            return true;
        }
    }
}