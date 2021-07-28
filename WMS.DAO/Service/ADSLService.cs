using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.Entities;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class ADSLService : IADSLService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public ADSLService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public Response<List<PartnerADSL>> GetListPartner()
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                //Create SP when DB is Ok
                //This SP not exist
                var listPartner = connection.Query<PartnerADSL>($"{schema}.\"WMS_GetPartnersADSL\"();").ToList();
                return new Response<List<PartnerADSL>>(listPartner, 0, "");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<List<PartnerADSL>>(null, -1, ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public List<BatchADSL> ReadCode(string codebars, string warehouse, int priceList)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                List<BatchADSL> listBatchs = connection.Query<BatchADSL>($"{schema}.\"WMS_BatcNumberADSL\"(batch=>'{codebars}', warehouse=> '{warehouse}', priceList=> {priceList});").ToList();
                return listBatchs;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<BatchADSL>();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        public Response<Delivery> ProcessDelivery(List<BatchADSL> data, string cardCode, string usuario)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            Guid guid;
            try
            {
                guid = Guid.NewGuid();
                int rowExecute = 0;
                foreach (var row in data)
                {
                    rowExecute = connection.Execute(
                        $"{schema}.\"WMS_SaveDelivery\"(" +
                        $"ItemCode=>'{row.ItemCode}',From_=>'{row.WhsCode}',To_=>''" +
                        $",Batch=>'{row.DistNumber}',WhsBatch=>'{row.WhsCode}',FromLocation=>'{row.AbsEntry}'" +
                        $",ToLocation=>'',QToLocation=>{row.Quantity},Estancia=>{row.Estancia}"+
                        $",PriceList=>{row.PriceList}, Price=>{row.Price},IdGUID=>'{guid.ToString()}')");

                }
                if (rowExecute > 0)
                    return new Response<Delivery>(BuildDelivery(guid.ToString(), cardCode, usuario, data[0].WhsCode,connection), 0, "");
                else
                    return new Response<Delivery>(null, -1, "No se se procesaron registros para la entrega");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<Delivery>(null, -1, ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
        private Delivery BuildDelivery(string guid, string CardCode, string usuario, string whsCode,IDbConnection connection)
        {
            try
            {
                Delivery delivery = new Delivery();

                delivery.CardCode = CardCode;
                delivery.DocDate = DateTime.Now; // Fecha de contabilización
                delivery.TaxDate = DateTime.Now; // Fecha del documento
                delivery.DocDueDate = DateTime.Now; // Fecha de entrega
                delivery.DocObjectCode = 15;
                delivery.U_OrigenMov = "HH";
                delivery.U_UsrHH = usuario;
                delivery.U_FechaMov = DateTime.Now.ToString("MM/dd/yyyy");
                delivery.U_HoraMov = DateTime.Now.ToString("HH:mm:ss");

                foreach(var row in GetLinesADSL(connection, guid))
                {
                    DeliveryLines line = new DeliveryLines();
                    line.LineNum = row.LineNum;
                    line.ItemCode = row.ItemCode;
                    line.Quantity = row.Quantity;
                    line.WarehouseCode = whsCode;
                    line.LineTotal = row.LineTotal;
                    line.DiscountPercent = 0;
                    delivery.DocumentLines.Add(line);

                    foreach(var distNum in GetBatchs(connection, guid, row.ItemCode, row.LineNum))
                    {
                        BatchNumbers distNumber = new BatchNumbers();
                        distNumber.BatchNumber = distNum.Batch;
                        distNumber.Location = whsCode;
                        distNumber.Quantity = distNum.Quantity;
                        distNumber.BaseLineNumber = row.LineNum;
                        line.BatchNumbers.Add(distNumber);
                        
                        foreach(var location in GetLocations(connection, guid, row.ItemCode, row.LineNum, distNum.LineNumBatch, distNum.Batch, 2))
                        {
                            if (location.FromLocation > 0)
                            {
                                BinLocation FromLocation = new BinLocation();
                                FromLocation.BinAbsEntry = location.FromLocation;
                                FromLocation.Quantity = location.Quantity;
                                FromLocation.SerialAndBatchNumbersBaseLine = distNum.LineNumBatch;
                                FromLocation.BinActionType = 2;
                                FromLocation.BaseLineNumber = row.LineNum;
                                line.DocumentLinesBinAllocations.Add(FromLocation);
                            }
                        }
                    }
                }
                return delivery;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Delivery();
            }
        }
        private List<LinesDelivery> GetLinesADSL(IDbConnection connection, string guid)
        {
            try
            {
                var result = connection.Query<LinesDelivery>($"{schema}.\"WMS_LinesADSL\"(guid=>'{guid}');").ToList();
                return result;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<LinesDelivery>();
            }
        }
        private List<DocumentBatch> GetBatchs(IDbConnection connection, string guid, string ItemCode, int NumLine)
        {
            try
            {
                var result = connection.Query<DocumentBatch>($"{schema}.\"WMS_BatchsADSL\"(guid=>'{guid}', Item=>'{ItemCode}', NumLine=>{NumLine});").ToList();
                return result;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<DocumentBatch>();
            }
        }
        private List<DocumentLocation> GetLocations(IDbConnection connection, string guid, string itemCode, int baseLine, int baseLineBatch, string Batch, int type)
        {
            try
            {
                List<DocumentLocation> result = null;
                if (type == 2)
                { // From
                    result = connection.Query<DocumentLocation>($"{schema}.\"WMS_FromLocationADSL\"(guid=>'{guid}', ItemCode=>'{itemCode}', BaseLine=>{baseLine}, BaseLineBatch=>{baseLineBatch}, Batch=>'{Batch}');").ToList();
                }
                else if (type == 1)
                { // To
                    result = connection.Query<DocumentLocation>($"{schema}.\"WMS_ToLocation\"(guid=>'{guid}', ItemCode=>'{itemCode}', BaseLine=>{baseLine}, BaseLineBatch=>{baseLineBatch}, Batch=>'{Batch}');").ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<DocumentLocation>();
            }
        }
    }
}
