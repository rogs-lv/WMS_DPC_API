using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class Shipment
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int AbsEntryF { get; set; }
        public string BinCode { get; set; }
        public string FromWhsCod { get; set; }
        public int AbsEntryT { get; set; }
        public string WhsCode { get; set; }
        public decimal Quantity { get; set; }
        public string U_LoteSAP { get; set; }
        public int Status { get; set; }
        public string U_stFolio { get; set; }
        public string OC { get; set; }
        public string GR { get; set; }
    }

    public class ValidateEFEEM
    {
        public string U_stFolio { get; set; }
        public string OC { get; set; }
        public string GR { get; set; }
    }
}
