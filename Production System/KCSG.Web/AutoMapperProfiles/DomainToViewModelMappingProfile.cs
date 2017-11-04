using System;
using System.Collections.Generic;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.Inquiry.ByWareHouseLocation;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.Web.Areas.Common.ViewModels;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ControlLimitEdit;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ProductMasterManagement;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialPostReceptionInput;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Material;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PckMtr;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Product;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProductPlan;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.SubMaterialMasters;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfMaterial;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplementaryMaterial;
using KCSG.Web.Areas.ProductManagement.ViewModels;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingPlanning;
using KCSG.Web.Areas.ProductManagement.ViewModels.OutOfPlanProduct;
using KCSG.Web.Areas.ProductManagement.ViewModels.ReStorageOfProduct;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models.PreProductCharging;

namespace KCSG.Web.AutoMapperProfiles
{
    public class DomainToViewModelMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<MaterialItem, MaterialViewModel>().ForMember(
                i=>i.F01_Unit,o=>o.MapFrom(m=>m.F01_Unit=="0"?"K":"P"));
            CreateMap<ProductItem, ProductViewModel>();
            CreateMap<PrePdtMkpItem, PrePdtMkpViewModel>();
            CreateMap<PreProductItem, PreProductViewModel>()
                .ForMember(i => i.MixDate1, o => o.MapFrom(m => m.F03_MixDate1.Value.TimeOfDay))
                .ForMember(i => i.MixDate2, o => o.MapFrom(m => m.F03_MixDate2.Value.TimeOfDay))
                .ForMember(i => i.MixDate3, o => o.MapFrom(m => m.F03_MixDate3.Value.TimeOfDay))
                .ForMember(i => i.TmpRetTime, o => o.MapFrom(m => m.F03_TmpRetTime.Value.TimeOfDay));

            CreateMap<PdtPlnItem, PdtPlnViewModel>()
                .ForMember(i => i.F39_KndEptBgnDate, o => o.MapFrom(m => m.F39_KndEptBgnDate.ToString("dd/MM/yyyy")));
       
            CreateMap<PckMtrItem, PckMtrViewModel>();
            CreateMap<PreProductPlanItem, PreProductPlanViewModel>().ForMember(i => i.F94_YearMonth, o => o.MapFrom(m => m.F94_YearMonth.ToString("MM/yyyy")));
            CreateMap<TM11_PckMtr, PckMtrViewModel>();
            CreateMap<MaterialPostReceptionInputItem, MaterialPostReceptionInputViewModel>();
            CreateMap< SubMaterialItem,SubMaterialViewModel>();
            CreateMap<SubMaterialItem, SubMaterialViewModel>()
                .ForMember(i => i.SubMaterialCode, o => o.MapFrom(m => m.F15_SubMaterialCode))
                .ForMember(i => i.SubMaterialDsp, o => o.MapFrom(m => m.F15_MaterialDsp))
                .ForMember(i => i.SupplierCode, o => o.MapFrom(m => m.F15_SupplierCode))
                .ForMember(i => i.Price, o => o.MapFrom(m => m.F15_Price))
                .ForMember(i => i.State, o => o.MapFrom(m => m.F15_State)).
                ForMember(i => i.ModifyClass, o => o.MapFrom(m => m.F15_ModifyClass)).
                ForMember(i => i.Department, o => o.MapFrom(m => m.F15_Department))
                .ForMember(i => i.PackingUnit, o => o.MapFrom(m => m.F15_PackingUnit))
                .ForMember(i => i.Unit, o => o.MapFrom(m => m.F15_Unit))
                .ForMember(i => i.EMP, o => o.MapFrom(m => m.F15_EMP)).
                ForMember(i => i.FactoryClass, o => o.MapFrom(m => m.F15_Point)).
                ForMember(i => i.Baliment, o => o.MapFrom(m => m.F15_EntrustedClass))
                ;
            CreateMap<MaterialReceptionItem, MaterialReceptionViewModel>();
            CreateMap<MaterialPostReceptionInputItem, MaterialPostReceptionInputViewModel>();
            CreateMap<StorageOfSupplementaryMaterialItem, StorageOfSupplementaryMaterialViewModel>();
            CreateMap<StockTakingOfMaterialC1ViewModelItem, FinalStockTakingMaterialItem>();
            CreateMap<StorageOfMaterialItem, StorageOfMaterialViewModel>();
            CreateMap<ProductCertificationItem, ProductCertificationViewModel>();
            CreateMap<ProductShippingPlanningItem, ProductShippingPlanningViewModel>();
            CreateMap<RestoreProductItem, ReStorageOfProductViewModel>();


