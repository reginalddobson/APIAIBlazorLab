using ApiAiBlazorLab.Data;
using ApiAiBlazorLab.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();
builder.Services.AddScoped<OpenAIChatService>();

builder.Services.AddScoped<OpenAiService>();

builder.Services.AddScoped<ApiAiBlazorLab.Services.CatFactService>();

builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();
app.Logger.LogInformation("OpenAI Key Loaded? {Loaded}",
    !string.IsNullOrWhiteSpace(builder.Configuration["OpenAI:ApiKey"]));

Console.WriteLine("OpenAI Key Loaded? " + (!string.IsNullOrEmpty(builder.Configuration["OpenAI:ApiKey"])));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
