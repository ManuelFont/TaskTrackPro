using Backend.Dominio;
using Servicios.Exportacion;
namespace TestServicios.Exportacion;

[TestClass]
public class ProyectoExportacionTest
{
    [TestInitialize]
    public void SetUp()
    {
        
    }
    
    [TestMethod]
    public void ProyectoExportacion_OrdenaPorFechaDeInicio()
    {
        var p1 = new Proyecto("A", "desc", new DateTime(2030, 1, 1));
        var p2 = new Proyecto("B", "desc", new DateTime(2032, 1, 1));

        var pe1 = new ProyectoExportacion(p1);
        var pe2 = new ProyectoExportacion(p2);

        List<ProyectoExportacion> lista = new() { pe2, pe1 };
        lista.Sort();

        Assert.AreEqual("A", lista[0].Nombre);
    }
    

    
    [TestMethod]
    public void ProyectoExportacion_CompareTo_Null_Returns1()
    {
        var proyecto = new Proyecto("P", "desc", DateTime.Today);
        var exportacion = new ProyectoExportacion(proyecto);

        int resultado = exportacion.CompareTo(null);

        Assert.AreEqual(1, resultado);
    }



}