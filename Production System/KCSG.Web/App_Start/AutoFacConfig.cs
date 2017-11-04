using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using KCSG.Core.Enumerations;
using KCSG.Core.Module;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains;
using KCSG.Domain.Domains.CommunicationManagement;
using KCSG.Domain.Domains.EnvironmentManagement;
using KCSG.Domain.Domains.Inquiry;
using KCSG.Domain.Domains.KneadingCommand;
using KCSG.Domain.Domains.MaterialManagement;
using KCSG.Domain.Domains.PreProductManagement;
using KCSG.Domain.Domains.ProductionPlanning;
using KCSG.Domain.Domains.ProductManagement;
using KCSG.Domain.Domains.SystemManagement;
using KCSG.Domain.Domains.TabletisingCommandSubSystem;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.CommunicationManagement;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.ProductCertificationManagement;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.ProductShippingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Web.Attributes;
using KCSG.Web.Services;
using log4net.Config;
using Microsoft.AspNet.SignalR;
using RegistrationExtensions = Autofac.Integration.SignalR.RegistrationExtensions;
using KCSG.Domain.Interfaces.EnvironmentManagement;

namespace KCSG.Products.AppStart
{
    public class AutoFacConfig
    {
        public static void RegisterContainers()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            RegistrationExtensions.RegisterHubs(builder);

            #region Modules

            // Log4net module registration (this is for logging)
            XmlConfigurator.Configure();
            builder.RegisterModule<Log4NetModule>();

            #endregion

            #region Register Context & UnitOfWork

            builder.RegisterType<KCSGDbContext>().As<IKCSGDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            
            var configurationService = new ConfigurationService();
            builder.RegisterType<ConfigurationService>()
                .As<IConfigurationService>()
                .OnActivating(x => x.ReplaceInstance(configurationService))
                .SingleInstance();

            #endregion

            #region Register Domains

            #region Common Search

            builder.RegisterType<BaseDomain>().As<ICommonDomain>().InstancePerLifetimeScope();
            builder.RegisterType<CommonSearchDomain>().As<ICommonSearchDomain>().InstancePerLifetimeScope();

            #endregion

            builder.RegisterType<RetrievalOfOutOfSpecPreProductDomain>()
                .As<IRetrievalOfOutOfSpecPreProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<StorageOfMaterialDomain>().As<IStorageOfMaterialDomain>().InstancePerLifetimeScope();
            builder.RegisterType<StockTakingOfMaterialDomain>()
                .As<IStockTakingOfMaterialDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<MaterialSimulationDomain>().As<IMaterialSimulationDomain>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierDomain>().As<ISupplierDomain>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialDomain>().As<IMaterialDomain>().InstancePerLifetimeScope();
            builder.RegisterType<PreProductDomain>().As<IPreProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<PrePdtMkpDomain>().As<IPrePdtMkpDomain>().InstancePerLifetimeScope();
            builder.RegisterType<PdtPlnDomain>().As<IPdtPlnDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ProductDomain>().As<IProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<PckMtrDomain>().As<IPckMtrDomain>().InstancePerLifetimeScope();
            builder.RegisterType<SubMaterialDomain>().As<ISubMaterialDomain>().InstancePerLifetimeScope();
            builder.RegisterType<PreProductPlanDomain>().As<IPreProductPlanDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ConveyorDomain>().As<IConveyorDomain>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialRequirementListDomain>()
                .As<IMaterialRequirementListDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<StorageOfWarehousePalletDomain>()
                .As<IStorageOfWarehousePalletDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RestorageOfMaterialDomain>()
                .As<IRestorageOfMaterialDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<Communication1Domain>()
                .As<ICommunication1Domain>().InstancePerLifetimeScope();
            builder.RegisterType<Communication2Domain>()
                .As<ICommunication2Domain>().InstancePerLifetimeScope();
            builder.RegisterType<Communication3Domain>()
                .As<ICommunication3Domain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PreProductManagementDomain>()
                .As<IPreProductManagementDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StockTakingPreProductDomain>()
                .As<IStockTakingPreProductDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductCertificationDomain>()
                .As<IProductCertificationDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MaterialShelfStatusDomain>()
                .As<IMaterialShelfStatusDomain>()
                .InstancePerLifetimeScope(); builder.RegisterType<WeighingEquipmentDomain>()
                .As<IWeighingEquipmentDomain>()
                .InstancePerLifetimeScope();

