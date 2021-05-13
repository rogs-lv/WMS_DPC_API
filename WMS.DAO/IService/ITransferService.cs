﻿using System.Collections.Generic;
using WMS.Models;

namespace WMS.DAO.IService
{
    public interface ITransferService
    {
        #region location movement
        List<AvailableLocation> AvailableLocations(string warehouseUser);
        List<BatchsInLocation> ViewLocation(string location, string warehouseTransfer);
        Location AbsEntryFromBinCode(string warehouse, string bincode, string type = "");
        #endregion
        #region transfer request
        List<OpenTransferRequest> OpenTransfersRequests(string warehouseUser);
        DocumentTransfer GetDocumentRequest(string warehouse, string number);
        #endregion
        #region transfer receipt
        DocumentTransferReceipt GetDocumentReceipt(string warehouse, int docnum);
        Response<Transfer> ProcessReceipt(List<Batch> batchs, DocumentTransferReceipt receipt);
        #endregion
        #region manual
        Response<List<Warehouse>> ListWarehouses(string warehouseUser);
        Response<List<Whs_Binlocation>> ListWarehousePiso(string warehouse);
        Response<Transfer> ProcessManual(List<Batch> batchs, ManualToWhsCode newLocation);
        #endregion
        #region common
        List<Batch> ReadCode(string codebars, string warehouse);
        DefaultLocationWhs LocationWarehouse(string warehouse);
        Response<Transfer> ProcessMovement(List<Batch> batchs, DocumentTransfer request);
        #endregion
    }
}
