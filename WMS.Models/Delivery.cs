using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Delivery
    {
        //public int DocEntry { get; set; }
        //public int DocNum { get; set; }
        public int DocObjectCode { get; set; }
        public string CardCode { get; set; }
        public string NumAtCard { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string Reference { get; set; }
        public string Comments { get; set; }
        public string U_OrigenMov { get; set; }
        public string U_UsrHH { get; set; }
        public string U_FechaMov { get; set; }
        public string U_HoraMov { get; set; }
        public List<DeliveryLines> DocumentLines { get; set; }
        public Delivery()
        {
            this.NumAtCard = "";
            this.Reference = "";
            this.Comments = "";
            DocumentLines = new List<DeliveryLines>();
        }
    }

    public class DeliveryLines
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string TaxCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public string WarehouseCode { get; set; }
        public List<BatchNumbers> BatchNumbers { get; set; }
        public List<BinLocation> DocumentLinesBinAllocations { get; set; }
        public DeliveryLines()
        {
            BatchNumbers = new List<BatchNumbers>();
            DocumentLinesBinAllocations = new List<BinLocation>();
        }
    }
    public class LinesDelivery
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public decimal Price { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Quantity { get; set; }
        public string Batch { get; set; }
        public decimal LineTotal { get; set; }
    }
}
