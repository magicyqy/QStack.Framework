using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QStack.Framework.Util
{
	
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>(Expression<Func<T, bool>> expr = null)
		{
			if (expr != null)
			{
				return expr;
			}
			return (T f) => true;
		}

		public static Expression<Func<T, bool>> False<T>(Expression<Func<T, bool>> expr = null)
		{
			if (expr != null)
			{
				return expr;
			}
			return (T f) => false;
		}

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exprLeft, Expression<Func<T, bool>> exprRight)
		{
			ParameterExpression candidateExpression = GetCandidateExpression<T>();
			ParameterReplacer parameterReplacer = new ParameterReplacer(candidateExpression);
			Expression left = parameterReplacer.Replace(exprLeft.Body);
			Expression right = parameterReplacer.Replace(exprRight.Body);
			return Expression.Lambda<Func<T, bool>>(Expression.Or(left, right), new ParameterExpression[1]
			{
			candidateExpression
			});
		}

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exprLeft, Expression<Func<T, bool>> exprRight)
		{
			ParameterExpression candidateExpression = GetCandidateExpression<T>();
			ParameterReplacer parameterReplacer = new ParameterReplacer(candidateExpression);
			Expression left = parameterReplacer.Replace(exprLeft.Body);
			Expression right = parameterReplacer.Replace(exprRight.Body);
			return Expression.Lambda<Func<T, bool>>(Expression.And(left, right), new ParameterExpression[1]
			{
			candidateExpression
			});
		}

		public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
		{
			ParameterExpression parameterExpression = expr.Parameters[0];
			return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), new ParameterExpression[1]
			{
			parameterExpression
			});
		}

		private static ParameterExpression GetCandidateExpression<T>()
		{
			return Expression.Parameter(typeof(T), "candidate");
		}
	}
}
