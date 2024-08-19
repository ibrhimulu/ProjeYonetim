using AutoMapper;
using herkesuyurkenkodlama.Models;


namespace ProjeYonetim
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<Project, ProjectViewModel>().ReverseMap();
        }
    }

}
