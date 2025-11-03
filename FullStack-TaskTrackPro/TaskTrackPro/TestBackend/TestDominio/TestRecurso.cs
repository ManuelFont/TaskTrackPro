using Backend.Dominio;

namespace TestBackend.TestDominio;

[TestClass]
public class TestRecurso
{
    [TestMethod]
    public void CrearRecurso()
    {
        Recurso recurso = new Recurso("Rider", "Software", "IDE utilizado", "Fun");
        Assert.AreEqual(recurso.Nombre, "Rider");
        Assert.AreEqual(recurso.Tipo, "Software");
        Assert.AreEqual(recurso.Descripcion, "IDE utilizado");
    }
    
    [TestMethod]
    public void AgregarFuncionalidadSeAgregaCorrectamente()
    {
        Recurso recurso = new Recurso("Rider", "Software", "IDE utilizado", "Fun");
        recurso.AsignarFuncionalidad("C++");
        Assert.AreEqual(recurso.Funcionalidad, "C++");
    }
    
    [TestMethod]
    public void RecursoDisponible_CuandoNoFueUsado_DeberiaEstarDisponible()
    {
        var recurso = new Recurso("Servidor AWS", "Infraestructura", "Servidor EC2", "Fun");
        recurso.AsignarFuncionalidad("C++");

        var fechaInicio = new DateTime(2025, 6, 10);
        int duracion = 3;

        bool disponible = recurso.Disponible(fechaInicio, duracion);

        Assert.IsTrue(disponible);
    }

    [TestMethod]
    public void RecursoDisponible_CuandoYaEstaUsado_DeberiaEstarNoDisponible()
    {
        var fechaInicioProyecto = new DateTime(2025, 6, 20);
        var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

        var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
            RolUsuario.LiderProyecto);
        proyecto.AsignarLider(lider);

        var tarea = new Tarea("Nom", "Des", 4, proyecto);
        var recurso = new Recurso("Servidor AWS", "Infraestructura", "Servidor EC2", "Fun");
        recurso.AsignarFuncionalidad("C++");

        var hoy = DateTime.Today;
        recurso.AsignarFechaDeUso(hoy.AddDays(1), 3, tarea);

        var fechaNueva = hoy.AddDays(2);
        int duracion = 2;

        bool disponible = recurso.Disponible(fechaNueva, duracion);

