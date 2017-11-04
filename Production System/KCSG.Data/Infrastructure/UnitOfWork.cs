using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Repositories;
using KCSG.Data.Repositories.ProductCertification;
using KCSG.Data.Repositories.ProductionPlanning;

namespace KCSG.Data.Infrastructure
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        #region Protected Fields

        private IKCSGDbContext _context;

        #region Production Planning

        private MaterialRepository _materialRepository;
        private ProductRepository _productRepository;
        private PrePdtMkpRepository _prePdtMkpRepository;
        private PreProductRepository _preProductRepository;
        private PdtPlnRepository _pdtPlnRepository;
        private PckMtrRepository _pckMtrRepository;
        private SubMaterialRepository _subMaterialRepository;
        private PreProductPlanRepository _preProductPlanRepository;
        private MaterialShelfStockRepository _materialShelfStockRepository;
        private AccessRepository _accessRepository;
        private CalenderRepository _calenderRepository;
        private CertificateHistoryRepository _certificateHistoryRepository;
        private ContainerRepository _containerRepository;
        private ConveyorRepository _conveyorRepository;
        private DeviceRepository _deviceRepository;
        private EndUserRepository _endUserRepository;
        private EnvAvalRepository _envAvalRepository;
        private EnvElseRepository _envElseRepository;
        private EnvLotRepository _envLotRepository;
        private EnvMespRepository _envMespRepository;
        private EnvProdRepository _envProdRepository;
        private EnvTempRepository _envTempRepository;
        private FunctionRepository _functionRepository;
        private KndCmdMsrSndHstRepository _kndCmdMsrSndHstRepository;
        private KneadingCommandRepository _kneadingCommandRepository;
        private KneadingRecordRepository _kneadingRecordRepository;
        private MaterialShelfRepository _materialShelfRepository;
        private MaterialShelfStatusRepository _materialShelfStatusRepository;
        private MaterialStorageRetrieveHistoryRepository _materialStorageRetrieveHistoryRepository;
        private MaterialTotalRepository _materialTotalRepository;
        private MaterialWarehouseCommandRepository _materialWarehouseCommandRepository;
        private MaterialWarehouseCommandHistoryRepository _materialWarehouseCommandHistoryRepository;
        private MtrMsrSndCmdHstRepository _mtrMsrSndCmdHstRepository;
        private MtrMsrSndCmdRepository _mtrMsrSndCmdRepository;
        private MtrRtrMsrSndCmdHstRepository _mtrRtrMsrSndCmdHstRepository;
        private MtrRtrMsrSndCmdRepository _mtrRtrMsrSndCmdRepository;
        private NoManageRepository _noManageRepository;
        private OutOfPlanProductRepository _outOfPlanProductRepository;
        private PasswordRepository _passwordRepository;
        private PreProductShelfStatusRepository _preProductShelfStatusRepository;
        private PreProductShelfStockRepository _preProductShelfStockRepository;
        private PreProductStorageRetrieveHistoryRepository _preProductStorageRetrieveHistoryRepository;
        private PreProductTotalRepository _preProductTotalRepository;
        private PreProductWarehouseCommandHistoryRepository _preProductWarehouseCommandHistoryRepository;
        private ProductShelfRepository _productShelfRepository;
        private ProductShelfStatusRepository _productShelfStatusRepository;
        private ProductShelfStockRepository _productShelfStockRepository;
        private ProductStorageRetrieveHistoryRepository _productStorageRetrieveHistoryRepository;
        private ProductTotalRepository _productTotalRepository;
        private ProductWarehouseCommandHistoryRepository _productWarehouseCommandHistoryRepository;
        private ProductWarehouseCommandRepository _productWarehouseCommandRepository;
        private ReceptionRepository _receptionRepository;
        private SupMaterialStockRepository _supMaterialStockRepository;
        private TabletCommandRepository _tabletCommandRepository;
        private TabletProductRepository _tabletProductRepository;
        private TerminalRepository _terminalRepository;
        private TermStatusRepository _termStatusRepository;
        private TrmPicMgnRepository _trmPicMgnRepository;

        #endregion

        #region Product Certification

        private ProductCertificationRepository _productCertification;        
        private StorageOfProductRepository _storageOfProduct;
        private ProductCertiicationOutOfPlanRepository _productCertiicationOutOfPlanRepository;  

        #endregion

        private SupplierRepossitories _supplierRepossitories;
        private PdtShipHstRepository _pdtShipHstRepository;
        private KndCmdMsrSndRepository _kndCmdMsrSndRepository;
        private OutSidePrePdtStkRepository _outSidePrePdtStkRepository;
        private ShippingPlanRepository _shippingPlanRepository;
        private ShipCommandRepository _shipCommandRepository;
        private PreProductWarehouseCommandRepository _preProductWarehouseCommandRepository;

        private RealtimeRepository _realtimeRepository;

        #endregion

        public UnitOfWork(IKCSGDbContext context)
        {
            _context = context;
        }

        #region Production Planning
        public MaterialRepository MaterialRepository
        {
            get { return _materialRepository ?? (_materialRepository = new MaterialRepository(_context)); }
        }
        public SupplierRepossitories SupplierRepossitories
        {
            get { return _supplierRepossitories ?? (_supplierRepossitories = new SupplierRepossitories(_context)); }
        }

        public ProductRepository ProductRepository
        {
            get { return _productRepository ?? (_productRepository = new ProductRepository(_context)); }
        }

        public PrePdtMkpRepository PrePdtMkpRepository
        {
            get { return _prePdtMkpRepository ?? (_prePdtMkpRepository = new PrePdtMkpRepository(_context)); }
        }

        public PreProductRepository PreProductRepository
        {
            get { return _preProductRepository ?? (_preProductRepository = new PreProductRepository(_context)); }
        }

        public PdtPlnRepository PdtPlnRepository
        {
            get { return _pdtPlnRepository ?? (_pdtPlnRepository = new PdtPlnRepository(_context)); }
        }


        public PckMtrRepository PckMtrRepository
        {
            get { return _pckMtrRepository ?? (_pckMtrRepository = new PckMtrRepository(_context)); }
        }
        public SubMaterialRepository SubMaterialRepository
        {
            get { return _subMaterialRepository ?? (_subMaterialRepository = new SubMaterialRepository(_context)); }
        }

        public PreProductPlanRepository PreProductPlanRepository
        {
            get { return _preProductPlanRepository ?? (_preProductPlanRepository = new PreProductPlanRepository(_context)); }
        }
        public MaterialShelfStockRepository MaterialShelfStockRepository
        {
            get
            {
                return _materialShelfStockRepository ??
                       (_materialShelfStockRepository = new MaterialShelfStockRepository(_context));
            }
        }
        #endregion

        #region Product Certification
        public ProductCertificationRepository ProductCertificationRepository
        {
            get { return _productCertification ?? (_productCertification = new ProductCertificationRepository(_context)); }
        }
        public StorageOfProductRepository StorageOfProductRepository
        {
            get { return _storageOfProduct ?? (_storageOfProduct = new StorageOfProductRepository(_context)); }
        }
        public ProductCertiicationOutOfPlanRepository ProductCertiicationOutOfPlanRepository
        {
            get { return _productCertiicationOutOfPlanRepository ?? (_productCertiicationOutOfPlanRepository = new ProductCertiicationOutOfPlanRepository(_context)); }
        }

        
        
        #endregion
        public AccessRepository AccessRepository
        {
            get { return _accessRepository ?? (_accessRepository = new AccessRepository(_context)); }
        }
        public CalenderRepository CalenderRepository
        {
            get { return _calenderRepository ?? (_calenderRepository = new CalenderRepository(_context)); }
        }
        public CertificateHistoryRepository CertificateHistoryRepository
        {
            get { return _certificateHistoryRepository ?? (_certificateHistoryRepository = new CertificateHistoryRepository(_context)); }
        }
        public ContainerRepository ContainerRepository
        {
            get { return _containerRepository ?? (_containerRepository = new ContainerRepository(_context)); }
        }
        public ConveyorRepository ConveyorRepository
        {
            get { return _conveyorRepository ?? (_conveyorRepository = new ConveyorRepository(_context)); }
        }
        public DeviceRepository DeviceRepository
        {
            get { return _deviceRepository ?? (_deviceRepository = new DeviceRepository(_context)); }
        }
        public EndUserRepository EndUserRepository
        {
            get { return _endUserRepository ?? (_endUserRepository = new EndUserRepository(_context)); }
        }
        public EnvAvalRepository EnvAvalRepository
        {
            get { return _envAvalRepository ?? (_envAvalRepository = new EnvAvalRepository(_context)); }
        }
        public EnvElseRepository EnvElseRepository
        {
            get { return _envElseRepository ?? (_envElseRepository = new EnvElseRepository(_context)); }
        }
        public EnvLotRepository EnvLotRepository
        {
            get { return _envLotRepository ?? (_envLotRepository = new EnvLotRepository(_context)); }
        }
        public EnvMespRepository EnvMespRepository
        {
            get { return _envMespRepository ?? (_envMespRepository = new EnvMespRepository(_context)); }
        }
        public EnvProdRepository EnvProdRepository
        {
            get { return _envProdRepository ?? (_envProdRepository = new EnvProdRepository(_context)); }
        }
        public EnvTempRepository EnvTempRepository
        {
            get { return _envTempRepository ?? (_envTempRepository = new EnvTempRepository(_context)); }
        }
        public FunctionRepository FunctionRepository
        {
            get { return _functionRepository ?? (_functionRepository = new FunctionRepository(_context)); }
        }
        public KndCmdMsrSndHstRepository KndCmdMsrSndHstRepository
        {
            get { return _kndCmdMsrSndHstRepository ?? (_kndCmdMsrSndHstRepository = new KndCmdMsrSndHstRepository(_context)); }
        }
        public KneadingCommandRepository KneadingCommandRepository
        {
            get { return _kneadingCommandRepository ?? (_kneadingCommandRepository = new KneadingCommandRepository(_context)); }
        }
        public KneadingRecordRepository KneadingRecordRepository
        {
            get { return _kneadingRecordRepository ?? (_kneadingRecordRepository = new KneadingRecordRepository(_context)); }
        }
        public MaterialShelfRepository MaterialShelfRepository
        {
            get { return _materialShelfRepository ?? (_materialShelfRepository = new MaterialShelfRepository(_context)); }
        }
        public MaterialShelfStatusRepository MaterialShelfStatusRepository
        {
            get { return _materialShelfStatusRepository ?? (_materialShelfStatusRepository = new MaterialShelfStatusRepository(_context)); }
        }
        public MaterialStorageRetrieveHistoryRepository MaterialStorageRetrieveHistoryRepository
        {
            get { return _materialStorageRetrieveHistoryRepository ?? (_materialStorageRetrieveHistoryRepository = new MaterialStorageRetrieveHistoryRepository(_context)); }
        }
        public MaterialTotalRepository MaterialTotalRepository
        {
            get { return _materialTotalRepository ?? (_materialTotalRepository = new MaterialTotalRepository(_context)); }
        }
        public MaterialWarehouseCommandRepository MaterialWarehouseCommandRepository
        {
            get { return _materialWarehouseCommandRepository ?? (_materialWarehouseCommandRepository = new MaterialWarehouseCommandRepository(_context)); }
        }
        public MaterialWarehouseCommandHistoryRepository MaterialWarehouseCommandHistoryRepository
        {
            get { return _materialWarehouseCommandHistoryRepository ?? (_materialWarehouseCommandHistoryRepository = new MaterialWarehouseCommandHistoryRepository(_context)); }
        }
        public MtrMsrSndCmdHstRepository MtrMsrSndCmdHstRepository
        {
            get { return _mtrMsrSndCmdHstRepository ?? (_mtrMsrSndCmdHstRepository = new MtrMsrSndCmdHstRepository(_context)); }
        }
        public MtrMsrSndCmdRepository MtrMsrSndCmdRepository
        {
            get { return _mtrMsrSndCmdRepository ?? (_mtrMsrSndCmdRepository = new MtrMsrSndCmdRepository(_context)); }
        }

        public MtrRtrMsrSndCmdHstRepository MtrRtrMsrSndCmdHstRepository
        {
            get { return _mtrRtrMsrSndCmdHstRepository ?? (_mtrRtrMsrSndCmdHstRepository = new MtrRtrMsrSndCmdHstRepository(_context)); }
        }
        public MtrRtrMsrSndCmdRepository MtrRtrMsrSndCmdRepository
        {
            get { return _mtrRtrMsrSndCmdRepository ?? (_mtrRtrMsrSndCmdRepository = new MtrRtrMsrSndCmdRepository(_context)); }
        }
        public NoManageRepository NoManageRepository
        {
            get { return _noManageRepository ?? (_noManageRepository = new NoManageRepository(_context)); }
        }
        public OutOfPlanProductRepository OutOfPlanProductRepository
        {
            get { return _outOfPlanProductRepository ?? (_outOfPlanProductRepository = new OutOfPlanProductRepository(_context)); }
        }
        public PasswordRepository PasswordRepository
        {
            get { return _passwordRepository ?? (_passwordRepository = new PasswordRepository(_context)); }
        }
        public PreProductShelfStatusRepository PreProductShelfStatusRepository
        {
            get { return _preProductShelfStatusRepository ?? (_preProductShelfStatusRepository = new PreProductShelfStatusRepository(_context)); }
        }
        public PreProductShelfStockRepository PreProductShelfStockRepository
        {
            get { return _preProductShelfStockRepository ?? (_preProductShelfStockRepository = new PreProductShelfStockRepository(_context)); }
        }
        public PreProductStorageRetrieveHistoryRepository PreProductStorageRetrieveHistoryRepository
        {
            get { return _preProductStorageRetrieveHistoryRepository ?? (_preProductStorageRetrieveHistoryRepository = new PreProductStorageRetrieveHistoryRepository(_context)); }
        }
        public PreProductTotalRepository PreProductTotalRepository
        {
            get { return _preProductTotalRepository ?? (_preProductTotalRepository = new PreProductTotalRepository(_context)); }
        }
        public PreProductWarehouseCommandHistoryRepository PreProductWarehouseCommandHistoryRepository
        {
            get { return _preProductWarehouseCommandHistoryRepository ?? (_preProductWarehouseCommandHistoryRepository = new PreProductWarehouseCommandHistoryRepository(_context)); }
        }
        public ProductShelfRepository ProductShelfRepository
        {
            get { return _productShelfRepository ?? (_productShelfRepository = new ProductShelfRepository(_context)); }
        }
        public ProductShelfStatusRepository ProductShelfStatusRepository
        {
            get { return _productShelfStatusRepository ?? (_productShelfStatusRepository = new ProductShelfStatusRepository(_context)); }
        }
        public ProductShelfStockRepository ProductShelfStockRepository
        {
            get { return _productShelfStockRepository ?? (_productShelfStockRepository = new ProductShelfStockRepository(_context)); }
        }
        public ProductStorageRetrieveHistoryRepository ProductStorageRetrieveHistoryRepository
        {
            get { return _productStorageRetrieveHistoryRepository ?? (_productStorageRetrieveHistoryRepository = new ProductStorageRetrieveHistoryRepository(_context)); }
        }
        public ProductTotalRepository ProductTotalRepository
        {
            get { return _productTotalRepository ?? (_productTotalRepository = new ProductTotalRepository(_context)); }
        }
        public ProductWarehouseCommandHistoryRepository ProductWarehouseCommandHistoryRepository
        {
            get { return _productWarehouseCommandHistoryRepository ?? (_productWarehouseCommandHistoryRepository = new ProductWarehouseCommandHistoryRepository(_context)); }
        }
        public ReceptionRepository ReceptionRepository
        {
            get { return _receptionRepository ?? (_receptionRepository = new ReceptionRepository(_context)); }
        }
        public SupMaterialStockRepository SupMaterialStockRepository
        {
            get { return _supMaterialStockRepository ?? (_supMaterialStockRepository = new SupMaterialStockRepository(_context)); }
        }
        public TabletCommandRepository TabletCommandRepository
        {
            get { return _tabletCommandRepository ?? (_tabletCommandRepository = new TabletCommandRepository(_context)); }
        }
        public TabletProductRepository TabletProductRepository
        {
            get { return _tabletProductRepository ?? (_tabletProductRepository = new TabletProductRepository(_context)); }
        }
        public TerminalRepository TerminalRepository
        {
            get { return _terminalRepository ?? (_terminalRepository = new TerminalRepository(_context)); }
        }
        public TermStatusRepository TermStatusRepository
        {
            get { return _termStatusRepository ?? (_termStatusRepository = new TermStatusRepository(_context)); }
        }
        public TrmPicMgnRepository TrmPicMgnRepository
        {
            get { return _trmPicMgnRepository ?? (_trmPicMgnRepository = new TrmPicMgnRepository(_context)); }
        }

        public KndCmdMsrSndRepository KndCmdMsrSndRepository
        {
            get { return _kndCmdMsrSndRepository ?? (_kndCmdMsrSndRepository = new KndCmdMsrSndRepository(_context)); }
        }
        public OutSidePrePdtStkRepository OutSidePrePdtStkRepository
        {
            get { return _outSidePrePdtStkRepository ?? (_outSidePrePdtStkRepository = new OutSidePrePdtStkRepository(_context)); }
        }

        public PdtShipHstRepository PdtShipHstRepository
        {
            get { return _pdtShipHstRepository ?? (_pdtShipHstRepository = new PdtShipHstRepository(_context)); }
        }

        public ShipCommandRepository ShipCommandRepository
        {
            get { return _shipCommandRepository ?? (_shipCommandRepository = new ShipCommandRepository(_context)); }
        }

        public ShippingPlanRepository ShippingPlanRepository
        {
            get { return _shippingPlanRepository ?? (_shippingPlanRepository = new ShippingPlanRepository(_context)); }
        }
        public PreProductWarehouseCommandRepository PreProductWarehouseCommandRepository
        {
            get { return _preProductWarehouseCommandRepository ?? (_preProductWarehouseCommandRepository = new PreProductWarehouseCommandRepository(_context)); }
        }
        public ProductWarehouseCommandRepository ProductWarehouseCommandRepository
        {
            get { return _productWarehouseCommandRepository ?? (_productWarehouseCommandRepository = new ProductWarehouseCommandRepository(_context)); }
        }

        /// <summary>
        /// Provides functions to access realtime connection repository.
        /// </summary>
        public RealtimeRepository RealtimeConnectionRepository
        {
            get { return _realtimeRepository ?? (_realtimeRepository = new RealtimeRepository(_context)); }
        }

        /// <summary>
        /// Accessor of KCSG DbContext.
        /// </summary>
        public IKCSGDbContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Save change into database synchronously.
        /// </summary>
        public void Commit()
        {
            
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var message = new StringBuilder();
                
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }

                var exception = new Exception(message.ToString());
                throw exception;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// Save all works asynchronously into database.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
