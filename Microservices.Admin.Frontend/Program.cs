using Microservices.Admin.Frontend.Models.Links;
using Microservices.Admin.Frontend.Models.ViewServices.ProductServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductManagementService>(p =>
{
    return new RProductManagementService(new
   RestClient(LinkServices.ApiGatewayAdmin), new HttpContextAccessor());
});

builder.Services.AddAuthentication(c =>
{
    c.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    c.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

}).
AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).
AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = LinkServices.IdentityServer;
    options.ClientId = "adminfrontend";
    options.ClientSecret = "123456";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.SaveTokens = true;
    options.Scope.Add("profile");
    options.Scope.Add("openid");
    options.Scope.Add("apigatewayadmin.fullaccess");
    options.Scope.Add("productservice.admin");
    options.Scope.Add("roles");
    options.ClaimActions.MapUniqueJsonKey("role", "role");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
