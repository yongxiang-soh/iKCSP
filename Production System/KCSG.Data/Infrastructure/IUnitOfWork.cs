using System;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Repositories;
using KCSG.Data.Repositories.ProductCertification;
using KCSG.Data.Repositories.ProductionPlanning;

namespace KCSG.Data.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        #region Repositories Declaration
        #region Production Planning
        MaterialRepository MaterialRepository { get; }
        ProductRepository ProductRepository { get; }
        PrePdtMkpRepository PrePdtMkpRepository { get; }
        PreProductRepository PreProductRepository { get; }
        PdtPlnRepository PdtPlnRepository { get; }
        PckMtrRepository PckMtrRepository { get; }
        SubMaterialRepository SubMaterialRepository { get; }
        PreProductPlanRepository PreProductPlanRepository { get; }

        SupplierRepossitories SupplierRepossitories { get; }
        MaterialShelfStockRepository MaterialShelfStockRepository { get; }
        #endregion

        #region Product Certification
        ProductCertificationRepository ProductCertificationRepository { get; }
                
        StorageOfProductRepository StorageOfProductRepository { get; }
        ProductCertiicationOutOfPlanRepository ProductCertiicationOutOfPlanRepository { get; }
        
                
        #endregion

        AccessRepository AccessRepository { get; }
        CalenderRepository CalenderRepository { get; }
        CertificateHistoryRepository CertificateHistoryRepository { get; }
        ContainerRepository ContainerRepository { get; }
        ConveyorRepository ConveyorRepository { get; }
        DeviceRepository DeviceRepository { get; }
        EndUserRepository EndUserRepository { get; }
        EnvAvalRepository EnvAvalRepository { get; }
        EnvElseRepository EnvElseRepository { get; }
        EnvLotRepository EnvLotRepository { get; }
        EnvMespRepository EnvMespRepository { get; }
        EnvProdRepository EnvProdRepository { get; }
        EnvTempRepository EnvTempRepository { get; }
        FunctionRepository FunctionRepository { get; }
        KndCmdMsrSndHstRepository KndCmdMsrSndHstRepository { get; }
        KneadingCommandRepository KneadingCommandRepository { get; }
        KneadingRecordRepository KneadingRecordRepository { get; }
        MaterialShelfRepository MaterialShelfRepository { get; }
        MaterialShelfStatusRepository MaterialShelfStatusRepository { get; }
        MaterialStorageRetrieveHistoryRepository MaterialStorageRetrieveHistoryRepository { get; }
        MaterialTotalRepository MaterialTotalRepository { get; }
        MaterialWarehouseCommandRepository MaterialWarehouseCommandRepository { get; }
        MaterialWarehouseCommandHistoryRepository MaterialWarehouseCommandHistoryRepository { get; }
        MtrMsrSndCmdHstRepository MtrMsrSndCmdHstRepository { get; }
        MtrMsrSndCmdRepository MtrMsrSndCmdRepository { get; }
        MtrRtrMsrSndCmdHstRepository MtrRtrMsrSndCmdHstRepository { get; }
        MtrRtrMsrSndCmdRepository MtrRtrMsrSndCmdRepository { get; }
        NoManageRepository NoManageRepository { get; }
        OutOfPlanProductRepository OutOfPlanProductRepository { get; }
        PasswordRepository PasswordRepository { get; }
        PreProductShelfStatusRepository PreProductShelfStatusRepository { get; }
        PreProductShelfStockRepository PreProductShelfStockRepository { get; }
        PreProductStorageRetrieveHistoryRepository PreProductStorageRetrieveHistoryRepository { get; }
        PreProductTotalRepository PreProductTotalRepository { get; }
        PreProductWarehouseCommandHistoryRepository PreProductWarehouseCommandHistoryRepository { get; }
        ProductShelfRepository ProductShelfRepository { get; }
        ProductShelfStatusRepository ProductShelfStatusRepository { get; }
        ProductShelfStockRepository ProductShelfStockRepository { get; }
        ProductStorageRetrieveHistoryRepository ProductStorageRetrieveHistoryRepository { get; }
        ProductTotalRepository ProductTotalRepository { get; }
        ProductWarehouseCommandHistoryRepository ProductWarehouseCommandHistoryRepository { get; }
        ReceptionRepository ReceptionRepository { get; }
        SupMaterialStockRepository SupMaterialStockRepository { get; }
        TabletCommandRepository TabletCommandRepository { get; }
        TabletProductRepository TabletProductRepository { get; }
        TerminalRepository TerminalRepository { get; }
        TermStatusRepository TermStatusRepository { get; }
        TrmPicMgnRepository TrmPicMgnRepository { get; }
        KndCmdMsrSndRepository KndCmdMsrSndRepository { get; }
        OutSidePrePdtStkRepository OutSidePrePdtStkRepository { get; }
        PdtShipHstRepository PdtShipHstRepository { get; }
        ShipCommandRepository ShipCommandRepository { get; }
        ShippingPlanRepository ShippingPlanRepository { get; }
        PreProductWarehouseCommandRepository PreProductWarehouseCommandRepository { get; }
        ProductWarehouseCommandRepository ProductWarehouseCommandRepository { get; }

        /// <summary>
        /// Repository which provides function to access realtime connection.
        /// </summary>
        RealtimeRepository RealtimeConnectionRepository { get; }

        #endregion

        /// <summary>
        /// Accessor of KCSG DbContext.
        /// </summary>
        IKCSGDbContext Context { get; }

        /// <summary>
        /// Save changes into database sychronously.
        /// </summary>
        void Commit();

        /// <summary>
        /// Save all works asynchronously into database.
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();
    }
}
