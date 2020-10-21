using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QStack.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QStack.Framework.Persistent.EFCore
{
    public static class IQueryableExtensions
    {
        /// <summary>
        ///     Specifies related entities to include in the query result.
        /// </summary>
        /// <typeparam name="T">The type of entity being queried.</typeparam>
        /// <param name="source">The source <see cref="IQueryable{T}" /> on which to call Include.</param>
        /// <param name="paths">The lambda expressions representing the paths to include.</param>
        /// <returns>A new <see cref="IQueryable{T}" /> with the defined query path.</returns>
        internal static IQueryable<T> Include<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] paths) where T : class
        {
            if (paths != null)
                source = paths.Aggregate(source, (current, include) => current.Include(include.AsPath()));

            return source;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<string> navigationPropertyPaths)
            where T : class
        {
            return navigationPropertyPaths.Aggregate(source, (query, path) => query.Include(path));
        }

        public static IEnumerable<string> GetIncludePaths(this DbContext context, Type clrEntityType, bool onlyReferenceNavigation = true, bool MutipleLevel = false)
        {
            var entityType = context.Model.FindEntityType(clrEntityType);
            var includedNavigations = new HashSet<INavigation>();
            var stack = new Stack<IEnumerator<INavigation>>();
            while (true)
            {
                var entityNavigations = new List<INavigation>();
                foreach (var navigation in entityType.GetNavigations())
                {
                    if (onlyReferenceNavigation && navigation.IsCollection()) continue;
                    if (includedNavigations.Add(navigation))
                        entityNavigations.Add(navigation);
                }
                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                }
                else
                {
                    foreach (var navigation in entityNavigations)
                    {
                        var inverseNavigation = navigation.FindInverse();
                        if (inverseNavigation != null)
                            includedNavigations.Add(inverseNavigation);
                    }
                    stack.Push(entityNavigations.GetEnumerator());
                }
                while (stack.Count > 0 && !stack.Peek().MoveNext())
                    stack.Pop();
                if (stack.Count == 0) break;
                if (MutipleLevel)
                    entityType = stack.Peek().Current.GetTargetType();
            }
        }


       
    }


    /// <summary>
    ///     查询帮助类，使efcore 可以提供类似ef6 关联查询方式
    ///     Example:IQueryable.Include(t => t.Navigation1, t => t.Navigation2.Select(x => x.Child1))
    ///     Provides extension methods to the <see cref="Expression" /> class.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Converts the property accessor lambda expression to a textual representation of it's path. <br />
        ///     The textual representation consists of the properties that the expression access flattened and separated by a dot character (".").
        /// </summary>
        /// <param name="expression">The property selector expression.</param>
        /// <returns>The extracted textual representation of the expression's path.</returns>
        public static string AsPath(this LambdaExpression expression)
        {
            if (expression == null)
                return null;

            TryParsePath(expression.Body, out var path);

            return path;
        }

        /// <summary>
        ///     Recursively parses an expression tree representing a property accessor to extract a textual representation of it's path. <br />
        ///     The textual representation consists of the properties accessed by the expression tree flattened and separated by a dot character (".").
        /// </summary>
        /// <param name="expression">The expression tree to parse.</param>
        /// <param name="path">The extracted textual representation of the expression's path.</param>
        /// <returns>True if the parse operation succeeds; otherwise, false.</returns>
        private static bool TryParsePath(Expression expression, out string path)
        {
            var noConvertExp = RemoveConvertOperations(expression);
            path = null;

            switch (noConvertExp)
            {
                case MemberExpression memberExpression:
                    {
                        var currentPart = memberExpression.Member.Name;

                        if (!TryParsePath(memberExpression.Expression, out var parentPart))
                            return false;

                        path = string.IsNullOrEmpty(parentPart) ? currentPart : string.Concat(parentPart, ".", currentPart);
                        break;
                    }

                case MethodCallExpression callExpression:
                    switch (callExpression.Method.Name)
                    {
                        case nameof(Queryable.Select) when callExpression.Arguments.Count == 2:
                            {
                                if (!TryParsePath(callExpression.Arguments[0], out var parentPart))
                                    return false;

                                if (string.IsNullOrEmpty(parentPart))
                                    return false;

                                if (!(callExpression.Arguments[1] is LambdaExpression subExpression))
                                    return false;

                                if (!TryParsePath(subExpression.Body, out var currentPart))
                                    return false;

                                if (string.IsNullOrEmpty(parentPart))
                                    return false;

                                path = string.Concat(parentPart, ".", currentPart);
                                return true;
                            }

                        case nameof(Queryable.Where):
                            throw new NotSupportedException("Filtering an Include expression is not supported");
                        case nameof(Queryable.OrderBy):
                        case nameof(Queryable.OrderByDescending):
                            throw new NotSupportedException("Ordering an Include expression is not supported");
                        default:
                            return false;
                    }
            }

            return true;
        }

        /// <summary>
        ///     Removes all casts or conversion operations from the nodes of the provided <see cref="Expression" />.
        ///     Used to prevent type boxing when manipulating expression trees.
        /// </summary>
        /// <param name="expression">The expression to remove the conversion operations.</param>
        /// <returns>The expression without conversion or cast operations.</returns>
        private static Expression RemoveConvertOperations(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
                expression = ((UnaryExpression)expression).Operand;

            return expression;
        }
    }
}
