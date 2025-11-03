using Backend.Dominio;
using DataAccess;
using Servicios;
using FrontEnd.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using InterfacesDataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepositorio<Notificacion>, RepositorioNotificaciones>();
builder.Services.AddScoped<IRepositorio<Proyecto>, RepositorioProyectos>();
builder.Services.AddScoped<IRepositorioTarea<Tarea>, RepositorioTareas>();
builder.Services.AddScoped<IRepositorio<Usuario>, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorio<Recurso>, RepositorioRecursos>();

builder.Services.AddScoped<ServicioUsuario>();
builder.Services.AddScoped<ServicioInicioDeSesion>();
builder.Services.AddScoped<ServicioProyecto>();
builder.Services.AddScoped<ServicioNotificaciones>();
builder.Services.AddScoped<ServicioRecurso>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    IRepositorio<Usuario> repoUsuarios = scope.ServiceProvider.GetRequiredService<IRepositorio<Usuario>>();
    
    if (repoUsuarios.TraerTodos().IsNullOrEmpty())
    {
        Usuario adminSistema = new Usuario("Administrador", "Sistema", "adminSistema@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.AdminSistema);
        repoUsuarios.Agregar(adminSistema);
  
    }
}

app.Run();