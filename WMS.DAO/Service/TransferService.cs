using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WMS.DAO.IService;
using WMS.Entities;
using WMS.Models;
using WMS.Utilities;

namespace WMS.DAO.Service
{
    public class TransferService : ITransferService
    {
        IDBAdapter dBAdapter;
        Log lg;
        private readonly string schema;
        public TransferService()
        {
            dBAdapter = DBFactory.GetDefaultAdapater();
            lg = Log.getIntance();
            schema = ConfigurationManager.AppSettings["schema"];
        }
        #region location movement
        public List<AvailableLocation> AvailableLocations(string warehouseUser)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var availableLocation = connection.Query<AvailableLocation>($"{schema}.\"WMS_AvailableLocations\"(warehouse=> '{warehouseUser}')").ToList();
                return availableLocation;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<AvailableLocation>();
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
        public List<BatchsInLocation> ViewLocation(string location, string warehouseInventory)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var locations = connection.Query<BatchsInLocation>($"{schema}.\"WMS_BatchsInLocation\"(location => '{location}', warehouse => '{warehouseInventory}');").ToList();
                return locations;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<BatchsInLocation>();
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
        public Location AbsEntryFromBinCode(string warehouse, string bincode, string type)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            Regex regex = new Regex(@"([A-Z]+\-PISO)");
            try
            {
                Match match = regex.Match(bincode);
                Location locationFromBinCode;
                if (match.Success)
                    locationFromBinCode = connection.Query<Location>($"{schema}.\"WMS_AbsEntryFromBinCode\"(warehouse => '{warehouse}', bincode => '', type_ => 'piso');").FirstOrDefault();
                else
                    locationFromBinCode = connection.Query<Location>($"{schema}.\"WMS_AbsEntryFromBinCode\"(warehouse => '{warehouse}', bincode => '{bincode}', type_ => 'nopiso');").FirstOrDefault();
                return locationFromBinCode;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Location();
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
        #endregion
        #region transfer request
        public List<OpenTransferRequest> OpenTransfersRequests(string warehouseUser)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                var openDocuments = connection.Query<OpenTransferRequest>($"{schema}.\"WMS_GetTransferRequest\"(warehouse => '{warehouseUser}', numberRequest => {0});").ToList();
                return openDocuments;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<OpenTransferRequest>();
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
        public DocumentTransfer GetDocumentRequest(string warehouse, string number)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                mult = connection.QueryMultiple($"{schema}.\"WMS_GetTransferRequest\"(warehouse => '{warehouse}', numberRequest => {number});");
                DocumentTransfer document = new DocumentTransfer();
                document.Document = mult.Read<TransferRequest>().FirstOrDefault();
                document.Detail = mult.Read<DetailTransferRequest>().ToList();
                return document;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new DocumentTransfer();
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
        #endregion
        #region transfer receipt
        public DocumentTransferReceipt GetDocumentReceipt(string warehouse, int docnum) {
            IDbConnection connection = dBAdapter.GetConnection();
            SqlMapper.GridReader mult;
            try
            {
                DocumentTransferReceipt document = new DocumentTransferReceipt();
                mult = connection.QueryMultiple($"{schema}.\"WMS_GetTransferReceipt\"(warehouse => '{warehouse}', docNum => {docnum});");
                
                document.Document = mult.Read<TransferReceipt>().FirstOrDefault();
                document.DetailDocument = mult.Read<DetailTransferReceipt>().ToList();
                return document;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new DocumentTransferReceipt();
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
        public Response<Transfer> ProcessReceipt(List<Batch> batchs, DocumentTransferReceipt receipt) {
            IDbConnection connection = dBAdapter.GetConnection();
            Guid guid;
            try
            {
                DefaultLocationWhs defaultLocation = connection.Query<DefaultLocationWhs>($"{schema}.\"WMS_BinDeftWarehouse\"(warehouse=>'{receipt.Document.U_Destino}');").FirstOrDefault();
                guid = Guid.NewGuid();
                int rowExecute = 0;
                foreach (Batch row in batchs)
                {
                    rowExecute = connection.Execute(
                        $"{schema}.\"WMS_SaveMovement\"(" +
                        $"ItemCode=>'{row.ItemCode}',From_=>'{row.WhsCode}',To_=>'{(receipt.Document.U_Destino)}'" +
                        $",Batch=>'{row.DistNumber}',WhsBatch=>'{row.WhsCode}',FromLocation=>''" +
                        $",ToLocation=>'{(defaultLocation != null ? defaultLocation.DftBinAbs.ToString() : "")}',QToLocation=>{row.Quantity},IdGUID=>'{guid.ToString()}')");

                }
                if (rowExecute > 0)
                    return new Response<Transfer>(BuildReceipt(guid.ToString(), receipt, connection), 0, "");
                else
                    return new Response<Transfer>(null, -1, "No se se procesaron registros para el documento");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<Transfer>(null, -1, ex.Message);
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
        #endregion
        #region common
        public List<Batch> ReadCode(string codebars, string warehouse)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                List<Batch> listBatchs = connection.Query<Batch>($"{schema}.\"WMS_BatcNumber\"(batch=>'{codebars}', warehouse=> '{warehouse}');").ToList();
                return listBatchs;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new List<Batch>();
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
        public DefaultLocationWhs LocationWarehouse(string warehouse) {
            IDbConnection connection = dBAdapter.GetConnection();
            try
            {
                DefaultLocationWhs defaultLocation = connection.Query<DefaultLocationWhs>($"{schema}.\"WMS_BinDeftWarehouse\"(warehouse=>'{warehouse}');").FirstOrDefault();
                if (defaultLocation == null)
                    return new DefaultLocationWhs();
                else
                    return defaultLocation;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new DefaultLocationWhs();
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
        public Response<Transfer> ProcessMovement(List<Batch> batchs, DocumentTransfer request)
        {
            IDbConnection connection = dBAdapter.GetConnection();
            Guid guid;
            try
            {
                DefaultLocationWhs defaultLocation = connection.Query<DefaultLocationWhs>($"{schema}.\"WMS_BinDeftWarehouse\"(warehouse=>'{request.Document.ToWhsCode}');").FirstOrDefault();
                guid = Guid.NewGuid();
                int rowExecute = 0;
                foreach (Batch row in batchs) {
                    rowExecute = connection.Execute(
                        $"{schema}.\"WMS_SaveMovement\"(" +
                        $"ItemCode=>'{row.ItemCode}',From_=>'{row.WhsCode}',To_=>'{(request.Document.ToWhsCode != "CICPR" ? "TRANSITO" : request.Document.ToWhsCode)}'" +
                        $",Batch=>'{row.DistNumber}',WhsBatch=>'{row.WhsCode}',FromLocation=>'{row.AbsEntry}'" +
                        $",ToLocation=>'{(request.Document.ToWhsCode != "CICPR" ? "" : defaultLocation == null ? "" : defaultLocation.DftBinAbs.ToString())}',QToLocation=>{row.Quantity},IdGUID=>'{guid.ToString()}')");

                }
                if (rowExecute > 0)
                    return new Response<Transfer>(BuildDocument(guid.ToString(), request, connection), 0, "");
                else
                    return new Response<Transfer>(null, -1, "No se se procesaron registros para el documento");
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Response<Transfer>(null, -1, ex.Message);
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
        private Transfer BuildReceipt(string guid, DocumentTransferReceipt receipt, IDbConnection connection)
        {
            try
            {
                Transfer document = new Transfer();

                document.Series = 0;
                document.DocDate = DateTime.Now.ToString("yyyy-MM-dd");
                document.FromWarehouse = receipt.Document.ToWhsCode;
                document.ToWarehouse = receipt.Document.U_Destino;
                document.U_Destino = "";
                document.U_OrigenMov = "HH";
                document.U_UsrHH = "";
                document.U_FechaMov = DateTime.Now.ToString("yyyy-MM-dd");
                document.U_HoraMov = DateTime.Now.ToString("HH:mm:ss");
                //Lines
                foreach (var line in GetLines(connection, guid))
                {
                    TransferLine row = new TransferLine();
                    row.ItemCode = line.ItemCode;
                    row.Quantity = line.Quantity;
                    row.FromWarehouseCode = line.From;
                    row.WarehouseCode = line.To;
                    document.StockTransferLines.Add(row);

                    foreach (var batch in GetBatchs(connection, guid, line.ItemCode, line.LineNum))
                    {
                        BatchNumbers distNumber = new BatchNumbers();
                        distNumber.BatchNumber = batch.Batch;
                        distNumber.Location = line.From;
                        distNumber.Quantity = batch.Quantity;
                        distNumber.BaseLineNumber = line.LineNum;
                        row.BatchNumbers.Add(distNumber);
                        foreach (var from_ in GetLocations(connection, guid, line.ItemCode, line.LineNum, batch.LineNumBatch, batch.Batch, 2))
                        {
                            if (from_.FromLocation > 0)
                            {
                                BinLocation FromLocation = new BinLocation();
                                FromLocation.BinAbsEntry = from_.FromLocation;
                                FromLocation.Quantity = from_.Quantity;
                                FromLocation.SerialAndBatchNumbersBaseLine = batch.LineNumBatch;
                                FromLocation.BinActionType = 2;
                                FromLocation.BaseLineNumber = line.LineNum;
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
                                ToLocation.BaseLineNumber = line.LineNum;
                                row.StockTransferLinesBinAllocations.Add(ToLocation);
                            }
                        }
                    }
                }
                return document;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Transfer();
            }
        }
        private Transfer BuildDocument(string guid, DocumentTransfer request, IDbConnection connection) {
            try
            {
                Transfer document = new Transfer();

                string whsTransito = "TRANSITO";
                string toLocation = request.Document.ToWhsCode != "CICPR" ? whsTransito : request.Document.ToWhsCode;
                
                document.Series = 0;
                document.DocDate = DateTime.Now.ToString("yyyy-MM-dd");
                document.FromWarehouse = request.Document.Filler;
                document.ToWarehouse = toLocation;
                document.U_Destino = request.Document.ToWhsCode != "CICPR" ? whsTransito : "";
                document.U_OrigenMov = "HH";
                document.U_UsrHH = "";
                document.U_FechaMov = DateTime.Now.ToString("yyyy-MM-dd");
                document.U_HoraMov = DateTime.Now.ToString("HH:mm:ss");
                //Lines
                foreach (var line in GetLines(connection, guid)) {
                    TransferLine row = new TransferLine();
                    row.ItemCode = line.ItemCode;
                    row.Quantity = line.Quantity;
                    row.FromWarehouseCode = line.From;
                    row.WarehouseCode = line.To;
                    if (request.Document.DocEntry > 0)
                    {
                        row.BaseEntry = request.Document.DocEntry;
                        row.BaseLine = GetBaseLineRequest(request.Detail, line.ItemCode);
                        row.BaseType = "InventoryTransferRequest";
                    }
                    document.StockTransferLines.Add(row);

                    foreach (var batch in GetBatchs(connection, guid, line.ItemCode, line.LineNum)) {
                        BatchNumbers distNumber = new BatchNumbers();
                        distNumber.BatchNumber = batch.Batch;
                        distNumber.Location = line.From;
                        distNumber.Quantity = batch.Quantity;
                        distNumber.BaseLineNumber = line.LineNum;
                        row.BatchNumbers.Add(distNumber);
                        foreach (var from_ in GetLocations(connection, guid, line.ItemCode, line.LineNum, batch.LineNumBatch, batch.Batch, 2)) {
                            BinLocation FromLocation = new BinLocation();
                            FromLocation.BinAbsEntry = from_.FromLocation;
                            FromLocation.Quantity = from_.Quantity;
                            FromLocation.SerialAndBatchNumbersBaseLine = batch.LineNumBatch;
                            FromLocation.BinActionType = 2;
                            FromLocation.BaseLineNumber = line.LineNum;
                            row.StockTransferLinesBinAllocations.Add(FromLocation);
                        }
                        foreach (var to_ in GetLocations(connection, guid, line.ItemCode, line.LineNum, batch.LineNumBatch, batch.Batch, 1)) {
                            BinLocation ToLocation = new BinLocation();
                            ToLocation.BinAbsEntry = to_.FromLocation;
                            ToLocation.Quantity = to_.Quantity;
                            ToLocation.SerialAndBatchNumbersBaseLine = batch.LineNumBatch;
                            ToLocation.BinActionType = 1;
                            ToLocation.BaseLineNumber = line.LineNum;
                            row.StockTransferLinesBinAllocations.Add(ToLocation);
                        }
                    }
                }
                return document;
            }
            catch (Exception ex)
            {
                lg.Registrar(ex, this.GetType().FullName);
                return new Transfer();
            }
        }
        private List<DocumentLine> GetLines(IDbConnection connection, string guid) {
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
        private List<DocumentBatch> GetBatchs(IDbConnection connection, string guid, string ItemCode, int NumLine) {
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
        private List<DocumentLocation> GetLocations(IDbConnection connection, string guid, string itemCode, int baseLine, int baseLineBatch, string Batch, int type) {
            try
            {
                List<DocumentLocation> result = null;
                if (type == 2) { // From
                    result = connection.Query<DocumentLocation>($"{schema}.\"WMS_FromLocation\"(guid=>'{guid}', ItemCode=>'{itemCode}', BaseLine=>{baseLine}, BaseLineBatch=>{baseLineBatch}, Batch=>'{Batch}');").ToList();
                }else if (type == 1) { // To
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
        private int GetBaseLineRequest(List<DetailTransferRequest> Detail, string ItemCode) {
            int NumLi = -1;
            foreach (DetailTransferRequest detail in Detail)
            {
                if (detail.ItemCode == ItemCode)
                {
                    NumLi = detail.LineNum;
                    break;
                }
            }
            return NumLi;
        }
        #endregion
    }
}
