using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo
{
    public static class UnicodeConverterService
    {
        private static readonly Dictionary<string, string> _unicodeMap = new Dictionary<string, string>
    {
        { "ạ", "ạ" }, { "ả", "ả" }, { "ã", "ã" },
        { "ẵ", "ẵ" }, { "ắ", "ắ" }, { "ặ", "ặ" }, { "ằ", "ằ" },
        { "ẫ", "ẫ" }, { "ấ", "ấ" }, { "ậ", "ậ" }, { "ầ", "ầ" }, { "ẩ", "ẩ" },
        { "ẽ", "ẽ" }, { "ẹ", "ẹ" }, { "ẻ", "ẻ" },
        { "ễ", "ễ" }, { "ế", "ế" }, { "ệ", "ệ" }, { "ề", "ề" }, { "ể", "ể" },
        { "ỡ", "ỡ" }, { "ớ", "ớ" }, { "ợ", "ợ" }, { "ờ", "ờ" }, { "ở", "ở" },
        { "ữ", "ữ" }, { "ứ", "ứ" }, { "ự", "ự" }, { "ừ", "ừ" }, { "ử", "ử" },
        { "ỗ", "ỗ" }, { "ố", "ố" }, { "ộ", "ộ" }, { "ồ", "ồ" }, { "ổ", "ổ" },
        { "ĩ", "ĩ" }, { "ị", "ị" }, { "iỉ", "ỉ" },
        { "ũ", "ũ" }, { "ụ", "ụ" }, { "ủ", "ủ" },
        { "õ", "õ" }, { "ọ", "ọ" }, { "ỏ", "ỏ" },
        { "ỹ", "ỹ" }, { "ỷ", "ỷ" }, { "ỵ", "ỵ" }
    };

        /// <summary>
        /// Chuyển đổi giữa unicode dựng sẵn và unicode tổ hợp
        /// </summary>
        /// <param name="text">chuỗi cần chuyển đổi</param>
        /// <param name="type">
        /// type = 0: dựng sẵn sang tổ hợp  
        /// type = 1: tổ hợp sang dựng sẵn
        /// </param>
        /// <returns></returns>
        public static string ConvertUnicode(string text, int type)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            foreach (var kv in _unicodeMap)
            {
                if (type == 0) // dựng sẵn → tổ hợp
                {
                    if (text.Contains(kv.Value))
                        text = text.Replace(kv.Value, kv.Key);
                }
                else // tổ hợp → dựng sẵn
                {
                    if (text.Contains(kv.Key))
                        text = text.Replace(kv.Key, kv.Value);
                }
            }
            return text;
        }
    }
}
