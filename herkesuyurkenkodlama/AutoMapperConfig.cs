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
            CreateMap<Project, CreateProjectModel>().ReverseMap();
            CreateMap<Project, EditProjectModel>().ReverseMap();

            CreateMap<Tasklar, TasklarViewModel>().ReverseMap();
            CreateMap<Tasklar, CreateTasklarModel>().ReverseMap();
            CreateMap<Tasklar, EditTasklarModel>().ReverseMap();
            
            CreateMap<Sdepartment, TeamViewModel>().ReverseMap();
            CreateMap<Sdepartment, CreateTeamModel>().ReverseMap();
            CreateMap<Sdepartment, EditTeamModel>().ReverseMap();

            CreateMap<Mdepartment, HeadshipViewModel>().ReverseMap();
            CreateMap<Mdepartment, CreateHeadshipModel>().ReverseMap();
            CreateMap<Mdepartment, EditHeadshipModel>().ReverseMap();


        }
    }

}
