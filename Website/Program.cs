using Aymeeeric.Website.Components;
using Aymeeeric.Website.Framework.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Spécificité pour tourner sur Heroku (port dynamique du container)...
var dynamicPort = Environment.GetEnvironmentVariable("PORT") ?? string.Empty;
if(dynamicPort.IsNotNullOrEmpty())
    builder.WebHost.ConfigureKestrel((context, serverOptions) =>
    {
        var herokuPort = int.Parse(dynamicPort);
        serverOptions.ListenAnyIP(herokuPort);
    });


builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
