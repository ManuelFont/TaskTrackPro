using System.Reflection;
using Backend.Dominio;
using DataAccess;
using Dtos;

[TestClass]
    public class RepositorioUsuariosTests
    {
        private RepositorioUsuarios _repo;

        [TestInitialize]
        public void Setup()
        {
            RepositorioUsuarios._listaUsuarios = new List<Usuario?>();
            _repo = new RepositorioUsuarios();
        }
        
        [TestMethod]
        public void ListaUsuarios_SetAndGet_WorksCorrectly()
        {
            var user = new Usuario("A", "B", "a@b.com", "Pass123@", DateTime.Today.AddYears(-20), RolUsuario.AdminSistema);
            var list = new List<Usuario?> { user };
            _repo.ListaUsuarios = list;
            Assert.AreSame(list, _repo.ListaUsuarios);
        }
        
        

        [TestMethod]
        public void TraerUsuarioPorLogin_ValidCredentials_ReturnsUser()
        {
            var testUser = new Usuario("X", "Y", "x@y.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            RepositorioUsuarios._listaUsuarios.Add(testUser);

            var dto = new LoginDto { Email = "x@y.com", Password = "Pass123@" };
            var result = _repo.TraerUsuarioPorLogin(dto);

            Assert.IsNotNull(result);
            Assert.AreSame(testUser, result);
        }

        [TestMethod]
        public void TraerUsuarioPorLogin_InvalidCredentials_ReturnsNull()
        {
            var testUser = new Usuario("X", "Y", "x@y.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            RepositorioUsuarios._listaUsuarios.Add(testUser);

            var dtoWrongEmail = new LoginDto { Email = "wrong@y.com", Password = "Pass123@" };
            var dtoWrongPwd = new LoginDto { Email = "x@y.com", Password = "Wrong123@" };

            Assert.IsNull(_repo.TraerUsuarioPorLogin(dtoWrongEmail));
            Assert.IsNull(_repo.TraerUsuarioPorLogin(dtoWrongPwd));
        }

        [TestMethod]
        public void TraerListaUsuarios_ListNotNull_ReturnsAllUsers()
        {
            var u1 = new Usuario("A", "B", "a@b.com", "Pass123@", DateTime.Today.AddYears(-30), RolUsuario.AdminProyecto);
            var u2 = new Usuario("C", "D", "c@d.com", "Pass123@", DateTime.Today.AddYears(-25), RolUsuario.AdminSistema);
            RepositorioUsuarios._listaUsuarios.Add(u1);
            RepositorioUsuarios._listaUsuarios.Add(u2);

            var list = _repo.TraerListaUsuarios();
            Assert.AreEqual(2, list.Count);
            CollectionAssert.Contains(list, u1);
            CollectionAssert.Contains(list, u2);
        }
        
        [TestMethod]
        public void BorrarUsuario_UsuarioExistente_DisminuyeConteoYLoRemueve()
        {
            var usuario = new Usuario("Juan", "Pérez", "juan@mail.com", "Pass123.", DateTime.Today.AddYears(-30), RolUsuario.MiembroProyecto);
            RepositorioUsuarios._listaUsuarios.Add(usuario);
            var conteoAntes = RepositorioUsuarios._listaUsuarios.Count;
            _repo.BorrarUsuario(usuario);
            Assert.AreEqual(conteoAntes - 1, RepositorioUsuarios._listaUsuarios.Count, "La lista debe reducirse en uno.");
            CollectionAssert.DoesNotContain(RepositorioUsuarios._listaUsuarios, usuario, "El usuario debe haber sido removido.");
        }
        
        [TestMethod]
        public void TraerUsuariosMiembros_DevuelveSoloUsuariosConRolMiembroProyecto()
        {
            RepositorioUsuarios _repositorio = new RepositorioUsuarios(); 

            var usuario1 = new Usuario("Ana", "Lopez", "ana@correo.com", "Password123.", new DateTime(1990, 1, 1), RolUsuario.MiembroProyecto);
            var usuario2 = new Usuario("Luis", "Garcia", "luis@correo.com", "Password123.", new DateTime(1985, 1, 1), RolUsuario.AdminSistema);
            var usuario3 = new Usuario("Marta", "Diaz", "marta@correo.com", "Password123.", new DateTime(1995, 1, 1), RolUsuario.MiembroProyecto);

            _repositorio.AgregarUsuario(usuario1);
            _repositorio.AgregarUsuario(usuario2);
            _repositorio.AgregarUsuario(usuario3);

            var resultado = _repositorio.traerUsuariosMiembros();

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.Any(u => u.Email == "ana@correo.com"));
            Assert.IsTrue(resultado.Any(u => u.Email == "marta@correo.com"));
            Assert.IsFalse(resultado.Any(u => u.Email == "luis@correo.com"));
        }

       
        
    }








