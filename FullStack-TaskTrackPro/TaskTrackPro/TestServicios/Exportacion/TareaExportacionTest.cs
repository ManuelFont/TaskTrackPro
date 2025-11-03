using Servicios.Exportacion;
using Backend.Dominio;
namespace TestServicios.Exportacion;

[TestClass]
public class TareaExportacionTest
{
    [TestMethod]
    public void TareaExportacion_SeOrdenaPorTituloDescendente()
    {
        var p = new Proyecto("P", "desc", DateTime.Today);
        var t1 = new Tarea("A", "desc", 1, p);
        var t2 = new Tarea("B", "desc", 1, p);
        p.AgregarTarea(t1);
        p.AgregarTarea(t2);

        var te1 = new TareaExportacion(t1);
        var te2 = new TareaExportacion(t2);

        List<TareaExportacion> tareas = new() { te1, te2 };
        tareas.Sort();

        Assert.AreEqual("B", tareas[0].Titulo);
    }
    
    [TestMethod]
    public void TareaExportacion_CompareTo_Null_Returns1()
    {
        var p = new Proyecto("P", "desc", DateTime.Today);
        var tarea = new Tarea("T", "desc", 1, p);
        p.AgregarTarea(tarea);
        var exportacion = new TareaExportacion(tarea);

        int resultado = exportacion.CompareTo(null);

        Assert.AreEqual(1, resultado);
    }
}