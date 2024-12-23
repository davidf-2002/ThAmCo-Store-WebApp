using Auth0.AspNetCore.Authentication;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth:ClientSecret"];
    options.ResponseType = "code";  // Use authorization code flow
    options.Scope = "write:products";  // Request these scopes at login
}).WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0:Audience"];
});


// Configure HttpClient for IProductsService with centralized settings
builder.Services.AddHttpClient("ProductsClient", client =>
{
    var baseUrl = builder.Configuration["ProductsApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(20);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

if (builder.Environment.IsDevelopment())
{
    //builder.Services.AddSingleton<IProductsService, ProductsServiceFake>(); 
    builder.Services.AddScoped<IProductsService, ProductsService>();
}
else 
{
    builder.Services.AddScoped<IProductsService, ProductsService>();
}


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
