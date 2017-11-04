using System;
using System.Globalization;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.Inquiry.ByWareHouseLocation;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Material;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PckMtr;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Product;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProductPlan;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.SubMaterialMasters;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.Web.Areas.Common.ViewModels;
using KCSG.Web.Areas.Communication.ViewModels;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ControlLimitEdit;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ProductMasterManagement;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialPostReceptionInput;
using KCSG.Web.Areas.MaterialManagement.ViewModels.RetrievalOfWarehousePallet;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfMaterial;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplementaryMaterial;
using KCSG.Web.Areas.ProductManagement.ViewModels;
using KCSG.Web.Areas.ProductManagement.ViewModels.OutOfPlanProduct;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingPlanning;
using KCSG.Web.Areas.ProductManagement.ViewModels.ReStorageOfProduct;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models.PreProductCharging;
using KCSG.Web.ViewModels;

namespace KCSG.Web.AutoMapperProfiles
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<MaterialViewModel, MaterialItem>().ForMember(
                i => i.F01_Unit, o => o.MapFrom(m => m.F01_Unit == "K" ? "0" : "1"));
            CreateMap<ProductViewModel, ProductItem>();
            CreateMap<PrePdtMkpViewModel, PrePdtMkpItem>();
            CreateMap<PreProductViewModel, PreProductItem>()
                .ForMember(i => i.F03_MixDate1, o => o.MapFrom(m => new DateTime(1980, 1, 1,m.MixDate1.HasValue? m.MixDate1.Value.Hours:0,m.MixDate1.HasValue? m.MixDate1.Value.Minutes:0,m.MixDate1.HasValue? m.MixDate1.Value.Milliseconds:0)))
                .ForMember(i => i.F03_MixDate2, o => o.MapFrom(m =>new DateTime(1980,1,1,m.MixDate2.HasValue?m.MixDate2.Value.Hours:0,m.MixDate2.HasValue?m.MixDate2.Value.Minutes:0,m.MixDate2.HasValue?m.MixDate2.Value.Milliseconds:0)))
                .ForMember(i => i.F03_MixDate3,o=> o.MapFrom(m => new DateTime(1980, 1, 1,m.MixDate3.HasValue? m.MixDate3.Value.Hours:0,m.MixDate3.HasValue? m.MixDate3.Value.Minutes:0, m.MixDate3.HasValue?m.MixDate3.Value.Milliseconds:0)));
            CreateMap<PdtPlnViewModel, PdtPlnItem>().ForMember(i => i.F39_KndEptBgnDate, o => o.MapFrom(m =>ConvertHelper.ConvertToDateTimeFull( m.F39_KndEptBgnDate)));
            CreateMap<PckMtrViewModel, PckMtrItem>();
            CreateMap<PreProductPlanViewModel, PreProductPlanItem>()    ;
            CreateMap<Pagination, GridSettings>();

            CreateMap<SubMaterialViewModel, SubMaterialItem>()
                .ForMember(i=>i.F15_SubMaterialCode,o=>o.MapFrom(m=>m.SubMaterialCode))
                .ForMember(i => i.F15_MaterialDsp,o => o.MapFrom(m => m.SubMaterialDsp))
                .ForMember(i => i.F15_SupplierCode,o => o.MapFrom(m => m.SupplierCode))
                .ForMember(i => i.F15_Price, o => o.MapFrom(m => m.Price))
                .ForMember(i => i.F15_State, o => o.MapFrom(m => m.State)).
                ForMember(i => i.F15_ModifyClass, o => o.MapFrom(m => m.ModifyClass)).
                ForMember(i => i.F15_Department, o => o.MapFrom(m => m.Department))
                .ForMember(i => i.F15_PackingUnit, o => o.MapFrom(m => m.PackingUnit))
                .ForMember(i => i.F15_Unit, o => o.MapFrom(m => m.Unit))
                .ForMember(i => i.F15_EMP, o => o.MapFrom(m => m.EMP)).
                ForMember(i => i.F15_Point, o => o.MapFrom(m => m.FactoryClass)).
                ForMember(i => i.F15_EntrustedClass, o => o.MapFrom(m => m.Baliment))
                ;

            CreateMap<MaterialReceptionViewModel, MaterialReceptionItem>();
            CreateMap<MaterialPostReceptionInputViewModel, MaterialPostReceptionInputItem>();
            CreateMap<StorageOfSupplementaryMaterialViewModel, StorageOfSupplementaryMaterialItem>();
            CreateMap<RetrievalOfWarehousePalletViewModel, RetrievalOfWarehousePalletItem>();
            CreateMap<FinalStockTakingMaterialItem,StockTakingOfMaterialC1ViewModelItem>();
            CreateMap<StorageOfMaterialViewModel, StorageOfMaterialItem>();
            //CreateMap<ProductCertificationViewModel, ProductCertificationItem>().ForMember(i => i.CertificationDate, o => o.MapFrom(m => ConvertHelper.ConvertToDateTimeFull(m.CertificationDate)));
            CreateMap<ProductCertificationViewModel, ProductCertificationItem>().ForMember(i => i.CertificationDate, o => o.MapFrom(m => m.CertificationDate));
            CreateMap<ProductShippingPlanningViewModel, ProductShippingPlanningItem>().ForMember(i => i.DeliveryDate, o => o.MapFrom(m => m.DeliveryDate));


            CreateMap<OutOfPlanProductViewModel, OutOfPlanProductItem>()
                .ForMember(i => i.F58_TbtCmdEndFrtAmt, o => o.MapFrom(m => m.Fraction))
                .ForMember(i => i.F58_TbtCmdEndPackAmt, o => o.MapFrom(m => m.PackQuantity))
                .ForMember(i => i.F58_PrePdtLotNo, o => o.MapFrom(m => m.PrePdtLotNo))
                .ForMember(i => i.F58_ProductCode, o => o.MapFrom(m => m.ProductCode))
                .ForMember(i => i.F58_ProductLotNo, o => o.MapFrom(m => m.ProductLotNo))
                .ForMember(i => i.ProductName, o => o.MapFrom(m => m.ProductName))
                .ForMember(i => i.F58_TbtEndDateString, o => o.MapFrom(m => m.F58_TbtEndDateString));
            CreateMap<ReStorageOfProductViewModel, RestoreProductItem>();
            CreateMap<StorageOfProductSelectedViewModel, StoreProductItem>();
            CreateMap<InquiryRawMaterialShelfStatusModelView, RawMaterialShelftStatusItem>().
                ForMember(i => i.StorageDate,
                    o => o.MapFrom(m => DateTime.ParseExact(m.StorageDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
            CreateMap<InquiryByPreProductShelfStatusModelView, PreProductShelfStatusItem>()
                .ForMember(i => i.ContainerNo, o=>o.MapFrom(m=>m.ContainerNo))
                .ForMember(i => i.StorageDate,
                    o => o.MapFrom(m => DateTime.ParseExact(m.StorageDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));
            CreateMap<InquiryBySupplierPalletModelView, InquiryBySupplierPalletItem>();
            CreateMap<InquiryByPreProductShelfStatusEmptyModelView, InquiryByPreProductShelfStatusEmptyItem>()
                .ForMember(i => i.ContainerNo, o => o.MapFrom(m => m.ContainerNo));

            CreateMap<InquiryByProductShelfStatusModelView, InquiryByProductShelfStatusItem>()
                .ForMember(i => i.StorageDate,
                    o => o.MapFrom(m => DateTime.ParseExact(m.StorageDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)))
                .ForMember(i => i.Row,o => o.MapFrom(m => m.Row))
                .ForMember(i => i.Bay,o => o.MapFrom(m => m.Bay))
                .ForMember(i => i.Level,o => o.MapFrom(m => m.Level))
                .ForMember(i => i.ProductClassification, o => o.MapFrom(m => ((int)m.ProductClassification).ToString()));

            CreateMap<InquiryByProductShelfStatusExternalPreProductModelView, InquiryByProductShelfStatusExternalPreProductItem>().
                ForMember(i => i.StorageDate,
                    o => o.MapFrom(m => DateTime.ParseExact(m.StorageDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)));

            CreateMap<InquiryByProductShelfStatusEmptyModelView, InquiryByProductShelfStatusEmptyItem>();
            CreateMap<WeighingEquipmentViewModel, WeighingEquipmentViewSearchModel>().ForMember(i=>i.LN ,o=>o.MapFrom(m=>m.Line));
            CreateMap<EndTabletisingViewModel, EndTabletisingItem>();
            CreateMap<PreProductChargingViewModel,PreProductChargingItem>();
            CreateMap<ControlLimitEditViewModel, ControlLimitEditItem>();
            CreateMap<ProductMasterManagementViewModel,ProductMasterManagementItem>();
        }
    }
}