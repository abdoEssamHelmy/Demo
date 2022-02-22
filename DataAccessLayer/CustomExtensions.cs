using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    internal class ExpressionClosureFactory
    {
        public static MemberExpression GetField(object value,Type T)
        {
            string ProperyType = T.ToString().Replace(".", "_");
            var closure = new ExpressionClosureField();
            closure.GetType().GetField(ProperyType).SetValue(closure, value);

            return Expression.Field(Expression.Constant(closure), ProperyType);
        }

        class ExpressionClosureField
        {
            public int System_Int32;
            public long System_Int64;
            public double System_Double;
            public float System_Single;
            public Guid System_Guid;

        }
    }
    public static class CustomExtensions
    {
        public static bool HasProperty(this IReadOnlyList<IProperty> Properties, string PropertyName)
        {
            foreach (var Property in Properties)
            {
                if (Property.Name == PropertyName)
                    return true;
            }
            return false;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(
       this IQueryable<TSource> Query, string PropertyName, string OrderBy)
        {
            string Method = OrderBy.Equals("asc") ? "OrderBy" : "OrderByDescending";
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(PropertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, PropertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == Method && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method
                 .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { Query, selector });
            return newQuery;
        }

    
    }
}