            #region Material

            builder.RegisterType<MaterialPostReceptionInputDomain>()
                .As<IMaterialPostReceptionInputDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<MaterialReceptionDomain>().As<IMaterialReceptionDomain>().InstancePerLifetimeScope();
            builder.RegisterType<AcceptanceOfMaterialDomain>()
                .As<IAcceptanceOfMaterialDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ForcedRetrievalOfRejectedMaterialDomain>()
                .As<IForcedRetrievalOfRejectedMaterialDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RetrieveOfMaterialDomain>().As<IRetrieveOfMaterialDomain>().InstancePerLifetimeScope();
            builder.RegisterType<StorageOfSupplementaryMaterialDomain>()
                .As<IStorageOfSupplementaryMaterialDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<InterFloorMovementOfMaterialDomain>()
                .As<IInterFloorMovementOfMaterialDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RetrieveSupplierPalletDomain>()
                .As<IRetrieveSupplierPalletDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RetrievalOfWarehousePalletDomain>()
                .As<IRetrievalOfWarehousePalletDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StorageOfSupplierPalletDomain>()
                .As<IStorageOfSupplierPalletDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region Material Reception Input

            builder.RegisterType<MaterialReceptionDomain>().As<IMaterialReceptionDomain>().InstancePerLifetimeScope();

            #endregion

            #region Kneading Command

            builder.RegisterType<InputOfKneadingCommandDomain>()
                .As<IInputOfKneadingCommandDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region Kneading command

            builder.RegisterType<KneadingStartEndControlDomain>()
                .As<IKneadingStartEndControlDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region Management System

            builder.RegisterType<StartEndSystemDomain>().As<IStartEndSystemDomain>().InstancePerLifetimeScope();
            builder.RegisterType<DailyProcessDomain>().As<IDailyProcessDomain>().InstancePerLifetimeScope();
            builder.RegisterType<MonthlyProcessDomain>()
                .As<IMonthlyProcessDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region Master database

            builder.RegisterType<DatabaseMaintainanceDomain>()
                .As<IDatabaseMaintainanceDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region Tabletising

            builder.RegisterType<TabletisingStartStopDomain>()
                .As<ITabletisingStartStopDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CreateTabletisingCommandDomain>()
                .As<ICreateTabletisingCommandDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ManagementOfProductLabelDomain>()
                .As<IManagementOfProductLabelDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #region PreProductManagement

            builder.RegisterType<PreProductManagementDomain>()
                .As<IPreProductManagementDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RetrievalOfPreProductDomain>()
                .As<IRetrievalOfPreProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ForcedRetrievalOfPreProductDomain>()
                .As<IForcedRetrievalOfPreProductDomain>()
                .InstancePerLifetimeScope();

            #endregion

            #endregion

            #region Independent domains

            builder.RegisterType<LoginDomain>().As<ILoginDomain>().SingleInstance();
            builder.RegisterType<IdentityService>().As<IIdentityService>().SingleInstance();
            builder.RegisterType<ExportReportDomain>().As<IExportReportDomain>().SingleInstance();

            #endregion

            #region Attributes

            builder.RegisterType<MvcAuthorizeAttribute>().PropertiesAutowired();
            //builder.Register(c => new MvcAuthorizeAttribute())
            //      .PropertiesAutowired()
            //      .AsAuthorizationFilterFor<Controller>().InstancePerHttpRequest();

            builder.RegisterType<SignalrCookieAuthenticateAttribute>().PropertiesAutowired();
            #endregion

            #region Services

            // Mvc authentication provider.

            // Notification service - broadcasting messages from domains to clients.
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();
            builder.RegisterType<LabelPrintService>().As<ILabelPrintService>().SingleInstance();

            #endregion

            #region Product management
            builder.RegisterType<FontStorageOfSupplierPalletDomain>().As<IFontStorageOfSupplierPalletDomain>().InstancePerLifetimeScope();
            builder.RegisterType<StorageOfProductDomain>().As<IStorageOfProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ForceRetrievalOfProductDomain>()
                .As<IForcedRetrievalOfProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ProductShippingCommandDomain>()
                .As<IProductShippingCommandDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<InterFloorMovementOfProductDomain>()
                .As<IInterFloorMovementOfProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<StockTakingOfProductDomain>()
                .As<IStockTakingOfProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RestorageOfProductDomain>().As<IRestorageOfProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<StorageOfEmptyProductPalletDomain>()
                .As<IStorageOfEmptyProductPalletDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RetrievalOfProductPalletDomain>()
                .As<IRetrievalOfProductPalletDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<StorageOfExternalPreProductDomain>()
                .As<IStorageOfExternalPreProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ProductShippingPlanningDomain>()
                .As<IProductShippingPlanningDomain>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RetrievalOfExternalPreProductDomain>()
                .As<IRetrievalOfExternalProductDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OutOfPlanProductDomain>().As<IOutOfPlanProductDomain>().InstancePerLifetimeScope();

