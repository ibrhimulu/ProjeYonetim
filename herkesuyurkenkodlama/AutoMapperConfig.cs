using AutoMapper;
using herkesuyurkenkodlama.Models;


namespace ProjeYonetim
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, CreateUserModel>().ReverseMap();
            CreateMap<User, EditUserModel>().ReverseMap();

            CreateMap<Project, ProjectViewModel>().ReverseMap();
        }
    }

}
