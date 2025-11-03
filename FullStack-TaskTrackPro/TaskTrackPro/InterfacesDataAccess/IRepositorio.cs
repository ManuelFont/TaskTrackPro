using System.Linq.Expressions;

namespace InterfacesDataAccess;

public interface IRepositorio <T>
{
    void Agregar(T elemento);
    IList<T> TraerTodos();
    IList<T> EncontrarLista(Expression<Func<T, bool>> filtro);
    T? EncontrarElemento(Expression<Func<T, bool>> filtro);
    void Actualizar(T elemento);
    void Eliminar(Func<T, bool> filtro);
}