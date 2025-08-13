using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Business.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enum;
using ECommerce.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Business.Service;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IUserInRoleRepository  _userInRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
   

    public AuthService(IAuthRepository authRepository,
                     IConfiguration configuration,
                     IUserRepository userRepository,
                     IUserInRoleRepository userInRoleRepository, 
                     IMapper mapper)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _mapper = mapper;
        _userRepository = userRepository;
        _userInRoleRepository = userInRoleRepository;
    }
    public async Task<UserDto> RegisterAsync(registerDto registerDto)
    {
        if (await _authRepository.UserExistsAsync(registerDto.Email))
            throw new ApplicationException("Email already exists");
               
        var user = _mapper.Map<User>(registerDto);
        user.CreatedAt = DateTime.UtcNow;
        //Hash password
        user.PasswordHash   = ComputeSha256Hash(registerDto.PasswordHash);
         var registeredUser = await _authRepository.RegisterAsync(user);
        UserInRole userInRole = new UserInRole
        {
            UserId = registeredUser.UserId,
            RoleId = 2 // Assuming RoleId is part of registerDto
        };
        await _userInRoleRepository.AddAsync(userInRole); 
        return _mapper.Map<UserDto>(registeredUser);
    }
    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
        string passwordHash = ComputeSha256Hash(loginDto.PasswordHash);
        if (user == null || (passwordHash !=user.PasswordHash))
            throw new ApplicationException("Invalid email or password");

        return await GenerateJwtToken(user);
    }
    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        return _mapper.Map<UserDto>(user);
    }
    public async Task ChangePasswordAsync(int userId, UserDto changePasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");
        string passwordHash = ComputeSha256Hash(changePasswordDto.PasswordHash);
        if ((passwordHash == user.PasswordHash))
            throw new ApplicationException("Current password is incorrect");

        user.PasswordHash = ComputeSha256Hash(changePasswordDto.PasswordHash);
        await _userRepository.UpdateAsync(user);
    }
    private async Task<string> GenerateJwtToken(User user)
    {

        var userInRole = await _userInRoleRepository.GetByIdAsync(user.UserId);
        if (userInRole == null)
            throw new ApplicationException("User role not found");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        string roleName = string.Empty;
        if (userInRole.RoleId == 1) roleName = EnumRoles.Admin.ToString(); //Admin
        if (userInRole.RoleId == 2) roleName = EnumRoles.User.ToString();//User
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // hex format
            }
            return builder.ToString();
        }
    }
}
