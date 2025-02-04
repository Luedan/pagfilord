using System.Linq.Expressions;
using System.Reflection;

namespace PagFilOrd;

public class Helper
{
    public static Expression<Func<T, bool>> BuildPredicate<T>(Dictionary<string, object> filters, string globalSearch = null, List<string> camposBusqueda = null)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression combinedExpression = Expression.Constant(true);

        // Filtros específicos por columnas
        foreach (var filter in filters)
        {
            Expression property = parameter;
            foreach (var part in filter.Key.Split('.'))
            {
                property = Expression.Property(property, part);
            }

            var propertyType = property.Type;

            if (filter.Value == null)
            {
                var isNullExpression = Expression.Equal(property, Expression.Constant(null, propertyType));
                combinedExpression = Expression.AndAlso(combinedExpression, isNullExpression);
            }
            else if (filter.Value is IEnumerable<object> listaValores)
            {
                Expression orExpression = Expression.Constant(false);
                foreach (var valor in listaValores)
                {
                    if (valor == null)
                    {
                        orExpression = Expression.OrElse(orExpression, Expression.Equal(property, Expression.Constant(null, propertyType)));
                    }
                    else
                    {
                        var constant = Expression.Constant(Convert.ChangeType(valor, propertyType));
                        var equals = Expression.Equal(property, constant);
                        orExpression = Expression.OrElse(orExpression, equals);
                    }
                }
                combinedExpression = Expression.AndAlso(combinedExpression, orExpression);
            }
            else
            {
                var constant = Expression.Constant(Convert.ChangeType(filter.Value, propertyType));
                var equals = Expression.Equal(property, constant);
                combinedExpression = Expression.AndAlso(combinedExpression, equals);
            }
        }

        // Filtro global
        if (!string.IsNullOrEmpty(globalSearch))
        {
            Expression globalExpression = Expression.Constant(false);

            // Si no se proporcionan campos de búsqueda específicos, buscar en todas las propiedades
            var targetProperties = camposBusqueda ?? typeof(T).GetProperties().Select(p => p.Name).ToList();
            foreach (var propertyName in targetProperties)
            {
                Expression property = parameter;
                foreach (var part in propertyName.Split('.'))
                {
                    property = Expression.Property(property, part);
                }

                var propertyType = property.Type;
                if (propertyType == typeof(string))
                {
                    var searchValue = Expression.Constant(globalSearch);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsExpression = Expression.Call(property, containsMethod, searchValue);
                    globalExpression = Expression.OrElse(globalExpression, containsExpression);
                }
            }

            combinedExpression = Expression.AndAlso(combinedExpression, globalExpression);
        }

        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }

    public static IQueryable<T> ApplySorting<T>(IQueryable<T> source, string sortColumns)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var sortPart in sortColumns.Split(','))
        {
            var sortParams = sortPart.Trim().Split(' ');
            var sortColumn = sortParams[0];
            var direction = sortParams.Length > 1 ? sortParams[1].ToUpper() : "ASC";

            Expression property = parameter;
            foreach (var part in sortColumn.Split('.'))
            {
                property = Expression.Property(property, part);
            }

            var lambda = Expression.Lambda(property, parameter);
            string methodName;

            if (orderedQuery == null)
            {
                methodName = direction == "DESC" ? "OrderByDescending" : "OrderBy";
                orderedQuery = (IOrderedQueryable<T>)typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type)
                    .Invoke(null, new object[] { source, lambda })!;
            }
            else
            {
                methodName = direction == "DESC" ? "ThenByDescending" : "ThenBy";
                orderedQuery = (IOrderedQueryable<T>)typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type)
                    .Invoke(null, new object[] { orderedQuery, lambda })!;
            }
        }

        return orderedQuery ?? source;
    }
    
    public static PaginatedResult<T> ApplyPagination<T>(IQueryable<T> source, int? pageNumber, int? pageSize)
    {
        // Si no se pasa página o tamaño, devolver toda la data
        if (pageNumber == null || pageSize == null)
        {
            return new PaginatedResult<T>
            {
                Items = source.ToList(),
                TotalItems = source.Count(),
                CurrentPage = 1, // O cualquier valor que indique que no hay paginación activa
                TotalPages = 1,  // Solo un total de página
                PageSize = source.Count() // La cantidad de elementos como PageSize
            };
        }

        // Si se pasa tamaño y página, aplicar paginación
        var totalItems = source.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = source.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();

        return new PaginatedResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            PageSize = pageSize.Value
        };
    }
    
    public static List<string> GetPropertyNames<T>()
    {
        // Obtiene todas las propiedades públicas de la entidad T
        return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}