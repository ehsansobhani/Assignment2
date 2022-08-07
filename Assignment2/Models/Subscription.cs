using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class Subscription
    {
        
        public int ClientId { get; set; }
        public string BrokerageId { get; set; }
        public virtual Client Client { get; set; }
        public virtual Brokerage Brokerage { get; set; }
    }
}
