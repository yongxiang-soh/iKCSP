using System;
using System.Collections.Generic;
using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using System.Linq;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Web.AutoMapperProfiles
{
    public class DataToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<TM01_Material, MaterialItem>();
            CreateMap<TM02_PrePdtMkp, PrePdtMkpItem>();
            CreateMap<TM03_PreProduct, PreProductItem>();
            CreateMap<TM03_PreProduct, PrintPreProductItem>()
                .ForMember(i => i.MixDate1,o =>o.MapFrom(m =>m.F03_MixDate1.HasValue?m.F03_MixDate1.Value.ToString("HH:mm:ss"):""))
                .ForMember(i => i.MixDate2, o => o.MapFrom(m => m.F03_MixDate2.HasValue ? m.F03_MixDate2.Value.ToString("HH:mm:ss") : ""))
                .ForMember(i => i.MixDate3, o => o.MapFrom(m => m.F03_MixDate3.HasValue ? m.F03_MixDate3.Value.ToString("HH:mm:ss") : ""));
            CreateMap<TM11_PckMtr, PckMtrItem>();
            CreateMap<IQueryable<TM11_PckMtr>, IQueryable<PckMtrItem>>();
            CreateMap<TM05_Conveyor,ConveyorItem>();
            CreateMap<TM09_Product, ProductItem>();
            CreateMap<TM15_SubMaterial, SubMaterialItem>();
            CreateMap<TM15_SubMaterial, StorageOfSupplementaryMaterialItem>();
            CreateMap<TX30_Reception, MaterialReceptionItem>();
            CreateMap<TX31_MtrShfSts, MaterialShelfStatusItem>();
            CreateMap<TX33_MtrShfStk, MaterialShelfStockItem>();
            CreateMap<TX34_MtrWhsCmd, MaterialWarehouseCommandItem>();
            CreateMap<TX39_PdtPln, PdtPlnItem>();
            CreateMap<TX48_NoManage, NoManageItem>();
            CreateMap<TX94_Prepdtplan, PreProductPlanItem>();
            CreateMap<TX31_MtrShfSts, PalletGridDetail>();
            CreateMap<TX56_TbtPdt, StorageOfProductItem>();
            CreateMap<TH67_CrfHst, ProductCertificationItem>();
            CreateMap<TX51_PdtShfSts, TempTableItem>();
            CreateMap<TX58_OutPlanPdt, ProductCertificationOutOfPlanItem>();
            CreateMap<TX44_ShippingPlan, ProductShippingPlanningItem>();
            CreateMap<TX47_PdtWhsCmd, ThirdCommunicationResponse>();
            CreateMap<TX40_PdtShfStk, RestorageOfProductViewItem>();
            CreateMap<TX34_MtrWhsCmd, FirstCommunicationResponse>();
            CreateMap<TX50_PrePdtWhsCmd, SecondCommunicationResponse>();
            CreateMap<Te80_Env_Mesp, CalculationOfControlLimitItem>();
        }
    }
}