
using Dtos;

namespace TestDtos
{
    [TestClass]
    public class TareaDtoTest
    {
        [TestMethod]
        public void ConstructorPorDefecto_PermiteAsignarPropiedadesEditables()
        {
            var dto = new TareaDto();

            dto.Titulo = "Diseñar base de datos";
            dto.Descripcion = "Modelado de entidades y relaciones";
            dto.Duracion = 5;
            dto.Realizada = false;
            dto.Seleccionada = true;
            dto.DependenciasNombres.Add("Tarea A");

            Assert.AreEqual("Diseñar base de datos", dto.Titulo);
            Assert.AreEqual("Modelado de entidades y relaciones", dto.Descripcion);
            Assert.AreEqual(5, dto.Duracion);
            Assert.IsFalse(dto.Realizada);
            Assert.IsTrue(dto.Seleccionada);
            Assert.AreEqual(1, dto.DependenciasNombres.Count);
            Assert.AreEqual("Tarea A", dto.DependenciasNombres[0]);
        }

        [TestMethod]
        public void ConstructorConParametros_AsignaValoresCorrectamente()
        {
            var fechaInicio = new DateTime(2025, 5, 1);
            var fechaFin = new DateTime(2025, 5, 10);

            var dto = new TareaDto("Tarea X", "Descripción X", 5, fechaInicio, fechaFin, true, 2);

            Assert.AreEqual("Tarea X", dto.Titulo);
            Assert.AreEqual("Descripción X", dto.Descripcion);
            Assert.AreEqual(5, dto.Duracion);
            Assert.AreEqual(fechaInicio, dto.FechaInicioTemprano);
            Assert.AreEqual(fechaFin, dto.FechaFinTemprano);
            Assert.IsTrue(dto.Realizada);
            Assert.AreEqual(2, dto.Holgura);
        }

        [TestMethod]
        public void Seleccionada_PuedeSerAsignadaCorrectamente()
        {
            var dto = new TareaDto();
            dto.Seleccionada = true;
            Assert.IsTrue(dto.Seleccionada);

            dto.Seleccionada = false;
            Assert.IsFalse(dto.Seleccionada);
        }

        [TestMethod]
        public void DependenciasNombres_PuedeSerReemplazadaConNuevaLista()
        {
            var dto = new TareaDto();

            var nuevaLista = new List<string> { "T1", "T2" };
            dto.DependenciasNombres = nuevaLista;

            Assert.AreEqual(2, dto.DependenciasNombres.Count);
            CollectionAssert.AreEqual(nuevaLista, dto.DependenciasNombres);
        }

        [TestMethod]
        public void DependenciasTransitivas_PuedeAsignarseYLeerseCorrectamente()
        {
            var dto = new TareaDto();
            var lista = new List<string> { "A", "B", "C" };

            dto.DependenciasTransitivas = lista;

            Assert.AreEqual(3, dto.DependenciasTransitivas.Count);
            CollectionAssert.AreEqual(lista, dto.DependenciasTransitivas);
        }
    }
}
