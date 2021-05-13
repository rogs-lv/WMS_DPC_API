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
    public class ShipmentService : IShipmentService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public ShipmentService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        public List<Shipment> GetNumberShipment(int docnum)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                List<Shipment> listBatchs = connection.Query<Shipment>($"{schema}.\"WMS_GetRequest\"(DocNum=>{docnum});").ToList();
                return listBatchs;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Shipment>();
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

        public List<Batch> ReadCode(string codebars, string warehouse)
        {
            throw new NotImplementedException();
        }

        public bool ApplyGR(string batch)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                bool apply = connection.Query<bool>($"{schema}.\"WMS_ApplyGR\"(batch=>'{batch}');").First();
                return apply;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return false;
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

        public ValidateEFEEM isValidEFEEM(string batch, int type)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var isValid = connection.Query<ValidateEFEEM>($"{schema}.\"WMS_ValidateEFEEM\"(batch=>'{batch}', event=>{type});").FirstOrDefault();
                return isValid;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new ValidateEFEEM();
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

        public int UpdateBatchs(string[] batchs, string status, int docnum, int docentry)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            string stringBatch = string.Empty;
            int updateBatch = 0;
            try
            {
                if (batchs.Length > 0)
                {
                    for (int a = 0; a < batchs.Length; a++)
                    {
                        if (stringBatch == string.Empty)
                            stringBatch = $"'{batchs[a]}'";
                        else
                            stringBatch += $",'{batchs[a]}'";
                    }
                    if (stringBatch != string.Empty)
                    {
                        updateBatch = connection.Execute($"UPDATE {schema.Replace("CALL","")}.\"OBTN\" SET \"U_stFolio\" = '{status}', \"U_Remision\" = {docnum} WHERE \"DistNumber\" IN({stringBatch});");
                    }
                }                        
                return updateBatch;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return 0;
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

        public Response<List<TransferShippment>> processShipment(List<shipmentProcess> data)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            Guid guid;
            try
            {
                guid = Guid.NewGuid();
                int rowExecute = 0;
                foreach (shipmentProcess row in data)
                {
                    rowExecute = connection.Execute(
                        $"{schema}.\"WMS_SaveMovement\"(" +
                        $"ItemCode=>'{row.ItemCode}',From_=>'{row.FromWhsCode}',To_=>'{ row.ToWhsCode}'" +
                        $",Batch=>'{row.DistNumber}',WhsBatch=>'{row.FromWhsCode}',FromLocation=>'{row.AbsEntry}'" +
                        $",ToLocation=>'{(row.AbsEntryT != -1 ? row.AbsEntryT.ToString() : "")}',QToLocation=>{row.Quantity},IdGUID=>'{guid.ToString()}')");

                }
                if (rowExecute > 0)
                {
                    var listDocuments = BuildShipment(guid.ToString(), data[0].FromWhsCode, data[0].ToWhsCode, data[0].OC, connection);
                    return new Response<List<TransferShippment>>(listDocuments, 0, "");
                }
                else
                    return new Response<List<TransferShippment>>(null, -1, "No se se procesaron registros para el documento");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<List<TransferShippment>>(null, -1, ex.Message);
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
        private List<TransferShippment> BuildShipment(string guid, string fromWarehouse, string toWarehouse, string OC,IDbConnection connection)
        {
            List<TransferShippment> listDocuments = new List<TransferShippment>();
            try
            {
                //Lines
                foreach (var line in GetLines(connection, guid))
                {
                    TransferShippment document = new TransferShippment();
                    int lineRow = 0;

                    document.Series = 0;
                    document.DocDate = DateTime.Now.ToString("yyyy-MM-dd");
                    document.FromWarehouse = fromWarehouse;
                    document.ToWarehouse = toWarehouse;
                    document.U_Destino = "";
                    document.U_OrigenMov = "HH";
                    document.U_UsrHH = "";
                    document.U_FechaMov = DateTime.Now.ToString("yyyy-MM-dd");
                    document.U_HoraMov = DateTime.Now.ToString("HH:mm:ss");
                    document.U_Remisionado = "Y";
                    document.Comments = OC;
                    document.U_ItemRem = line.ItemCode;
                    document.U_ItemRemQty = line.Quantity;

                    TransferLine row = new TransferLine();
                    row.ItemCode = line.ItemCode;
                    row.Quantity = line.Quantity;
                    row.FromWarehouseCode = line.From;
                    row.WarehouseCode = line.To;
                    document.StockTransferLines.Add(row);

                    foreach (var batch in GetBatchs(connection, guid, line.ItemCode, line.LineNum))
                    {
                        if (!string.IsNullOrEmpty(batch.Batch))
                        {
                            BatchNumbers distNumber = new BatchNumbers();
                            distNumber.BatchNumber = batch.Batch;
                            distNumber.Location = line.From;
                            distNumber.Quantity = batch.Quantity;
                            distNumber.BaseLineNumber = lineRow;//line.LineNum;
                            row.BatchNumbers.Add(distNumber);
                        }
                        foreach (var from_ in GetLocations(connection, guid, line.ItemCode, line.LineNum, batch.LineNumBatch, batch.Batch, 2))
                        {
                            if (from_.FromLocation > 0)
                            {
                                BinLocation FromLocation = new BinLocation();
                                FromLocation.BinAbsEntry = from_.FromLocation;
                                FromLocation.Quantity = from_.Quantity;
                                FromLocation.SerialAndBatchNumbersBaseLine = batch.LineNumBatch;
                                FromLocation.BinActionType = 2;
                                FromLocation.BaseLineNumber = lineRow;//line.LineNum;
                                row.StockTransferLinesBinAllocations.Add(FromLocation);
                            }
                        }
                        foreach (var to_ in GetLocations(connection, guid, line.ItemCode, line.LineNum, batch.LineNumBatch, batch.Batch, 1))
                        {
                            if (to_.ToLocation > 0)
                            {
                                BinLocation ToLocation = new BinLocation();
                                ToLocation.BinAbsEntry = to_.ToLocation;
                                ToLocation.Quantity = to_.Quantity;
                                ToLocation.SerialAndBatchNumbersBaseLine = batch.LineNumBatch;
                                ToLocation.BinActionType = 1;
                                ToLocation.BaseLineNumber = lineRow;//line.LineNum;
                                row.StockTransferLinesBinAllocations.Add(ToLocation);
                            }
                        }
                    }
                    listDocuments.Add(document);
                }
                return listDocuments;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<TransferShippment>();
            }
        }
        private List<DocumentLine> GetLines(IDbConnection connection, string guid)
        {
            try
            {
                var result = connection.Query<DocumentLine>($"{schema}.\"WMS_Lines\"(guid=>'{guid}');").ToList();
                return result;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<DocumentLine>();
            }
        }
        private List<DocumentBatch> GetBatchs(IDbConnection connection, string guid, string ItemCode, int NumLine)
        {
            try
            {
                var result = connection.Query<DocumentBatch>($"{schema}.\"WMS_Batchs\"(guid=>'{guid}', Item=>'{ItemCode}', NumLine=>{NumLine});").ToList();
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
                    result = connection.Query<DocumentLocation>($"{schema}.\"WMS_FromLocation\"(guid=>'{guid}', ItemCode=>'{itemCode}', BaseLine=>{baseLine}, BaseLineBatch=>{baseLineBatch}, Batch=>'{Batch}');").ToList();
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
