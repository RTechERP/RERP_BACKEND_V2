namespace RERPAPI.Model.DTO.ESL
{
    public class ESLBindRequest
    {
        public string store_code { get; set; }
        public string f1 { get; set; } // ESL Code
        public string f2 { get; set; } // Product Code
        public int f3 { get; set; } = 0;
        public string is_base64 { get; set; }
        public string sign { get; set; }
    }
}
