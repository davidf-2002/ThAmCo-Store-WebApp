using Auth0.AspNetCore.Authentication;
using WebApp;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Antiforgery;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


// Configure Authentication
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


// Configure centralised HttpClient for ProductsAPI
builder.Services.AddHttpClient("ProductsClient", client =>
{
    var baseUrl = builder.Configuration["ProductsApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(40);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());


// Configure anti-forgery to prevent XSRF attacks
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});


// Inject services in different environments
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IProductsService, ProductsServiceFake>(); 
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.Seconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            }
        );
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
            {
                Console.WriteLine($"Circuit broken due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}. Break duration: {timespan.TotalSeconds} seconds.");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit reset.");
            },
            onHalfOpen: () =>
            {
                Console.WriteLine("Circuit is half-open. Next call is a trial.");
            });
}