using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

// dotnet add package Microsoft.AspNetCore.Authorization

namespace BlazorServer.Common;

public static class PolicyTypes
{
    public const string RequireAdmin = nameof(RequireAdmin);
    public const string RequireCustomer = nameof(RequireCustomer);
    public const string RequireEmployee = nameof(RequireEmployee);
    public const string RequireEmployeeOrCustomer = nameof(RequireEmployeeOrCustomer);

    public static AuthorizationOptions AddAppPolicies(this AuthorizationOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.AddPolicy(RequireAdmin, policy => policy.RequireRole(ConstantRoles.Admin));
        options.AddPolicy(RequireCustomer,
                          policy =>
                              policy.RequireAssertion(context => context.User.HasClaim(claim =>
                                                          string.Equals(claim.Type, ClaimTypes.Role,
                                                                        StringComparison.Ordinal)
                                                          && (string.Equals(claim.Value, ConstantRoles.Admin,
                                                                            StringComparison.Ordinal) ||
                                                              string.Equals(claim.Value,
                                                                            ConstantRoles.Customer,
                                                                            StringComparison.Ordinal)))
                                                     ));
        options.AddPolicy(RequireEmployee,
                          policy =>
                              policy.RequireAssertion(context => context.User.HasClaim(claim =>
                                                          string.Equals(claim.Type, ClaimTypes.Role
                                                                      , StringComparison.Ordinal) &&
                                                          (string.Equals(claim.Value, ConstantRoles.Admin,
                                                                         StringComparison.Ordinal) ||
                                                           string.Equals(claim.Value,
                                                                         ConstantRoles.Employee,
                                                                         StringComparison.Ordinal)))
                                                     ));

        options.AddPolicy(RequireEmployeeOrCustomer,
                          policy =>
                              policy.RequireAssertion(context => context.User.HasClaim(claim =>
                                                          string.Equals(claim.Type, ClaimTypes.Role
                                                                      , StringComparison.Ordinal) &&
                                                          (string.Equals(claim.Value, ConstantRoles.Admin,
                                                                         StringComparison.Ordinal) ||
                                                           string.Equals(claim.Value, ConstantRoles.Employee,
                                                                         StringComparison.Ordinal) ||
                                                           string.Equals(claim.Value, ConstantRoles.Customer,
                                                                         StringComparison.Ordinal)))
                                                     ));
        return options;
    }
}