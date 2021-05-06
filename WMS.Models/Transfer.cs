using System;
using System.Collections.Generic;
using System.Windows;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WMS.Models
{
    public class BinLocation
    {
        public int BinAbsEntry { get; set; }
        public int BinActionType { get; set; }
        public int BaseLineNumber { get; set; }
        public decimal Quantity { get; set; }
        public int SerialAndBatchNumbersBaseLine { get; set; }        
    }
    public class BatchNumbers
    {
        public string BatchNumber { get; set; }
        public string Location { get; set; }
        public decimal Quantity { get; set; }
        public int BaseLineNumber { get; set; }
    }
    public class TransferLine
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public string WarehouseCode { get; set; }
        public string FromWarehouseCode { get; set; }
        public string BaseType { get; set; }
        public int BaseLine { get; set; }
        public int BaseEntry { get; set; }
        public List<BatchNumbers> BatchNumbers { get; set; }
        public List<BinLocation> StockTransferLinesBinAllocations { get; set; }
        public TransferLine()
        {
            BatchNumbers = new List<BatchNumbers>();
            StockTransferLinesBinAllocations = new List<BinLocation>();
        }
    }
    public class Transfer
    {
        public int Series { get; set; }
        public string DocDate { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string U_Destino { get; set; }
        public string U_OrigenMov { get; set; }
        public string U_UsrHH { get; set; }
        public string U_FechaMov { get; set; }
        public string U_HoraMov { get; set; }
        public List<TransferLine> StockTransferLines { get; set; }
        public Transfer()
        {
            StockTransferLines = new List<TransferLine>();
        }
    }
    public class OpenTransferRequest
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
    }
    public class TransferRequest
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int U_OF { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
    }
    public class DetailTransferRequest
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
    }
    public class DocumentTransfer
    {
        public TransferRequest Document { get; set; }
        public List<DetailTransferRequest> Detail { get; set; }
    }
    public class DataMovement
    {
        public List<Batch> batchs { get; set; }
        public DocumentTransfer request { get; set; }
    }
    public class TransferReceipt
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string U_Destino { get; set; }
    }
    public class DetailTransferReceipt
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
    }
    public class DocumentTransferReceipt
    {
        public TransferReceipt Document { get; set; }
        public List<DetailTransferReceipt> DetailDocument { get; set; }
    }
    public class DataReceipt
    {
        public List<Batch> batchs { get; set; }
        public DocumentTransferReceipt receipt { get; set; }
    }
    public class DataManual
    {
        public List<Batch> batchs { get; set; }
        public ManualToWhsCode manual { get; set; }
    }
}
