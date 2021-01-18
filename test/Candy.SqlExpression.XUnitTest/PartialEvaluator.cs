using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Candy.SqlExpression.XUnitTest
{
	public class PartialEvaluator : ExpressionVisitor
	{
		private readonly Func<Expression, bool> _fnCanBeEvaluated;
		private HashSet<Expression> _candidates;

		public PartialEvaluator() : this(CanBeEvaluatedLocally) { }

		public PartialEvaluator(Func<Expression, bool> fnCanBeEvaluated) => _fnCanBeEvaluated = fnCanBeEvaluated;

		public Expression Eval(Expression expression)
		{
			_candidates = new Nominator(_fnCanBeEvaluated).Nominate(expression);

			return Visit(expression);
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null) return null;

			if (_candidates.Contains(expression)) return Evaluate(expression);

			return base.Visit(expression);
		}

		private static Expression Evaluate(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Constant) return expression;

			LambdaExpression lambda = Expression.Lambda(expression);
			Delegate fn = lambda.Compile();
			return Expression.Constant(fn.DynamicInvoke(null), expression.Type);
		}

		private static bool CanBeEvaluatedLocally(Expression expression) => expression.NodeType != ExpressionType.Parameter;

		#region Nominator

		/// <summary>
		/// Performs bottom-up analysis to determine which nodes can possibly
		/// be part of an evaluated sub-tree.
		/// </summary>
		private class Nominator : ExpressionVisitor
		{
			private readonly Func<Expression, bool> _fnCanBeEvaluated;
			private readonly HashSet<Expression> _candidates;
			private bool _cannotBeEvaluated;

			internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
			{
				_candidates = new HashSet<Expression>();
				_fnCanBeEvaluated = fnCanBeEvaluated;
			}

			internal HashSet<Expression> Nominate(Expression expression)
			{
				Visit(expression);
				return _candidates;
			}

			public override Expression Visit(Expression expression)
			{
				if (expression == null) return expression;

				bool saveCannotBeEvaluated = _cannotBeEvaluated;
				_cannotBeEvaluated = false;

				base.Visit(expression);

				if (!_cannotBeEvaluated)
				{
					if (_fnCanBeEvaluated(expression))
						_candidates.Add(expression);

					else
						_cannotBeEvaluated = true;
				}

				_cannotBeEvaluated |= saveCannotBeEvaluated;

				return expression;
			}
		}

		#endregion
	}
}
