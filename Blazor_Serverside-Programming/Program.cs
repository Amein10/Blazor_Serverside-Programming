using Blazor_Serverside_Programming.Components;
using Blazor_Serverside_Programming.Components.Account;
using Blazor_Serverside_Programming.Data;
using Blazor_Serverside_Programming.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<AesDecryptHandler>();
builder.Services.AddScoped<AesGcmDecryptHandler>();
builder.Services.AddDataProtection();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var connectionString2 = builder.Configuration.GetConnectionString("FileUploaderConnection")
    ?? throw new InvalidOperationException("Connection string 'FileUploaderConnection' not found.");

builder.Services.AddDbContext<FileInfoDbContext>(options =>
    options.UseSqlite(connectionString2));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<RsaHandler>();
builder.Services.AddSingleton<CertificateHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRole = "Admin";
    const string adminUsername = "admin";
    const string adminEmail = "admin@test.dk";
    const string adminPassword = "Admin123!";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminUser = await userManager.FindByNameAsync(adminUsername);

    if (adminUser is null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}

app.MapGet("/download-file/{id:int}", async (
    int id,
    HttpContext httpContext,
    FileInfoDbContext fileDb,
    IHashingService hashingService,
    IDataProtectionProvider dataProtectionProvider) =>
{
    var userName = httpContext.User.Identity?.Name;

    if (string.IsNullOrWhiteSpace(userName))
    {
        return Results.Unauthorized();
    }

    var file = await fileDb.Files
        .FirstOrDefaultAsync(f => f.Id == id && f.UserName == userName);

    if (file is null || !File.Exists(file.FilePath))
    {
        return Results.NotFound();
    }

    var fileBytes = await File.ReadAllBytesAsync(file.FilePath);

    var protector = dataProtectionProvider
        .CreateProtector("FileVerificationKeyProtector");

    var unprotectedKey = protector.Unprotect(file.VerificationKey);
    var keyBytes = Convert.FromBase64String(unprotectedKey);

    var newHash = hashingService.HmacSha256Hash(fileBytes, keyBytes);

    var newHashBytes = Convert.FromBase64String(newHash);
    var storedHashBytes = Convert.FromBase64String(file.VerificationHash);

    var isValid = CryptographicOperations.FixedTimeEquals(
        newHashBytes,
        storedHashBytes);

    if (!isValid)
    {
        File.Delete(file.FilePath);

        fileDb.Files.Remove(file);
        await fileDb.SaveChangesAsync();

        return Results.BadRequest("File validation failed.");
    }

    File.Delete(file.FilePath);

    fileDb.Files.Remove(file);
    await fileDb.SaveChangesAsync();

    return Results.File(
        fileBytes,
        file.FileType,
        file.FileName);
})
.RequireAuthorization();

app.MapGet("/api/publickey", (RsaHandler rsaHandler) =>
{
    return rsaHandler.GetPublicKey();
});

app.MapGet("/api/certificate", (CertificateHandler certificateHandler) =>
{
    return certificateHandler.GetCertificate();
});

app.MapPost("/api/upload-encrypted-file", async (
    EncryptedFileUploadRequest request,
    CertificateHandler certificateHandler,
    AesDecryptHandler aesDecryptHandler,
    FileInfoDbContext fileDb,
    IHashingService hashingService,
    IDataProtectionProvider dataProtectionProvider) =>
{
    var encryptedFileBytes = Convert.FromBase64String(request.EncryptedFile);
    var encryptedKeyBytes = Convert.FromBase64String(request.EncryptedKey);
    var ivBytes = Convert.FromBase64String(request.IV);

    var aesKey = certificateHandler.Decrypt(encryptedKeyBytes);

    var decryptedFileBytes = aesDecryptHandler.Decrypt(
        encryptedFileBytes,
        aesKey,
        ivBytes);

    var adminFolder = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Files",
        "admin");

    Directory.CreateDirectory(adminFolder);

    var filePath = Path.Combine(adminFolder, request.FileName);

    await File.WriteAllBytesAsync(filePath, decryptedFileBytes);

    var verificationKeyBytes = RandomNumberGenerator.GetBytes(32);
    var verificationKey = Convert.ToBase64String(verificationKeyBytes);

    var protector = dataProtectionProvider
        .CreateProtector("FileVerificationKeyProtector");

    var protectedVerificationKey = protector.Protect(verificationKey);

    var verificationHash =
        hashingService.HmacSha256Hash(decryptedFileBytes, verificationKeyBytes);

    var record = new FileRecord
    {
        FileName = request.FileName,
        FileType = request.FileType,
        FilePath = filePath,
        FileSize = decryptedFileBytes.Length,
        UploadDate = DateTime.UtcNow,
        UserName = "admin",
        VerificationHash = verificationHash,
        VerificationKey = protectedVerificationKey,
        HashAlgorithm = "HMAC-SHA256"
    };

    fileDb.Files.Add(record);
    await fileDb.SaveChangesAsync();

    return Results.Ok($"Encrypted file '{request.FileName}' was received, decrypted and stored for admin.");
});

app.MapPost("/api/upload-encrypted-gcm-file", async (
    EncryptedGcmFileUploadRequest request,
    CertificateHandler certificateHandler,
    AesGcmDecryptHandler aesGcmDecryptHandler,
    FileInfoDbContext fileDb,
    IHashingService hashingService,
    IDataProtectionProvider dataProtectionProvider) =>
{
    var encryptedFileBytes = Convert.FromBase64String(request.EncryptedFile);
    var encryptedKeyBytes = Convert.FromBase64String(request.EncryptedKey);
    var nonceBytes = Convert.FromBase64String(request.Nonce);
    var tagBytes = Convert.FromBase64String(request.Tag);

    var aesKey = certificateHandler.Decrypt(encryptedKeyBytes);

    byte[] decryptedFileBytes;

    try
    {
        decryptedFileBytes = aesGcmDecryptHandler.Decrypt(
            encryptedFileBytes,
            aesKey,
            nonceBytes,
            tagBytes);
    }
    catch (CryptographicException)
    {
        return Results.BadRequest("Contaminated");
    }

    var adminFolder = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Files",
        "admin");

    Directory.CreateDirectory(adminFolder);

    var filePath = Path.Combine(adminFolder, request.FileName);

    await File.WriteAllBytesAsync(filePath, decryptedFileBytes);

    var verificationKeyBytes = RandomNumberGenerator.GetBytes(32);
    var verificationKey = Convert.ToBase64String(verificationKeyBytes);

    var protector = dataProtectionProvider
        .CreateProtector("FileVerificationKeyProtector");

    var protectedVerificationKey = protector.Protect(verificationKey);

    var verificationHash =
        hashingService.HmacSha256Hash(decryptedFileBytes, verificationKeyBytes);

    var record = new FileRecord
    {
        FileName = request.FileName,
        FileType = request.FileType,
        FilePath = filePath,
        FileSize = decryptedFileBytes.Length,
        UploadDate = DateTime.UtcNow,
        UserName = "admin",
        VerificationHash = verificationHash,
        VerificationKey = protectedVerificationKey,
        HashAlgorithm = "HMAC-SHA256"
    };

    fileDb.Files.Add(record);
    await fileDb.SaveChangesAsync();

    return Results.Ok("No contamination detected");
});

app.Run();