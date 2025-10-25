using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class ConfigSystemRepo : GenericRepo<ConfigSystem>
    {

        /// <summary>
        /// Lấy đường dẫn upload từ bảng ConfigSystem theo key
        /// </summary>
        /// <param name="keyName">Key để tìm đường dẫn</param>
        /// <returns>Đường dẫn upload hoặc null nếu không tìm thấy</returns>
        public string? GetUploadPathByKey(string keyName)
        {
            var config = GetAll(x => x.KeyName == keyName).FirstOrDefault();
            return config?.KeyValue;
        }
    }
}