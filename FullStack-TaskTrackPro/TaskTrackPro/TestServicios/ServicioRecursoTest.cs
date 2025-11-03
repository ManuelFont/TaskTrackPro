using Backend.Dominio;
using DataAccess;
using Dtos;
using Servicios;
using Microsoft.EntityFrameworkCore;

namespace TestServicios
{
    [TestClass]
    public class ServicioRecursoTest
    {

        private RepositorioRecursos _repoRecursos;
        private RepositorioProyectos _repoProyectos;
        private ServicioRecurso _servicioRecurso;
        private SqlContext _context;
        private RecursoDto _recursoDto;
        private ServicioNotificaciones _servicioNotificaciones;
        private ServicioProyecto _servicioProyecto;
        private RepositorioUsuarios _repositorioUsuarios;
        private ServicioUsuario _servicioUsuario;
        private RepositorioNotificaciones _repositorioNotificaciones;
        private RepositorioTareas _repositorioTareas;




        private SqlContext CrearContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
                .Options;
            return new SqlContext(options);
        }

        [TestInitialize]
        public void Initialize()
        {
            var context = CrearContextoInMemory();
            _repoRecursos = new RepositorioRecursos(context);
            _repoProyectos = new RepositorioProyectos(context);
            _repositorioUsuarios = new RepositorioUsuarios(context);
            _repositorioNotificaciones = new RepositorioNotificaciones(context);
            _repositorioTareas = new RepositorioTareas(context);
            _servicioNotificaciones = new ServicioNotificaciones(_repositorioNotificaciones);
            _servicioUsuario = new ServicioUsuario(_repositorioUsuarios, _servicioNotificaciones);
            _servicioRecurso = new ServicioRecurso(_repoRecursos, _repoProyectos, _servicioNotificaciones,
                _repositorioTareas, _repositorioNotificaciones);

            _servicioProyecto = new ServicioProyecto(_repoProyectos, _repositorioUsuarios, _servicioUsuario,
                _repositorioNotificaciones,_servicioNotificaciones, _repositorioTareas);

            _recursoDto = new RecursoDto
            {
                Nombre = "Recurso1",
                Descripcion = "Des",
                Tipo = "Tipo1",
                Funcionalidad = "C++"
            };
        }

        [TestMethod]
        public void AgregarYTraerTodo()
        {
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("ProyectoTest", "Descripcion", DateTime.Today);
            proyecto.Lider = lider;
            proyecto.LiderEmail = lider.Email;
            _repoProyectos.Agregar(proyecto);
            
            var recursoDto = new RecursoDto
            {
                Nombre = "Recurso1",
                Descripcion = "Des",
                Tipo = "Tipo1",
                Funcionalidad = "C++",
                ProyectoNombre = "ProyectoTest",
                Capacidad = 1
            };

            _servicioRecurso.Agregar(recursoDto);
            
            var resultado = _servicioRecurso.TraerTodo().FirstOrDefault();
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Recurso1", resultado.Nombre);
            Assert.AreEqual("Des", resultado.Descripcion);
            Assert.AreEqual("Tipo1", resultado.Tipo);
            Assert.AreEqual("C++", resultado.Funcionalidad);
            Assert.AreEqual("ProyectoTest", resultado.ProyectoNombre);
        }






        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void AsignarRecursoATarea_RecursoNoExiste_LanzaExcepcion()
        {
            var proyecto = new Proyecto("Test", "Desc", DateTime.Today.AddDays(1));
            _repoProyectos.Agregar(proyecto);
            var tarea = new Tarea("Tarea1", "Desc", 3, proyecto);

            _servicioRecurso.AsignarRecursoATarea("Test", "Tarea1", 1, 1, DateTime.Today);

        }

