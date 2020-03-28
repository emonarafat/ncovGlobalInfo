namespace NCovid.Service.Mapper
{
    using AutoMapper;
    using DataContext;

    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<GlobalInfo, AllResults>().ReverseMap();
            CreateMap<Countries, CountryResult>().ReverseMap();
        }
    }
}