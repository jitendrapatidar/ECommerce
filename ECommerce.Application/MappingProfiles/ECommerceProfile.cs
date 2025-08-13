using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.MappingProfiles;

public class ECommerceProfile : Profile
{
    public ECommerceProfile()
    {
        // Category
        CreateMap<Category, CategoryDto>()
            .ReverseMap();

        // Product
        CreateMap<Product, ProductDto>()
            .ReverseMap();

        // Order
        CreateMap<Order, OrderDto>()
            .ReverseMap();

        // OrderItem
        CreateMap<OrderItem, OrderItemDto>()
            .ReverseMap();

        // User
        CreateMap<User, UserDto>()
            .ReverseMap();

        CreateMap<User, registerDto>()
         .ReverseMap();

        // Role
        CreateMap<Role, RoleDto>()
            .ReverseMap();
    }
}