using Microsoft.AspNetCore.Identity;

namespace Blazor_Serverside_Programming.Data;

public class ApplicationUser : IdentityUser
{
    public string? EmailHash { get; set; }
    public string? EmailSalt { get; set; }
    public int EmailHashIterations { get; set; }
    public string? EmailHashAlgorithm { get; set; }
}