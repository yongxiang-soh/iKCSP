using System.Collections;
using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using System.Collections.Generic;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Web.AutoMapperProfiles
{
    public class DomainToDataMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<MaterialItem, TM01_Material>();
            CreateMap<IEnumerable<MaterialItem>, IEnumerable<TM01_Material>>();
            CreateMap<IEnumerable<ConveyorItem>,IEnumerable<TM05_Conveyor>>();
            CreateMap<IEnumerable<ProductItem>, IEnumerable<TM09_Product>>();
            CreateMap<IEnumerable<PreProductItem>, IEnumerable<TM03_PreProduct>>();
            CreateMap<PrePdtMkpItem, TM02_PrePdtMkp>();
            CreateMap<PreProductItem, TM03_PreProduct>();
            CreateMap<ProductItem, TM09_Product>();
            CreateMap<PckMtrItem, TM11_PckMtr>();
            CreateMap<StorageOfSupplementaryMaterialItem, TM15_SubMaterial>();
            CreateMap<SubMaterialItem, TM15_SubMaterial>();
            CreateMap<MaterialReceptionItem, TX30_Reception>();
            CreateMap<MaterialWarehouseCommandItem, TX34_MtrWhsCmd>();
            CreateMap<PdtPlnItem, TX39_PdtPln>();
            CreateMap<NoManageItem, TX48_NoManage>();
            CreateMap<PreProductPlanItem, TX94_Prepdtplan>();
            CreateMap<PalletGridDetail, TX31_MtrShfSts>();
            CreateMap<StorageOfProductItem, TX56_TbtPdt>();
            CreateMap<ProductCertificationItem, TH67_CrfHst>();
            CreateMap<TempTableItem, TX51_PdtShfSts>();
            CreateMap<ProductCertificationOutOfPlanItem, TX58_OutPlanPdt>();
            CreateMap<ProductShippingPlanningItem, TX44_ShippingPlan>();
            CreateMap<RestorageOfProductViewItem, TX40_PdtShfStk>();
            CreateMap<SecondCommunicationResponse, TX50_PrePdtWhsCmd>();
            CreateMap<CalculationOfControlLimitItem,Te80_Env_Mesp>();
        }
    }
}