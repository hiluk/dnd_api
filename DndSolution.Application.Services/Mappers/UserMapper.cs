using Data.Entities;
using DndSolution.Application.Models;

namespace DndSolution.Application.Services.Mappers;

public static class UserMapper
{
    public static UserEntity MapToEntity(User model)
    {
        return new UserEntity
        {
            Email = model.Email,
            Password = model.Password
        };
    }
}