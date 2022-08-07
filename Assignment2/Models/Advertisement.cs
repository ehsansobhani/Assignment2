using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public enum Broker
    {
        Alpha, Beta, Omega
    }
    public class Advertisement
    {
        [Key]
        public int AnswerImageId { get; set; }

        [Display(Name = "File Name")]
        [Required]
        public string FileName { get; set; }


        [Url]
        [Required]
        public string Url { get; set; }

        public Brokerage Brokerage { get; set; }

    }
}
