using System;
using System.Collections.Generic;
using System.Text;
using Profile = AutoMapper.Profile;

namespace NCovid.Service.AutoMapper
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
