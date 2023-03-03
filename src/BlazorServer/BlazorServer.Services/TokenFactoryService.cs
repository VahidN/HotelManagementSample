using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlazorServer.Services;

public class TokenFactoryService : ITokenFactoryService
{
    private readonly BearerTokensOptions _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenFactoryService(
        UserManager<ApplicationUser> userManager,
        IOptionsSnapshot<BearerTokensOptions> bearerTokensOptions)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        if (bearerTokensOptions is null)
        {
            throw new ArgumentNullException(nameof(bearerTokensOptions));
        }

        _configuration = bearerTokensOptions.Value;
    }

    public async Task<string> CreateJwtTokensAsync(ApplicationUser user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var configurationKey = _configuration.Key;
        if (string.IsNullOrWhiteSpace(configurationKey))
        {
            throw new InvalidOperationException("configurationKey is null");
        }

        var signingCredentials = new SigningCredentials(
                                                        new SymmetricSecurityKey(
                                                         Encoding.UTF8.GetBytes(configurationKey)),
                                                        SecurityAlgorithms.HmacSha256);
        var claims = await GetClaimsAsync(user);
        var now = DateTime.UtcNow;
        var tokenOptions = new JwtSecurityToken(
                                                _configuration.Issuer,
                                                _configuration.Audience,
                                                claims,
                                                now,
                                                now.AddMinutes(_configuration.AccessTokenExpirationMinutes),
                                                signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var issuer = _configuration.Issuer;
        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("issuer is null");
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InvalidOperationException("user.Email is null");
        }

        var claims = new List<Claim>
                     {
                         // Issuer
                         new(JwtRegisteredClaimNames.Iss, issuer, ClaimValueTypes.String, issuer),
                         // Issued at
                         new(JwtRegisteredClaimNames.Iat,
                             DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                             ClaimValueTypes.Integer64, issuer),
                         new(ClaimTypes.Name, user.Email, ClaimValueTypes.String, issuer),
                         new(ClaimTypes.Email, user.Email, ClaimValueTypes.String, issuer),
                         new("Id", user.Id, ClaimValueTypes.String, issuer),
                         new("DisplayName", user.Name ?? user.Email, ClaimValueTypes.String, issuer),
                     };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, issuer));
        }

        return claims;
    }
}