        [TestMethod]
        public void ReprogramarTareaPorConflicto_ReagendaYNotificaCorrectamente()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);
            _repositorioUsuarios.Agregar(lider);

            var tarea = new Tarea("Tarea X", "Descripcion", 3, proyecto);
            var tareaFinal = new Tarea("Tarea Final", "Final", 5, proyecto);

            proyecto.AgregarTarea(tarea);
            proyecto.AgregarTarea(tareaFinal);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Recurso Z", "Tipo", "Descripcion", "Funcionalidad");
            var fechaOcupada = new DateTime(2025, 6, 20);
            recurso.AsignarFechaDeUso(fechaOcupada, 5, tarea);
           
            _repoRecursos.Agregar(recurso);

            var fechaEsperada = recurso.ProximaDisponibilidad(fechaOcupada, tarea.DuracionEnDias);

            _servicioRecurso.ReprogramarTareaPorConflicto(proyecto.Nombre, tarea.Titulo, recurso.Id,
                tarea.DuracionEnDias, fechaOcupada);

            Assert.AreEqual(fechaEsperada, tarea.FechaInicioForzada, "La tarea no fue reagendada correctamente");

            var notificaciones = _servicioNotificaciones.TraerNoVistas(lider.Email);
            Assert.IsTrue(notificaciones.Any(n => n.Mensaje.ToLower().Contains("conflicto")),
                "No se encontró una notificación relacionada al conflicto de recursos.");
        }






        [TestMethod]
        public void AsignarRecursoATarea_RecursoDisponible_SeAsignaYNotifica()
        {
            var proyecto = new Proyecto("Proyecto1", "Desc", DateTime.Today.AddDays(5));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.AdminProyecto);
            proyecto.Lider = lider;

            var tarea = new Tarea("Tarea1", "Desc", 5, proyecto);

            var recurso = new Recurso("RecursoX", "Tipo", "Desc", "Fun");
            recurso.AsignarFuncionalidad("C++");
            proyecto.AgregarTarea(tarea);
            _repoProyectos.Agregar(proyecto);

            _repoRecursos.Agregar(recurso);
            _repoRecursos.Actualizar(recurso);

            var fechaAsignacion = DateTime.Today.AddDays(5);
            _servicioRecurso.AsignarRecursoATarea("Proyecto1", "Tarea1", 1, 1, fechaAsignacion);

            Assert.IsTrue(tarea.Recursos.Contains(recurso));
            var notificaciones = _servicioNotificaciones.TraerNoVistas(lider.Email);
            Assert.IsTrue(notificaciones.Any(n => n.Mensaje.Contains("Recurso") && n.Mensaje.Contains("asignado")));
        }


        [TestMethod]
        public void AgregarRecursoProyecto_AgregaCorrectamenteElRecursoAlProyecto()
        {
            
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

         
            var proyecto = new Proyecto("ProyectoTest", "desc", DateTime.Today);
            proyecto.Lider = lider;
            _repoProyectos.Agregar(proyecto);
            
            var recursoDto = new RecursoDto
            {
                Id = 1,
                Nombre = "Recurso Test",
                Tipo = "TipoX",
                Descripcion = "Descripción de prueba",
                Funcionalidad = "Funcionalidad X",
                ProyectoNombre = "ProyectoTest",
                Capacidad = 2
            };
            
            _servicioRecurso.AgregarRecursoProyecto(recursoDto);
            
            var proyectoActualizado = _repoProyectos.EncontrarElemento(p => p.Nombre == "ProyectoTest");
            Assert.IsNotNull(proyectoActualizado);
            Assert.AreEqual(1, proyectoActualizado.Recursos.Count);
            Assert.AreEqual("Recurso Test", proyectoActualizado.Recursos.First().Nombre);
        }



        [TestMethod]
        public void AsignarRecursoATarea_RecursoDisponible_AsignaCorrectamente()
        {
            var proyecto = new Proyecto("ProyectoX", "Desc", DateTime.Today.AddDays(1));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;
            _repoProyectos.Agregar(proyecto);

            var tarea = new Tarea("TareaTest", "Desc", 3, proyecto);
            proyecto.AgregarTarea(tarea);
            var recurso = new Recurso("RecursoA", "Tipo", "Desc", "Funcionalidad");
            _repoRecursos.Agregar(recurso);
            _repoRecursos.Actualizar(recurso);
            
            _servicioRecurso.AsignarRecursoATarea("ProyectoX", "TareaTest", 1, 1, DateTime.Today.AddDays(1));

            Assert.IsTrue(tarea.Recursos.Contains(recurso));
        }

        [TestMethod]
        public void AsignarRecursoATarea_RecursoDisponible_NoNotificaConflicto()
        {
            var proyecto = new Proyecto("Proyecto A", "Desc", DateTime.Today.AddDays(5));
            var lider = new Usuario("Carlos", "Lopez", "carlos@mail.com", "Pass123@", new DateTime(1988, 3, 10),
                RolUsuario.AdminProyecto);
            proyecto.Lider = lider;

            var tarea = new Tarea("Tarea sin conflicto", "Desc", 2, proyecto);
            var recurso = new Recurso("Recurso Libre", "Tipo", "Desc", "Func");

            proyecto.AgregarTarea(tarea);
            _repoProyectos.Agregar(proyecto);

            _repoRecursos.Agregar(recurso);
            _repoRecursos.Actualizar(recurso);

            var fechaAsignacion = DateTime.Today.AddDays(5);
            _servicioRecurso.AsignarRecursoATarea("Proyecto A", "Tarea sin conflicto", recurso.Id, 1, fechaAsignacion);

            var notificaciones = _servicioNotificaciones.TraerNoVistas(lider.Email);
            Assert.IsFalse(notificaciones.Any(n => n.Mensaje.Contains("Conflicto")));
        }


        [TestMethod]
        public void AsignarRecursoATarea_RecursoNoDisponible_ReagendaYNotifica()
        {
            var fechaInicio = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoTest", "Desc", fechaInicio);
            var lider = new Usuario("Juan", "Pérez", "juan@mail.com", "Pass123@", new DateTime(1990, 1, 1), RolUsuario.AdminProyecto);
            proyecto.AsignarLider(lider);
            _repositorioUsuarios.Agregar(lider);

            var tareaX = new Tarea("Tarea X", "Desc", 3, proyecto);
            var tareaFinal = new Tarea("Tarea Final", "Desc", 10, proyecto);

            proyecto.AgregarTarea(tareaX);
            proyecto.AgregarTarea(tareaFinal);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Recurso Z", "Tipo", "Desc", "Funcionalidad");
            var fechaOcupada = new DateTime(2025, 6, 20);
            recurso.AsignarFechaDeUso(fechaOcupada, 5, tareaX);
            _repoRecursos.Agregar(recurso);

            var fechaEsperada = recurso.ProximaDisponibilidad(fechaOcupada, tareaX.DuracionEnDias);
            
            _servicioRecurso.ReprogramarTareaPorConflicto(proyecto.Nombre, tareaX.Titulo, recurso.Id, 3, fechaOcupada);

            Assert.AreEqual(fechaEsperada, tareaX.FechaInicioForzada, "La tarea debería haber sido reagendada.");

            var notificaciones = _servicioNotificaciones.TraerNoVistas(lider.Email);
            Assert.IsTrue(
                notificaciones.Any(n => n.Mensaje.ToLower().Contains("conflicto")),
                "No se encontró una notificación que mencione un conflicto."
            );
        }



        [TestMethod]
        public void ReagendarTareaPorConflicto_EstableceFechaEjecucionCorrecta()
        {
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", DateTime.Today);
            var tarea = new Tarea("Tarea1", "Desc", 3, proyecto);
            var recurso = new Recurso("Recurso1", "Tipo", "Desc", "Funcionalidad");

            recurso.AsignarFechaDeUso(DateTime.Today, 5, tarea);

            var fechaEsperada = recurso.ProximaDisponibilidad(DateTime.Today, tarea.DuracionEnDias);
            tarea.EstablecerFechaEjecucion(fechaEsperada);

            Assert.AreEqual(DateTime.Today.AddDays(5), tarea.FechaEjecucion,
                "La tarea debería comenzar cuando el recurso esté disponible.");
        }

        [TestMethod]
        public void AsignarRecursoATarea_RecursoNoDisponible_ReagendaFechaInicioForzada()
        {
            var inicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoTest", "Descripción del proyecto", inicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.AdminProyecto);
            proyecto.AsignarLider(lider);

            var tarea1 = new Tarea("Tarea21", "Descripción", 3, proyecto);
            var tareaFinal = new Tarea("Final", "desc", 10, proyecto);

            proyecto.AgregarTarea(tarea1);
            proyecto.AgregarTarea(tareaFinal);

            var recurso = new Recurso("RecursoX", "Tipo", "Desc", "Funcionalidad");
            var fechaOcupada = new DateTime(2025, 6, 21);
            recurso.AsignarFechaDeUso(fechaOcupada, 5, tarea1);

            _repoProyectos.Agregar(proyecto);
            _repoRecursos.Agregar(recurso);
            

            var fechaEsperada = recurso.ProximaDisponibilidad(fechaOcupada, tarea1.DuracionEnDias);

            _servicioRecurso.ReprogramarTareaPorConflicto(proyecto.Nombre, tarea1.Titulo, recurso.Id, tarea1.DuracionEnDias, fechaOcupada);

            Assert.IsNotNull(tarea1.FechaInicioForzada,
                "La tarea debería tener una fecha forzada si no estaba disponible el recurso.");

            Assert.AreEqual(fechaEsperada, tarea1.FechaInicioForzada.Value,
                "La fecha reagendada no coincide con la próxima disponibilidad real del recurso.");
        }



        [TestMethod]
        public void ProximaDisponibilidad_RecursoConUsoPrevio_RetornaFechaPosteriorLibre()
        {
            var proyecto = new Proyecto("Proyecto Critico", "Desc", new DateTime(2025, 6, 20));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;
            var tarea = new Tarea("Tarea A", "Desc", 5, proyecto);
            var recurso = new Recurso("Recurso1", "Tipo", "Desc", "Funcionalidad");

            var fechaOcupadaInicio = new DateTime(2025, 6, 20);
            recurso.AsignarFechaDeUso(fechaOcupadaInicio, 5, tarea);

            var fechaBusqueda = new DateTime(2025, 6, 22);
            int duracion = 3;

            var fechaEsperada = new DateTime(2025, 6, 25);

            var fechaDisponible = recurso.ProximaDisponibilidad(fechaBusqueda, duracion);

            Assert.AreEqual(fechaEsperada, fechaDisponible);
        }

        [TestMethod]
        public void ProximaDisponibilidad_RecursoLibreDesdeInicio_RetornaFechaInicio()
        {
            var recurso = new Recurso("Recurso2", "Tipo", "Desc", "Funcionalidad");

            var fechaInicio = new DateTime(2025, 7, 1);
            int duracion = 2;

            var fechaDisponible = recurso.ProximaDisponibilidad(fechaInicio, duracion);

            Assert.AreEqual(fechaInicio, fechaDisponible);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void AsignarRecursoATarea_RecursoInexistente_LanzaExcepcion()
        {
            var proyecto = new Proyecto("Proyecto X", "Desc", DateTime.Today);
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;
            var tarea = new Tarea("Tarea Inexistente", "Desc", 2, proyecto);
            proyecto.AgregarTarea(tarea);
            _repoProyectos.Agregar(proyecto);
            
            _servicioRecurso.AsignarRecursoATarea("Proyecto X", "Tarea Inexistente", 9999, 1, DateTime.Today);
        }

        [TestMethod]
        public void AsignarRecursoATarea_UsaEquivalenteDisponible_SiPrincipalNoDisponible()
        {
            var fechaHoy = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("Proyecto Equiv", "Desc", fechaHoy);
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);
            _repositorioUsuarios.Agregar(lider);

            var tarea = new Tarea("Tarea Equiv", "Desc", 2, proyecto);
            
            proyecto.AgregarTarea(tarea);

            _repoProyectos.Agregar(proyecto);

            var recursoPrincipal = new Recurso("Principal", "TipoA", "Desc", "FuncA");
            recursoPrincipal.AsignarFechaDeUso(fechaHoy, 5, tarea);

            var recursoEquivalente = new Recurso("Equivalente", "TipoA", "Desc", "FuncA");

            _repoRecursos.Agregar(recursoPrincipal);
            _repoRecursos.Agregar(recursoEquivalente);

            _servicioRecurso.RedistribuirRecursoEquivalente(proyecto.Nombre, tarea.Titulo, recursoPrincipal.Id, 2, fechaHoy);

            Assert.AreEqual(1, tarea.Recursos.Count);
            Assert.AreEqual("Equivalente", tarea.Recursos.First().Nombre);

            var notificaciones = _servicioNotificaciones.TraerNoVistas(lider.Email);
            Assert.IsTrue(notificaciones.Any(n => n.Mensaje.Contains("Recurso 'Equivalente'")));
        }



        [TestMethod]
        public void AsignarRecurso_TareaCritica_AsignacionDentroDeRango_Exito()
        {
            var proyecto = new Proyecto("Proyecto Critico", "Desc", new DateTime(2025, 6, 20));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;
            var tarea = new Tarea("Tarea A", "Desc", 5, proyecto);
            proyecto.AgregarTarea(tarea);

            _repoProyectos.Agregar(proyecto);
            var recurso = new Recurso("R", "Tipo", "D", "F");
            _repoRecursos.Agregar(recurso);

            var fechaAsignacion = new DateTime(2025, 6, 21);
            int duracion = 4;
            
            _servicioRecurso.AsignarRecursoATarea("Proyecto Critico", "Tarea A", 1, 1, fechaAsignacion);

            Assert.AreEqual(1, tarea.Recursos.Count);
            Assert.AreEqual(recurso.Nombre, tarea.Recursos.First().Nombre);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AsignarRecurso_TareaCritica_AsignacionFueraDeRango_LanzaExcepcion()
        {
            var proyecto = new Proyecto("Proyecto Critico", "Desc", new DateTime(2025, 6, 20));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;
            var tarea = new Tarea("Tarea A", "Desc", 5, proyecto);
            proyecto.AgregarTarea(tarea);
            
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("R", "Tipo", "D", "F");
            _repoRecursos.Agregar(recurso);
            
            var fechaAsignacion = new DateTime(2025, 6, 25);
            int duracion = 3;
            
            _servicioRecurso.AsignarRecursoATarea("Proyecto Critico", "Tarea A", recurso.Id, duracion, fechaAsignacion);
        }

        [TestMethod]
        public void AsignarRecurso_TareaNoCritica_DentroDeHolgura_Exito()
        {
            var proyecto = new Proyecto("Proyecto Holgura", "Desc", new DateTime(2025, 6, 20));
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.Lider = lider;

            var tareaA = new Tarea("Tarea A", "Desc", 6, proyecto);
            var tareaB = new Tarea("Tarea B", "Desc", 4, proyecto);


            proyecto.AgregarTarea(tareaA);
            proyecto.AgregarTarea(tareaB);

            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("R", "Tipo", "D", "F");
            _repoRecursos.Agregar(recurso);

            var fechaAsignacion = tareaB.FechaInicioTemprano.AddDays(1);
            _servicioRecurso.AsignarRecursoATarea("Proyecto Holgura", "Tarea B", recurso.Id, 3, fechaAsignacion);


            Assert.AreEqual(1, tareaB.Recursos.Count);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AsignarRecurso_DuracionInvalida_LanzaExcepcion()
        {
            var lider = new Usuario("Nombre", "Apellido", "lider@test.com", "Hola123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            proyecto.AsignarLider(lider);
            proyecto.AgregarTarea(tarea);
            _repoProyectos.Agregar(proyecto);
            

            var recurso = new Recurso("RecursoX", "Tipo", "Desc", "Func");
            _repoRecursos.Agregar(recurso);
            
            _servicioRecurso.AsignarRecursoATarea("Proyecto A", "TareaTest", recurso.Id, 0, DateTime.Today);

        }

        [TestMethod]
        public void CPM_DeberiaAgregarDependenciaPorConflictoDeRecurso()
        {
            var inicio = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("Conflicto de recurso", "desc", inicio);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);
            _repositorioUsuarios.Agregar(lider);

            var tareaA = new Tarea("A", "desc", 3, proyecto);
            var tareaB = new Tarea("B", "desc", 4, proyecto);

            proyecto.AgregarTarea(tareaA);
            proyecto.AgregarTarea(tareaB);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Maria", ".", ".", ".");
            _repoRecursos.Agregar(recurso);
            
            _servicioRecurso.AsignarRecursoATarea("Conflicto de recurso", "B", recurso.Id, 3, new DateTime(2025, 6, 20));


            _servicioRecurso.ReprogramarTareaPorConflicto(
                proyecto.Nombre,
                tareaA.Titulo,
                recurso.Id,
                3,
                new DateTime(2025, 6, 20)
            );
            
            bool ADependeDeB = tareaA.Dependencias.Contains(tareaB);
            Assert.IsTrue(ADependeDeB, "Tarea B debería depender de Tarea A debido a conflicto de recurso.");
            Assert.AreEqual(new DateTime(2025, 6, 23), tareaA.FechaInicioForzada,
                "Tarea B no fue correctamente reagendada tras el conflicto.");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RedistribuirRecursoEquivalente_NoUsaRecursoYaAsignado()
        {
            var inicio = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoEq", "desc", inicio);
            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            proyecto.AgregarTarea(tarea);
            _repoProyectos.Agregar(proyecto);

            var recursoBase = new Recurso("Base", "TipoA", "Desc", "FuncA");
            recursoBase.Id = 1;

            var equivalente1 = new Recurso("Equivalente1", "TipoA", "Desc", "FuncA");
            equivalente1.Id = 2;
            
            _repoRecursos.Agregar(recursoBase);
            _repoRecursos.Agregar(equivalente1);
           
            tarea.AgregarRecurso(equivalente1);

            _servicioRecurso.RedistribuirRecursoEquivalente(proyecto.Nombre, tarea.Titulo, recursoBase.Id, 3, inicio);

        }
        
        [TestMethod]
        public void TraerRecursosDelProyecto_DevuelveRecursosConUsos()
        {
            var lider = new Usuario("Nombre", "Apellido", "lider@test.com", "Hola123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Sala", "Fisico", "desc", "uso", proyecto);
            recurso.AsignarCapacidad(3);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(1), 1, tarea);
            _repoRecursos.Agregar(recurso);
            
            var resultado = _servicioRecurso.TraerRecursosDelProyecto("Proyecto A");
            
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(1, resultado[0].Usos);
            Assert.AreEqual(3, resultado[0].Capacidad);
        }

        [TestMethod]
        public void EliminarRecursoPorId_EliminaRecursoSinTareas()
        {
            var lider = new Usuario("Nombre", "Apellido", "admin@test.com", "Pass1234*", DateTime.Today.AddYears(-20), RolUsuario.AdminSistema);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("ProyectoTemporal", "Desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Sala", "Fisico", "desc", "uso", proyecto);
            recurso.AsignarCapacidad(2);
            _repoRecursos.Agregar(recurso);

            recurso.DesasignarDeProyecto(); 

            int id = recurso.Id;
            
            _servicioRecurso.EliminarRecursoPorId(id);
            
            var recursoEliminado = _repoRecursos.EncontrarElemento(r => r.Id == id);
            Assert.IsNull(recursoEliminado);
        }

        [TestMethod]
        public void TraerPorId_DevuelveRecursoEsperado()
        {
            var lider = new Usuario("Lider", "Apellido", "lider@test.com", "Hola123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto Test", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Proyector", "Fisico", "desc", "proyectar", proyecto);
            _repoRecursos.Agregar(recurso);

            var resultado = _servicioRecurso.TraerPorId(recurso.Id);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Proyector", resultado.Nombre);
        }

        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TraerPorId_LanzaExcepcion_SiNoExiste()
        {
            _servicioRecurso.TraerPorId(999);
        }
        
        [TestMethod]
        public void Actualizar_ModificaCapacidad()
        {
            var lider = new Usuario("Ana", "Test", "ana@test.com", "Hola123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Recurso 1", "Tipo", "desc", "func", proyecto);
            recurso.AsignarCapacidad(1);
            _repoRecursos.Agregar(recurso);

            var dto = new RecursoDto
            {
                Id = recurso.Id,
                Nombre = recurso.Nombre,
                Descripcion = recurso.Descripcion,
                Funcionalidad = recurso.Funcionalidad,
                Capacidad = 5
            };

            _servicioRecurso.Actualizar(dto);

            var actualizado = _repoRecursos.EncontrarElemento(r => r.Id == recurso.Id);
            Assert.AreEqual(5, actualizado.Capacidad);
        }

        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Actualizar_LanzaExcepcion_SiRecursoNoExiste()
        {
            var dto = new RecursoDto
            {
                Id = 999,
                Nombre = "No existe",
                Descripcion = "desc",
                Funcionalidad = "func",
                Capacidad = 5
            };

            _servicioRecurso.Actualizar(dto);
        }
        [TestMethod]
        public void BuscarRecursosEquivalentesDisponibles_DevuelveRecursosDisponiblesCorrectamente()
        {
            var lider = new Usuario("Juan", "Líder", "juan@lider.com", "Clave123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recursoBase = new Recurso("Base", "Tipo1", "desc", "Func", proyecto);
            recursoBase.AsignarCapacidad(1);
            _repoRecursos.Agregar(recursoBase);

            var recursoEquivalente1 = new Recurso("Equivalente1", "Tipo1", "desc", "Func", proyecto);
            recursoEquivalente1.AsignarCapacidad(1);
            _repoRecursos.Agregar(recursoEquivalente1);

            var recursoEquivalente2 = new Recurso("Equivalente2", "Tipo1", "desc", "Func", proyecto);
            recursoEquivalente2.AsignarCapacidad(1);
            _repoRecursos.Agregar(recursoEquivalente2);

            var fecha = DateTime.Today.AddDays(1);
            int duracion = 2;

            var equivalentes = _servicioRecurso.BuscarRecursosEquivalentesDisponibles(recursoBase, fecha, duracion);

            Assert.AreEqual(2, equivalentes.Count);
            Assert.IsTrue(equivalentes.Any(r => r.Nombre == "Equivalente1"));
            Assert.IsTrue(equivalentes.Any(r => r.Nombre == "Equivalente2"));
        }


        [TestMethod]
        public void BuscarRecursosEquivalentesDisponibles_NoIncluyeNoDisponibles()
        {
            var lider = new Usuario("Ana", "Líder", "ana@lider.com", "Clave123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto B", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recursoBase = new Recurso("Base", "Tipo2", "desc", "Func2", proyecto);
            recursoBase.AsignarCapacidad(1);
            _repoRecursos.Agregar(recursoBase);

            var noDisponible = new Recurso("NoDisponible", "Tipo2", "desc", "Func2", proyecto);
            noDisponible.AsignarCapacidad(1);
            _repoRecursos.Agregar(noDisponible);

            var tarea = new Tarea("Tarea X", "desc", 3, proyecto, DateTime.Today.AddDays(1));
            _repositorioTareas.Agregar(tarea);
            noDisponible.AsignarFechaDeUso(DateTime.Today.AddDays(1), 3, tarea);

            var disponible = new Recurso("Disponible", "Tipo2", "desc", "Func2", proyecto);
            disponible.AsignarCapacidad(1);
            _repoRecursos.Agregar(disponible);

            var fecha = DateTime.Today.AddDays(1);
            int duracion = 2;

            var equivalentes = _servicioRecurso.BuscarRecursosEquivalentesDisponibles(recursoBase, fecha, duracion);

            Assert.AreEqual(1, equivalentes.Count);
            Assert.AreEqual("Disponible", equivalentes[0].Nombre);
        }

        [TestMethod]
        public void RecursoDisponible_RetornaFalseSiNoTieneCapacidad()
        {
            var lider = new Usuario("Ana", "Líder", "ana@lider.com", "Clave123@", new DateTime(1990, 1, 1), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto C", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Proyector", "Tecnología", "desc", "Proyectar", proyecto);
            recurso.AsignarCapacidad(1);
            _repoRecursos.Agregar(recurso);

            var tarea = new Tarea("Tarea 1", "desc", 2, proyecto, DateTime.Today.AddDays(1));
            _repositorioTareas.Agregar(tarea);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(1), 2, tarea);

            var disponible = _servicioRecurso.RecursoDisponible(recurso.Id, DateTime.Today.AddDays(1), 2);

            Assert.IsFalse(disponible);
        }
        
        [TestMethod]
        public void RecursoDisponible_RetornaTrueSiTieneCapacidad()
        {
            var lider = new Usuario("Carlos", "Líder", "carlos@lider.com", "Clave456@", new DateTime(1992, 5, 12), RolUsuario.LiderProyecto);
            _repositorioUsuarios.Agregar(lider);

            var proyecto = new Proyecto("Proyecto D", "desc", DateTime.Today);
            proyecto.AsignarLider(lider);
            _repoProyectos.Agregar(proyecto);

            var recurso = new Recurso("Pantalla", "Tecnología", "desc", "Mostrar contenido", proyecto);
            recurso.AsignarCapacidad(2);
            _repoRecursos.Agregar(recurso);

            var tarea = new Tarea("Tarea 2", "desc", 2, proyecto, DateTime.Today.AddDays(1));
            _repositorioTareas.Agregar(tarea);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(1), 2, tarea); 

            var disponible = _servicioRecurso.RecursoDisponible(recurso.Id, DateTime.Today.AddDays(1), 2);

            Assert.IsTrue(disponible); 
        }



        
    }
    
    
    
}