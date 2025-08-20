using AutoMapper;
using ReKlik.DTO.ProductDTO;
using ReKlik.DTO.UsersDTO;
using ReKlik.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo para Productos
            CreateMap<Product, ProductDTO>()
                .ReverseMap(); // Permite mapeo bidireccional

            CreateMap<Product, ProductCreateDTO>()
                .ReverseMap();

            CreateMap<User, UserDTO>()
                .ReverseMap();

            CreateMap<User, UserUpdateDTO>()
                .ReverseMap();

            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.CreatedAt,
                      opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.MinValue))
            .ReverseMap();
            // Agregar otros mapeos aquí
        }
    }
}
