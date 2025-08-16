using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RERPAPI.Model.Entities
{
   
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

    
        public int? CategoryId { get; set; }
    }
}
