using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
    public class NewsletterWithFilesDTO
    {
        public int ID { get; set; }

        public string? Code { get; set; }

        public string? Title { get; set; }

        public string? NewsletterContent { get; set; }

        public int? Type { get; set; }

        public string? Image { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public string? OriginImgPath { get; set; }

        public string? ServerImgPath { get; set; }

        public string? NewsletterTypeCode { get; set; }

        public string? NewsletterTypeName { get; set; }

        public List<NewsletterFile>? NewsletterFiles { get; set; }
    }
}
