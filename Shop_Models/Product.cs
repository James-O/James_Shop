using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Models
{
    public class Product
    {
        public Product()
        {
            Quantity = 1;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        [Range(1,int.MaxValue)]
        public double Price { get; set; }
        public string Image { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        [DisplayName("Application")]
        public int ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; }

        [NotMapped]
        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }
    }
}
