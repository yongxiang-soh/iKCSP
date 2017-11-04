using AutoMapper;
using KCSG.Web.AutoMapperProfiles;

namespace KCSG.Products.AppStart
{
    public class AutoMapperConfig
    {
        public static void RegisterProfiles()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DataToDomainMappingProfile>();
                x.AddProfile<DomainToViewModelMappingProfile>();
                x.AddProfile<DomainToDataMappingProfile>();
                x.AddProfile<ViewModelToDomainMappingProfile>();
            });
        }
    }
}