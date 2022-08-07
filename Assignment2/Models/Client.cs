using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    
    public class Client
    {
        public int ID { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 1)]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 1)]
        [Required]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public ICollection<Subscription> Subscriptions { get; set; }

    }
}