            CreateMap<OutOfPlanProductItem, OutOfPlanProductViewModel>()
                .ForMember(i => i.Fraction, o => o.MapFrom(m => m.F58_TbtCmdEndFrtAmt))
                .ForMember(i => i.PackQuantity, o => o.MapFrom(m => m.F58_TbtCmdEndPackAmt))
                .ForMember(i => i.PrePdtLotNo, o => o.MapFrom(m => m.F58_PrePdtLotNo.Trim()))
                .ForMember(i => i.ProductCode, o => o.MapFrom(m => m.F58_ProductCode.Trim()))
                .ForMember(i => i.ProductLotNo, o => o.MapFrom(m => m.F58_ProductLotNo.Trim()))
                .ForMember(i => i.ProductName, o => o.MapFrom(m => m.ProductName));
                //.ForMember(i => i.F58_TbtEndDateString, o => o.MapFrom(m => m.F58_TbtEndDate));
            CreateMap<StoreProductItem, StorageOfProductSelectedViewModel>();
            CreateMap<RawMaterialShelftStatusItem, InquiryRawMaterialShelfStatusModelView>()
                .ForMember(i => i.StorageDate, o => o.MapFrom(m => m.StorageDate.ToString("dd/MM/yyyy HH:mm:ss")));

            CreateMap<PreProductShelfStatusItem, InquiryByPreProductShelfStatusModelView>()
                .ForMember(i => i.ContainerNo, o => o.MapFrom(m => m.ContainerNo.ToString()))
                .ForMember(i => i.StorageDate, o => o.MapFrom(m => m.StorageDate.ToString("dd/MM/yyyy HH:mm:ss")));

            CreateMap<InquiryBySupplierPalletItem,InquiryBySupplierPalletModelView>();
            CreateMap<InquiryByPreProductShelfStatusEmptyItem, InquiryByPreProductShelfStatusEmptyModelView>()
                .ForMember(i => i.ContainerNo, o => o.MapFrom(m=>m.ContainerNo));

            CreateMap<InquiryByProductShelfStatusItem,InquiryByProductShelfStatusModelView>()
                .ForMember(i => i.StorageDate, o => o.MapFrom(m => m.StorageDate.ToString("dd/MM/yyyy HH:mm:ss")))
                .ForMember(i => i.Row, o => o.MapFrom(m => m.Row))
                .ForMember(i => i.Bay, o => o.MapFrom(m => m.Bay))
                .ForMember(i => i.Level, o => o.MapFrom(m => m.Level))
                .ForMember(i => i.ProductClassification, o => o.MapFrom(m => (Constants.ProductClassification)Enum.Parse(typeof(Constants.ProductClassification), m.ProductClassification)));

            CreateMap<InquiryByProductShelfStatusExternalPreProductItem,InquiryByProductShelfStatusExternalPreProductModelView>()
                .ForMember(i => i.StorageDate, o => o.MapFrom(m => m.StorageDate.ToString("dd/MM/yyyy HH:mm:ss")));
            CreateMap<InquiryByProductShelfStatusEmptyItem,InquiryByProductShelfStatusEmptyModelView>();
            CreateMap<EndTabletisingItem, EndTabletisingViewModel>();

            CreateMap<PreProductChargingItem, PreProductChargingViewModel>();
            CreateMap<ControlLimitEditItem, ControlLimitEditViewModel>();
            CreateMap<ProductMasterManagementItem, ProductMasterManagementViewModel>();
        }
    }
}