﻿using Core.Sdk.Dtos;
using DndSolution.Application.Models;

namespace Core.Api.Mappings;

public static class UserMapper
{
    public static User MapToModel(UserDto dto)
    {
        return new User
        {
            Email = dto.Email,
            Id = dto.UserId,
            PasswordHash = dto.PasswordHash,
            UserName = dto.UserName,
        };
    } 
}