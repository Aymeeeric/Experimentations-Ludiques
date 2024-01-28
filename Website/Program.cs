using Aymeeeric.Website.Components;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Spécificité pour tourner sur Heroku...
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    var herokuPort = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? throw new Exception("Impossible de trouver le port de lancement."));
    serverOptions.ListenAnyIP(herokuPort);
});

// Add services to the container.
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
