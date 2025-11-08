using AutoMapper;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {

            CreateMap<CreateTeamDTO, Team>();
            CreateMap<UpdateTeamDTO, Team>();
            CreateMap<Team, TeamDTO>();
            CreateMap<TeamMember, TeamMemberDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ProfileImg, opt => opt.MapFrom(src => src.User.ProfileImg));
            CreateMap<CreateTaskHeadDTO, TaskHead>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>Domain.Enum.TaskStatus.Pending));

            CreateMap<UpdateTaskHeadDTO, TaskHead>();
            CreateMap<TaskHead, TaskHeadDTO>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.TeamName))
                .ForMember(dest => dest.TaskDefinitions, opt => opt.MapFrom(src => src.TaskDefinitions));
        
        }
    }
}
