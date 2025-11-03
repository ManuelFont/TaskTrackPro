using Backend.Dominio;
using DataAccess;
using Dtos;
using Microsoft.IdentityModel.Tokens;

namespace TestDataAccess
{
    [TestClass]
    public class RepositorioUsuariosTest
    {
        private SqlContext _context;
        private RepositorioUsuarios _repositorio;
        private readonly InMemoryAppContextFactory _factory = new();

    
        [TestInitialize]
        public void Setup()
        {
            string dbName = "TestDb_" + Guid.NewGuid();
            _context = _factory.CreateDbContext(dbName);
            _repositorio = new RepositorioUsuarios(_context);
        }
    

    [TestMethod]
        public void InicializarUsuarios_DebeAgregarTresUsuarios()
        {

         
                Usuario adminSistema = new Usuario("Administrador", "Sistema", "adminSistema@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.AdminSistema);
                Usuario adminProyecto = new Usuario("Administrador", "Proyecto", "adminProyecto@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.AdminProyecto);
                Usuario miembro = new Usuario("Miembro", "Proyecto", "miembro@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.MiembroProyecto);
                _repositorio.Agregar(adminSistema);
                _repositorio.Agregar(adminProyecto);
                _repositorio.Agregar(miembro);
            

            var usuarios = _repositorio.TraerTodos();
            Assert.AreEqual(15, usuarios.Count);
            Assert.IsTrue(usuarios.Any(u => u.Email == "adminSistema@mail.com"));
            Assert.IsTrue(usuarios.Any(u => u.Email == "adminProyecto@mail.com"));
            Assert.IsTrue(usuarios.Any(u => u.Email == "miembro@mail.com"));
        }


        [TestMethod]
        public void TraerListaUsuarios_DebeRetornarListaDeUsuariosInicializada()
        {

            if (_repositorio.TraerTodos().IsNullOrEmpty())
            {
                Usuario adminSistema = new Usuario("Administrador", "Sistema", "adminSistema@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.AdminSistema);
                Usuario adminProyecto = new Usuario("Administrador", "Proyecto", "adminProyecto@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.AdminProyecto);
                Usuario miembro = new Usuario("Miembro", "Proyecto", "miembro@mail.com", "Pass123@", new DateTime(1995, 5, 5), RolUsuario.MiembroProyecto);
                _repositorio.Agregar(adminSistema);
                _repositorio.Agregar(adminProyecto);
                _repositorio.Agregar(miembro);
            }

            var usuarios = _repositorio.TraerTodos();

            Assert.IsNotNull(usuarios);
            Assert.AreEqual(15, usuarios.Count);
        }
        
        [TestMethod]
        public void BorrarUsuario_DebeEliminarUsuarioDeRepositorioYDeTodosSusProyectos()
        {
            var proyecto = new Proyecto("Proyecto Test", "Descripcion", DateTime.Today.AddDays(1));
            var usuarioLider = new Usuario("Lider", "Test", "lider10@test.com", "Password123.", new DateTime(2000, 1, 1), RolUsuario.LiderProyecto);

            proyecto.AsignarLider(usuarioLider);
            var usuario = new Usuario("Ana", "Tester", "ana10@mail.com", "Test123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);

            proyecto.AgregarMiembroAlProyecto(usuario);
            usuario.ListaProyectos.Add(proyecto);

            _repositorio.Agregar(usuario);

            _repositorio.Eliminar(u => u.Email == usuario.Email);
            
            Assert.IsFalse(proyecto.ListaUsuarios.Contains(usuario));
        }
        
        [TestMethod]
        public void TraerListaUsuarios_DeberiaRetornarLaListaCorrecta()
        {
            var usuario = new Usuario("A", "B", "a@b.com", "Pass123@", DateTime.Today.AddYears(-20), RolUsuario.AdminSistema);
            _repositorio.Agregar(usuario);

            var resultado = _repositorio.TraerTodos();

            Assert.AreEqual(17, resultado.Count);
            Assert.AreEqual("juan@mail.com", resultado[0].Email);
        }
        
        [TestMethod]
        public void TraerUsuarioPorLogin_CredencialesValidas_DeberiaRetornarUsuario()
        {

            var testUser = new Usuario("X", "Y", "x@y.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            _repositorio.Agregar(testUser);

            var dto = new LoginDto { Email = "x@y.com", Password = "Pass123@" };

            var resultado = _repositorio.EncontrarElemento(u => u.Email == dto.Email);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("x@y.com", resultado.Email);
        }

        [TestMethod]
        public void TraerUsuarioPorLogin_CredencialesInvalidas_DeberiaRetornarNull()
        {
            var testUser = new Usuario("X", "Y", "x12@y.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            _repositorio.Agregar(testUser);

            var dtoEmailIncorrecto = new LoginDto { Email = "wrong@y.com", Password = "Pass123@" };
            var dtoPasswordIncorrecto = new LoginDto { Email = "x12@y.com", Password = "Wrong123@" };

            Assert.IsNull(_repositorio.EncontrarElemento(u => u.Email == dtoEmailIncorrecto.Email));
            Assert.IsNull(_repositorio.EncontrarElemento(u => u.Password == dtoPasswordIncorrecto.Password));
        }
        [TestMethod]
        public void TraerListaUsuarios_ConUsuariosEnBase_DeberiaRetornarTodos()
        {
            var u1 = new Usuario("A", "B", "a12@b.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.AdminProyecto);
            var u2 = new Usuario("C", "D", "c12@d.com", "Pass123@", DateTime.Today.AddYears(-25), RolUsuario.AdminSistema);

            _repositorio.Agregar(u1);
            _repositorio.Agregar(u2);

            var lista = _repositorio.TraerTodos();

            Assert.AreEqual(21, lista.Count);
            Assert.IsTrue(lista.Any(u => u.Email == "a12@b.com"));
            Assert.IsTrue(lista.Any(u => u.Email == "c12@d.com"));
        }
        [TestMethod]
        public void BorrarUsuario_UsuarioExistente_DeberiaReducirConteoYEliminarlo()
        {
            var usuario = new Usuario("Juan", "Pérez", "juan14@mail.com", "Pass123.", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            _repositorio.Agregar(usuario);

            var conteoAntes = _repositorio.TraerTodos().Count;

            _repositorio.Eliminar(u => u.Email == usuario.Email);

            var usuariosRestantes = _context.Usuarios.ToList();

            Assert.AreEqual(conteoAntes - 1, usuariosRestantes.Count, "El conteo debe reducirse en uno.");
            Assert.IsFalse(usuariosRestantes.Any(u => u.Email == "juan14@mail.com"), "El usuario debe haber sido eliminado.");
        }
        [TestMethod]
        public void TraerUsuariosMiembros_DeberiaRetornarSoloUsuariosConRolMiembroProyecto()
        {
            var usuario1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
            var usuario2 = new Usuario("Luis", "Garcia", "luis@correo.com", "Password123.", new DateTime(1985, 1, 1), RolUsuario.AdminSistema);
            var usuario3 = new Usuario("Marta", "Diaz", "marta@correo.com", "Password123.", new DateTime(1995, 1, 1), RolUsuario.MiembroProyecto);

            _repositorio.Agregar(usuario1);
            _repositorio.Agregar(usuario2);
            _repositorio.Agregar(usuario3);

            List<Usuario> miembros = _repositorio.EncontrarLista(u => u.RolesSerializados.Contains("MiembroProyecto")).ToList();

            Assert.AreEqual(9, miembros.Count);
            Assert.IsTrue(miembros.Any(u => u.Email == "ana@correo.com"));
            Assert.IsTrue(miembros.Any(u => u.Email == "marta@correo.com"));
            Assert.IsFalse(miembros.Any(u => u.Email == "luis@correo.com"));
        }
        
        [TestMethod]
        public void TraerUsuarioPorLogin_DeberiaIncluirListaProyectos()
        {
            var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today.AddDays(1));
            var usuario = new Usuario("Test", "User", "test@correo.com", "GDianesi1234..", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
            
            _repositorio.Agregar(usuario);
            usuario.AgregarProyecto(proyecto);
            proyecto.AgregarMiembroAlProyecto(usuario);
        
        
            var loginDto = new LoginDto { Email = "test@correo.com", Password = "GDianesi1234.." };
        
            var resultado = _repositorio.EncontrarElemento(u => u.Email == loginDto.Email);
        
            Assert.AreEqual("test@correo.com", resultado.Email);
            Assert.AreEqual("Proyecto Test", resultado.ListaProyectos.First().Nombre);
        }
        [TestMethod]
        public void TraerUsuarioPorLogin_DeberiaIncluirTareasDeProyectos()
        {
        
            var proyecto = new Proyecto("Proyecto Test", "Descripción", DateTime.Today.AddDays(1));
            var usuario = new Usuario("Test", "User", "test13@correo.com", "GDianesi1234..", DateTime.Today.AddYears(-25), RolUsuario.MiembroProyecto);
            var Tarea = new Tarea("testTarea", "descr", 4, proyecto);
            proyecto.AgregarTarea(Tarea);
            _repositorio.Agregar(usuario);
            usuario.AgregarProyecto(proyecto);
            proyecto.AgregarMiembroAlProyecto(usuario);
            
            var loginDto = new LoginDto { Email = "test13@correo.com", Password = "GDianesi1234.." };
        
            var resultado = _repositorio.EncontrarElemento(u => u.Email == loginDto.Email);
        
            Assert.AreEqual("test13@correo.com", resultado.Email);
            Assert.AreEqual("Proyecto Test", resultado.ListaProyectos.First().Nombre);
            
            var proyectoObtenido = resultado.ListaProyectos.First();
            Assert.IsTrue(proyectoObtenido.ListaDeTareas.Any(t => t.Titulo == "testTarea"));
        }

        [TestMethod]
        public void TraerLiderDeProyectoDebeRetornarUsuarioCorrecto()
        {
            var usuario = new Usuario("Test", "User", "test11@correo.com", "GDianesi1234..", DateTime.Today.AddYears(-25), RolUsuario.LiderProyecto);
            _repositorio.Agregar(usuario);
            List<Usuario> lista = _repositorio.EncontrarLista(u => u.RolesSerializados.Contains("LiderProyecto")).ToList();
            
            Assert.AreEqual("lider2@test.com", lista.First().Email);
        }

    }
    
    
}