            #endregion
            
            #region Inquiry

            builder.RegisterType<InquiryByMaterialNameDomain>().As<IInquiryByMaterialNameDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryByPreProductDomain>().As<IInquiryByPreProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryByPreProductNameDomain>().As<IInquiryByPreProductNameDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryByExternalPreProductNameDomain>().As<IInquiryByExternalPreProductNameDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryBySupplierNameDomain>().As<IInquiryBySupplierNameDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryByProductNameDomain>().As<IInquiryByProductNameDomain>().InstancePerLifetimeScope();

            //builder.RegisterType<InquiryByMaterialShelfStatusDomain>()
            //    .As<IInquiryByMaterialShelfStatus>()
            //    .InstancePerLifetimeScope(); 
            //builder.RegisterType<InquiryByWarehouseLocationDomain>()
            //    .As<IInquiryByWarehouseLocation>()
            //    .InstancePerLifetimeScope(); 
                builder.RegisterType<InquiryCommonDomain>()
                .As<IInquiryCommonDomain>()
                .InstancePerLifetimeScope();
            builder.RegisterType<InquiryByProductDomain>().As<IInquiryByProductDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryKneadingLineNoDomain>().As<IInquiryKneadingLineNoDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryKneadingCommandNoDomain>().As<IInquiryKneadingCommandNoDomain>().InstancePerLifetimeScope();
            builder.RegisterType<InquiryKneadingLotNoDomain>().As<IInquiryKneadingLotNoDomain>().InstancePerLifetimeScope();
            builder.RegisterType<MaterialStockDomain>().As<IMaterialStockDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ManagementReportDomain>().As<IManagementReportDomain>().InstancePerLifetimeScope();

            #endregion

            #region PreProduct Charging

            builder.RegisterType<PreProductChargingDomain>().As<IPreProductCharging>().InstancePerLifetimeScope();

            #endregion

            #region Environment
            builder.RegisterType<EnvironmentBaseDomain>().As<IEnvironmentBaseDomain>().InstancePerLifetimeScope();
            builder.RegisterType<CalculationOfControlLimitDomain>().As<ICalculationOfControlLimitDomain>().InstancePerLifetimeScope();
            builder.RegisterType<LotDataCleanupDomain>().As<ILotDataCleanupDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ProductMasterManagementDomain>().As<IProductMasterManagementDomain>().InstancePerLifetimeScope();
            builder.RegisterType<ControlLimitEditDomain>().As<IControlLimitEditDomain>().InstancePerLifetimeScope();
            builder.RegisterType<CleanlinessDataInputDomain>().As<ICleanlinessDataInputDomain>().InstancePerLifetimeScope();
            builder.RegisterType<LotDataSamplingDomain>().As<ILotDataSamplingDomain>().InstancePerLifetimeScope();
            builder.RegisterType<XRTemperatureManagementDomain>().As<IXRTemperatureManagement>().InstancePerLifetimeScope();
            builder.RegisterType<CreepingAndRollSpeedDurationDomain>().As<ICreepingAndRollSpeedDurationDomain>().InstancePerLifetimeScope();
            builder.RegisterType<CreepingAndRollSpeedDataEditDomain>().As<ICreepingAndRollSpeedDataEditDomain>().InstancePerLifetimeScope();
            builder.RegisterType<AvailabityDataEditDomain>().As<IAvailabityDataEditDomain>().InstancePerLifetimeScope();
            #endregion

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);
            
        }
        
        /// <summary>
        /// Read file.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private static string ReadFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            // Find absolute path.
            var absolutePath = HttpContext.Current.Server.MapPath(relativePath);

            if (string.IsNullOrEmpty(absolutePath) || !System.IO.File.Exists(absolutePath))
                return string.Empty;

            return System.IO.File.ReadAllText(absolutePath);
        }
    }
}