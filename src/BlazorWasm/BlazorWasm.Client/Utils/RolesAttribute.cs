using BlazorServer.Common;
using Microsoft.AspNetCore.Authorization;

namespace BlazorWasm.Client.Utils;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RolesAttribute : AuthorizeAttribute
{
    public RolesAttribute(params string[] rolesList) => Roles = $"{ConstantRoles.Admin},{string.Join(",", rolesList)}";

    public string[]? RolesList { get; }
}