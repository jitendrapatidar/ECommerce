using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Business.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;   
        _mapper = mapper;
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(userDto.UserId);
        if (existingUser == null) throw new KeyNotFoundException("User not found");

        // Don't allow email change through this method
        _mapper.Map(userDto, existingUser);
        await _userRepository.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");

        await _userRepository.DeleteAsync(user);
    }

    public async Task UpdateUserRoleAsync(int userId, string newRole)
    {
        var userInRole = await _roleRepository.GetByRoleNameAsync(newRole);
        if (userInRole==null)
            throw new ArgumentException("Invalid role specified");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");
        await _userRepository.UpdateAsync(user);
    }

}