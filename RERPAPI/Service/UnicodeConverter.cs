namespace RERPAPI.Service
{
    public static class UnicodeConverter
    {
        private static readonly Dictionary<string, string> _unicodeMap;
        static UnicodeConverter()
        {
            _unicodeMap = new Dictionary<string, string>()
            {
                { "à", "a" }, { "á", "a" }, { "ả", "a" }, { "ã", "a" }, { "ạ", "a" },
                { "ă", "a" }, { "ắ", "a" }, { "ằ", "a" }, { "ẳ", "a" }, { "ẵ", "a" }, { "ặ", "a" },
                { "â", "a" }, { "ấ", "a" }, { "ầ", "a" }, { "ẩ", "a" }, { "ẫ", "a" }, { "ậ", "a" },
                { "è", "e" }, { "é", "e" }, { "ẻ", "e" }, { "ẽ", "e" }, { "ẹ", "e" },
                { "ê", "e" }, { "ế", "e" }, { "ề", "e" }, { "ể", "e" }, { "ễ", "e" }, { "ệ", "e" },
                { "ì", "i" }, { "í", "i" }, { "ỉ", "i" }, { "ĩ", "i" }, { "ị", "i" },
                {  "\u1EC9","o"}, // ơ
                {"ò","o"}, {"ó","o"}, {"ỏ","o"}, {"õ","o"}, {"ọ","o"},
                {"ô","o"}, {"ố","o"}, {"ồ","o"}, {"ổ","o"}, {"ỗ","o"}, {"ộ","o"},
                {"ờ","o"}, {"ớ","o"}, {"ở","o"}, {"ỡ","o"}, {"ợ","o"},
                {"ù","u"}, {"ú","u"}, {"ủ","u"}, {"ũ","u"}, {"ụ","u"},
                {"ư","u"}, {"ứ","u"}, {"ừ","u"}, {"ử","u"}, {"ữ","u"}, {"ự","u"},
                {"ỳ","y"}, {"ý","y"}, {"ỷ","y"}, {"ỹ","y"}, {"ỵ","y"},
                // Dấu câu
                //{"-",".-"},{"_",".-."},{"!","-.-"},{"?",".--.."},{"'",".----."},{"\"",".-..-."},
            };
        }
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
            foreach (var kv in _unicodeMap)
            {
                if (type == 0)
                {
                    if (text.Contains(kv.Value))
                        text = text.Replace(kv.Value, kv.Key);
                }
                else
                {
                    if (text.Contains(kv.Key))
                        text = text.Replace(kv.Key, kv.Value);
                }
            }
            return text;
        }
    }
}
