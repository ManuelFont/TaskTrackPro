using Dtos;

namespace TestDtos;

[TestClass]
public class RolDtoTest
{
    [TestMethod]
    public void RolDto_AsignarYLeerPropiedades()
    {
        var dto = new RolDto();
        
        dto.Id = 5;
        dto.Nombre = "AdminProyecto";
        
        Assert.AreEqual(5, dto.Id);
        Assert.AreEqual("AdminProyecto", dto.Nombre);
    }
}