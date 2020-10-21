using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QStack.Framework.Util
{
	public class ParameterReplacer : ExpressionVisitor
	{
		public ParameterExpression ParameterExpression
		{
			get;
			private set;
		}

		public ParameterReplacer(ParameterExpression paramExpr)
		{
			ParameterExpression = paramExpr;
		}

		public Expression Replace(Expression expr)
		{
			return Visit(expr);
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if(p.Type==ParameterExpression.Type)
				return ParameterExpression;
			return base.VisitParameter(p);
		}
	}

}
