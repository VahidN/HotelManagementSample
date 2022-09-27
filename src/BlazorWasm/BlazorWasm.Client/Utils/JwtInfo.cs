using System.Security.Claims;

namespace BlazorWasm.Client.Utils;

public class JwtInfo
{
    /// <summary>
    ///     The actual token
    /// </summary>
    public string Token { set; get; } = default!;

    /// <summary>
    ///     The JWT's claims list
    /// </summary>
    public IEnumerable<Claim> Claims { set; get; } = new List<Claim>();

    /// <summary>
    ///     The expiration date of the JWT
    /// </summary>
    public DateTime? ExpirationDateUtc { set; get; }

    /// <summary>
    ///     Is this JWT still valid?
    /// </summary>
    public bool IsExpired { set; get; }

    /// <summary>
    ///     The JWT's role-claims list
    /// </summary>
    public IEnumerable<string> Roles { set; get; } = new List<string>();
}