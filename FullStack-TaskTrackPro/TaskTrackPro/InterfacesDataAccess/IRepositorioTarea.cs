using System.Linq.Expressions;

namespace InterfacesDataAccess;

public interface IRepositorioTarea <T>
{
    void Agregar(T elemento);
    IList<T> TraerTodos();
    void Actualizar(T elemento);
    T? EncontrarElemento(Expression<Func<T, bool>> filtro);
}