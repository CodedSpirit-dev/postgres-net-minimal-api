using postgres_net_minimal_api.Authorization.Enums;

namespace postgres_net_minimal_api.Authorization.DTOs;

// Request DTOs
public record AssignPermissionRequest(int FeatureActionId);
public record RemovePermissionRequest(int FeatureActionId);

// Response DTOs
public record ModuleDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    int DisplayOrder);

public record FeatureDto(
    int Id,
    string Name,
    string ResourceKey,
    string? Description,
    int DisplayOrder);

public record ActionDto(
    int Id,
    string Name,
    string ActionKey,
    string? Description,
    int DisplayOrder);

public record FeatureActionDto(
    int Id,
    int FeatureId,
    string FeatureName,
    string ResourceKey,
    int ActionId,
    string ActionName,
    string ActionKey,
    bool IsEnabled);

public record RolePermissionDto(
    int Id,
    Guid RoleId,
    string RoleName,
    int FeatureActionId,
    string FeatureName,
    string ResourceKey,
    string ActionName,
    string ActionKey,
    DateTime CreatedAt);

public record UserPermissionsDto(
    Guid UserId,
    string UserName,
    Guid RoleId,
    string RoleName,
    HashSet<string> Permissions);
