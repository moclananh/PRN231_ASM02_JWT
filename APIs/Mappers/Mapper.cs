using APIs.Models.Entities;
using APIs.ViewModels;
using AutoMapper;

namespace APIs.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<LoginVm, User>().ReverseMap();

            CreateMap<ProductVm, Product>().ReverseMap();
        }
    }
}
