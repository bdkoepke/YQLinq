using System.Linq.Expressions;
using Remotion.Linq.Parsing;
using System.Text;
using System.Collections.Generic;
using System;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;

namespace YQLinq
{

	public class YqlGeneratorExpressionTreeVisitor : ThrowingExpressionTreeVisitor
	{
		public static string GetYqlExpression (Expression expression, ParameterAggregator parameterAggregator)
		{
			var visitor = new YqlGeneratorExpressionTreeVisitor (parameterAggregator);
			visitor.VisitExpression (expression);
			return visitor.GetYqlExpression ();
		}

		// map expression types to the yql query type
		readonly IDictionary<ExpressionType, string> expressionTypeMap = new Dictionary<ExpressionType, string> {
			{ExpressionType.And, " and "},
			{ExpressionType.Or, " or "},
			{ExpressionType.Add, " + "},
			{ExpressionType.Subtract, " - "},
			{ExpressionType.Multiply, " * "},
			{ExpressionType.Divide, " / "},
		};

		readonly StringBuilder yqlExpression = new StringBuilder ();
		readonly ParameterAggregator parameterAggregator;

		YqlGeneratorExpressionTreeVisitor (ParameterAggregator parameterAggregator)
		{
			this.parameterAggregator = parameterAggregator;
		}

		public string GetYqlExpression ()
		{
			return yqlExpression.ToString ();
		}

		protected override Expression VisitQuerySourceReferenceExpression (Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression expression)
		{
			yqlExpression.Append (expression.ReferencedQuerySource.ItemName);
			return expression;
		}

		protected override Expression VisitBinaryExpression (BinaryExpression expression)
		{
			yqlExpression.Append ("(");
			VisitExpression (expression.Left);
			string value;

			if (expressionTypeMap.TryGetValue (expression.NodeType, out value)) {
				yqlExpression.Append (value);
			} else {
				base.VisitBinaryExpression (expression);
			}

			VisitExpression (expression.Right);
			yqlExpression.Append (")");

			return expression;
		}

		protected override Expression VisitMethodCallExpression (MethodCallExpression expression)
		{
			var supportedMethod = typeof(string).GetMethod ("Contains");
			if (! expression.Method.Equals (supportedMethod)) {
				return base.VisitMethodCallExpression (expression);
			}

			yqlExpression.Append ("(");
			VisitExpression (expression.Object);
			yqlExpression.Append (" like '%'+");
			VisitExpression (expression.Arguments [0]);
			yqlExpression.Append ("+'%')");
			return expression;
		}

		protected override Exception CreateUnhandledItemException<T> (T unhandledItem, string visitMethod)
		{
			string itemText = FormatUnhandledItem (unhandledItem);
			var message = string.Format ("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
			return new NotSupportedException (message);
		}
		
		string FormatUnhandledItem<T> (T unhandledItem)
		{
			var itemAsExpression = unhandledItem as Expression;
			return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format (itemAsExpression) : unhandledItem.ToString ();
		}
	}
}