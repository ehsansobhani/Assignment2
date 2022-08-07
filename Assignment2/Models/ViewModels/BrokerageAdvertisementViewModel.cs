namespace Assignment2.Models.ViewModels
{
    public class BrokerageAdvertisementViewModel
    {
        public IEnumerable<Advertisement> Advertisements { get; set; }
        public IEnumerable<Brokerage> Brokerages { get; set; }
    }
}
