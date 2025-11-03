using Microsoft.EntityFrameworkCore;
using Backend.Dominio;

namespace DataAccess
{
    public class SqlContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        
        public DbSet<RangoFecha> RangosFecha { get; set; }

        
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {
            if (!Database.IsInMemory())
                Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Email);

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.FechaNacimiento)
                    .IsRequired();

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasMany(u => u.ListaProyectos)
                    .WithMany(p => p.ListaUsuarios);

                entity.Property(u => u.RolesSerializados).IsRequired();
                entity.Ignore(u => u.Roles);
            });

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario("Admin", "Sistema", "adminSistema@mail.com", "Pass123@", new DateTime(1995, 1, 1), RolUsuario.AdminSistema)
            );
            
            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("Proyecto");
                
                entity.HasKey(p => p.Nombre);

                entity.Property(p => p.Nombre)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(p => p.Descripcion)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(p => p.FechaInicioEstimada)
                    .IsRequired();

                entity.HasMany(p => p.ListaUsuarios)
                    .WithMany(u => u.ListaProyectos);

                entity.HasMany(p => p.ListaDeTareas)
                    .WithOne(t => t.Proyecto)
                    .HasForeignKey("ProyectoNombre");
                
                entity.HasOne(p => p.Lider)
                    .WithMany()
                    .HasForeignKey(p => p.LiderEmail)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Recurso>(entity =>
            {
                
                entity.ToTable("Recurso");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.Property(r => r.Nombre).IsRequired();
                entity.Property(r => r.Tipo).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Descripcion).IsRequired().HasMaxLength(250);
                entity.Property(r => r.ProyectoNombre).HasMaxLength(450);
                entity.Property(r => r.Funcionalidad).IsRequired().HasMaxLength(100); 
                entity.Property(r => r.UtilizadoHasta).IsRequired(false);
                entity.Property(r => r.Capacidad).IsRequired();
                entity.Property(r => r.Usos).IsRequired().HasDefaultValue(0);



                modelBuilder.Entity<Tarea>()
                    .HasMany(t => t.Recursos)
                    .WithMany(r => r.Tareas)
                    .UsingEntity(j => j.ToTable("TareaRecurso"));

                entity.HasOne(r => r.Proyecto).WithMany(p => p.Recursos);
            });
            
            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.ToTable("Tarea");
                modelBuilder.Entity<Tarea>().HasKey(t => t.Titulo);
                modelBuilder.Entity<Tarea>().Property(t => t.Titulo).HasColumnName("Titulo");

                entity.Property(t => t.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(t => t.Descripcion)
                    .IsRequired();

                entity.Property(t => t.DuracionEnDias)
                    .IsRequired();

                entity.Property(t => t.Realizada)
                    .IsRequired();

                entity.Property(t => t.ProyectoNombre).HasMaxLength(450);
                entity.HasOne(t => t.Proyecto)
                    .WithMany(p => p.ListaDeTareas);
                
                entity.Property(t => t.FechaInicioForzada)
                    .HasColumnName("FechaInicioForzada")
                    .IsRequired(false); 
            });
            modelBuilder.Entity<Tarea>()
                .HasMany(t => t.Dependencias)
                .WithMany(t => t.Requeridores)
                .UsingEntity<Dictionary<string, object>>(
                    "TareaDependencia",
                    j => j
                        .HasOne<Tarea>()
                        .WithMany()
                        .HasForeignKey("TareaId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Tarea>()
                        .WithMany()
                        .HasForeignKey("DependenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                    );
           
            
            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.Mensaje)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(n => n.Fecha)
                    .IsRequired();
                
                entity.Property(n => n.Vista)
                    .IsRequired();

                entity.Property(n => n.UsuarioEmail)
                    .IsRequired();

                entity.HasOne(n => n.Usuario)
                    .WithMany(u => u.Notificaciones)
                    .HasForeignKey(n => n.UsuarioEmail);
            });
            
           modelBuilder.Entity<RangoFecha>(entity =>
           {
               entity.HasKey(rf => rf.Id);
               entity.Property(rf => rf.Desde).IsRequired().HasColumnName("Desde");
               entity.Property(rf => rf.Hasta).IsRequired();
               entity.HasOne(rf => rf.Recurso)
                   .WithMany(r => r.FechasDeUso)
                   .HasForeignKey(rf => rf.RecursoId)
                   .OnDelete(DeleteBehavior.Cascade);
               
               entity.HasOne(rf => rf.Tarea)
                   .WithMany(t => t.RangosDeUso)
                   .HasForeignKey(rf => rf.TareaTitulo)
                   .OnDelete(DeleteBehavior.Cascade);
           });
           
           
        }
    }
}