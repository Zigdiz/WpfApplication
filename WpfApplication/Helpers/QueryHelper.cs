using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WpfApplication.Helpers
{
    public static class QueryHelper
    {
        /// <summary>
        /// Filters IQueryable and returns all objects that matches given parameters
        /// </summary>
        /// <typeparam name="TSource">IQueryable type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <typeparam name="TValue">Values collection type</typeparam>
        /// <param name="source">Filtered IQueryable</param>
        /// <param name="propertyExpression">Property expression</param>
        /// <param name="values">Collection where propertys value is</param>
        /// <returns>Filtered IQueryable</returns>
        public static IQueryable<TSource> WherePropertyIn<TSource, TProperty, TValue>(this IQueryable<TSource> source, Expression<Func<TSource, TProperty>> propertyExpression, ICollection<TValue> values)
        {
            if (values == null || !values.Any())
                return source;

            var castedValues = values.Cast<TProperty>().ToList();

            var parameter = Expression.Parameter(typeof(TSource));

            var parameterVisitor = new ParameterVisitor(parameter);
            var visitedPropertyExpression = (Expression<Func<TSource, TProperty>>)parameterVisitor.Visit(propertyExpression);
            var memberExpressionWithReplacedParameter = (MemberExpression)visitedPropertyExpression.Body;

            var expression = Expression.Lambda<Func<TSource, bool>>(
                Expression.Call(Expression.Constant(castedValues),
                    typeof(ICollection<TProperty>).GetMethod("Contains"),
                    memberExpressionWithReplacedParameter), parameter);

            return source.Where(expression);

        }

        private class ParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression toReplace;

            public ParameterVisitor(ParameterExpression toReplace)
            {
                this.toReplace = toReplace;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return toReplace;
            }
        }
    }
}
