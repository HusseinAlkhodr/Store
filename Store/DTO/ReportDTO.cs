namespace Store.DTO
{
    public class ItemSale
    {
        public string Description { get; set; }
        public int QTY { get; set; }
        public long Rest { get; set; }
        public double Total { get; set; }
    }
    public class SalesReport
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public List<ItemSale> Sales { get; set; }
        public double Total { get; set; }
    }

}
