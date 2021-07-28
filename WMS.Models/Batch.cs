using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Batch
    {
        public int AbsEntryBatch { get; set; }
        public string DistNumber { get; set; }
        public string BinCode { get; set; }
        public int AbsEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string WhsCode { get; set; }
        public int Status { get; set; }
        public int ItmsGrpCod { get; set; }
        public string U_stFolio { get; set; }
    }
    public class BatchsInLocation
    {
        public string DistNumber { get; set; }
        public string BinCode { get; set; }
        public int AbsEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string WhsCode { get; set; }
        public int U_TarimasMax { get; set; }
    }

    public class BatchADSL
    {
        public int AbsEntryBatch { get; set; }
        public string DistNumber { get; set; }
        public string BinCode { get; set; }
        public int AbsEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string WhsCode { get; set; }
        public int Status { get; set; }
        public DateTime InDate { get; set; }
        public int Estancia { get; set; }
        public int PriceList { get; set; }
        public decimal Price { get; set; }
    }
}
