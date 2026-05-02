using GameBackend.Core.Entities;

namespace GameBackend.Application.Contracts.Admin;

public class UpdatePlayerRoleRequest
{
    public PlayerRole Role { get; set; }
}