        Assert.IsFalse(disponible);
    }


    
       [TestMethod]
        public void ProximaDisponibilidad_RecursoLibre_RetornaFechaActual()
        {
            var recurso = new Recurso("Recurso1", "TipoA", "Desc", "Func");
            DateTime hoy = DateTime.Today;

            DateTime resultado = recurso.ProximaDisponibilidad(DateTime.Today, 3);

            Assert.AreEqual(hoy, resultado);
        }

        [TestMethod]
        public void ProximaDisponibilidad_RecursoOcupado_RetornaDiaSiguiente()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var recurso = new Recurso("Recurso1", "TipoA", "Desc", "Func");
            recurso.AsignarFechaDeUso(DateTime.Today, 2, tarea);

            DateTime resultado = recurso.ProximaDisponibilidad(DateTime.Today, 1);

            Assert.AreEqual(DateTime.Today.AddDays(2), resultado);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProximaDisponibilidad_DuracionInvalida_LanzaExcepcion()
        {
            var recurso = new Recurso("Recurso1", "TipoA", "Desc", "Func");

            recurso.ProximaDisponibilidad(DateTime.Today, 0); 
        }
        [TestMethod]
        public void ProximaDisponibilidad_DuracionTresDias_RecursoOcupado_RetornaDiaSiguiente()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var recurso = new Recurso("R", "Tipo", "Desc", "Funcionalidad");
            recurso.AsignarFechaDeUso(DateTime.Today, 5, tarea);

            var fecha = recurso.ProximaDisponibilidad(DateTime.Today,4);

            Assert.AreEqual(DateTime.Today.AddDays(5), fecha);
        }

        
        [TestMethod]
        public void Disponible_RecursoOcupadoEnRangoRetornaFalse()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var recurso = new Recurso("RecursoTest", "Tipo", "Desc", "Func");
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(5), 5, tarea); 
            
            bool disponible = recurso.Disponible(DateTime.Today.AddDays(7), 3);
            
            Assert.IsFalse(disponible);
        }
        
        [TestMethod]
        public void Disponible_RecursoLibreEnRangoRetornaTrue()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var recurso = new Recurso("RecursoTest", "Tipo", "Desc", "Func");
            
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(5), 5, tarea);
            
            bool disponible = recurso.Disponible(DateTime.Today.AddDays(10), 2);
            
            Assert.IsTrue(disponible);
        }
        
        [TestMethod]
        public void AsignarFechaDeUso_SeAgregaRangoCorrectamente()
        {
            var fechaInicioProyecto = new DateTime(2025, 6, 20);
            var proyecto = new Proyecto("ProyectoPrueba", "Desc", fechaInicioProyecto);

            var lider = new Usuario("Lider", "Apellido", "lider@mail.com", "Pass123@", new DateTime(1990, 1, 1),
                RolUsuario.LiderProyecto);
            proyecto.AsignarLider(lider);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var recurso = new Recurso("R", "Tipo", "Desc", "Func");
            var inicio = DateTime.Today.AddDays(1);
            recurso.AsignarFechaDeUso(inicio, 2, tarea);

            Assert.AreEqual(1, recurso.FechasDeUso.Count);
            Assert.AreEqual(inicio, recurso.FechasDeUso[0].Desde);
            Assert.AreEqual(inicio.AddDays(1), recurso.FechasDeUso[0].Hasta);
        }

        [TestMethod]
        public void Recurso_AsignacionDentroDeCapacidad_NoLanzaExcepcion()
        {
            var fechaInicioProyecto = DateTime.Today.AddDays(1);
            var proyecto = new Proyecto("P1", "Proyecto de prueba", fechaInicioProyecto);
            var recurso = new Recurso("R1", "Tipo", "Desc", "Func", proyecto);
            recurso.AsignarCapacidad(2);
            var fechaInicio = fechaInicioProyecto;

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var tarea2 = new Tarea("Nom2", "Des", 3, proyecto);

            
            recurso.AsignarFechaDeUso(fechaInicio, 3, tarea);
            recurso.AsignarFechaDeUso(fechaInicio, 3, tarea2);
            
            Assert.AreEqual(2, recurso.FechasDeUso.Count);
        }


        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Recurso_AsignacionExcedeCapacidad_LanzaExcepcion()
        {
            var fechaInicioProyecto = DateTime.Today.AddDays(1);
            var proyecto = new Proyecto("P1", "Proyecto de prueba", fechaInicioProyecto);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var tarea2 = new Tarea("Nom2", "Des", 3, proyecto);
            var tarea3 = new Tarea("Nom3", "Des", 2, proyecto);

            
            var recurso = new Recurso("R1", "TipoA", "Desc", "Funcionalidad");
            recurso.AsignarCapacidad(2);


          
          
            recurso.AsignarFechaDeUso(new DateTime(2025, 6, 10), 3, tarea);
            recurso.AsignarFechaDeUso(new DateTime(2025, 6, 11), 3, tarea2);
            recurso.AsignarFechaDeUso(new DateTime(2025, 6, 11), 2, tarea3);


          
        }
        
        [TestMethod]
        public void Recurso_DisponibilidadRespetaCapacidad()
        {

            var fechaInicioProyecto = DateTime.Today.AddDays(1);
            var proyecto = new Proyecto("P1", "Proyecto de prueba", fechaInicioProyecto);
            var recurso = new Recurso("R1", "Tipo", "Desc", "Func", proyecto);
            recurso.AsignarCapacidad(2);

            var tarea = new Tarea("Nom", "Des", 4, proyecto);
            var tarea2 = new Tarea("Nom2", "Des", 3, proyecto);

            var fechaInicio = fechaInicioProyecto;
            int duracion = 3;
            

            recurso.AsignarFechaDeUso(fechaInicio, duracion, tarea);
            recurso.AsignarFechaDeUso(fechaInicio, duracion, tarea2); 

            
            Assert.IsFalse(recurso.Disponible(fechaInicio, duracion), "El recurso no debería estar disponible: se alcanzó la capacidad.");
        }

        [TestMethod]
        public void Recurso_Disponible_DevuelveTrueSiNoSuperaCapacidad()
        {
            var recurso = new Recurso("Proyector", "Tecnologia", "desc", "uso", new Proyecto("P", "desc", DateTime.Today));
            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            recurso.AsignarCapacidad(2);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(1), 1, tarea);

            var disponible = recurso.Disponible(DateTime.Today.AddDays(1), 1);

            Assert.IsTrue(disponible);
        }

        [TestMethod]
        public void Recurso_Disponible_DevuelveFalseSiSuperaCapacidad()
        {
            var recurso = new Recurso("Proyector", "Tecnologia", "desc", "uso", new Proyecto("P", "desc", DateTime.Today));
            recurso.AsignarCapacidad(1);
            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(1), 1, tarea); 

            var disponible = recurso.Disponible(DateTime.Today.AddDays(1), 1);

            Assert.IsFalse(disponible); 
        }
        
        [TestMethod]
        public void AsignarFechaDeUso_AgregaRangoCorrectamente()
        {
            var recurso = new Recurso("Laptop", "Tecnologia", "desc", "uso", new Proyecto("P", "desc", DateTime.Today));
            recurso.AsignarCapacidad(1);
            var proyecto = new Proyecto("Proyecto A", "desc", DateTime.Today);
            var tarea = new Tarea("TareaTest", "desc", 3, proyecto);
            recurso.AsignarFechaDeUso(DateTime.Today.AddDays(2), 2, tarea);

            Assert.AreEqual(1, recurso.FechasDeUso.Count);
            var rango = recurso.FechasDeUso.First();
            Assert.AreEqual(DateTime.Today.AddDays(2), rango.Desde);
            Assert.AreEqual(DateTime.Today.AddDays(3), rango.Hasta);
        }
}