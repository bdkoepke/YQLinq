using System;
using System.Linq.Expressions;
using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System.Collections.Generic;

namespace YQLinq
{
	public class YqlGeneratorQueryModelVisitor : QueryModelVisitorBase
	{
		public static CommandData GenerateYqlQuery (QueryModel queryModel)
		{
			var visitor = new YqlGeneratorQueryModelVisitor ();
			visitor.VisitQueryModel (queryModel);
			return visitor.GetHqlCommand ();
		}

		readonly QueryPartsAggregator queryParts = new QueryPartsAggregator ();
		readonly ParameterAggregator parameterAggregator = new ParameterAggregator ();
		readonly Dictionary<Type, string> resultOperatorMap = new Dictionary<Type, String> {
			{CountResultOperator, "cast(count({0}) as int)"},
			{SumResultOperator, "cast(sum({0}) as int)"},
			{MinResultOperator, "cast(min({0}) as int)"},
			{MaxResultOperator, "cast(max({0}) as int)"},
		};
		
		public CommandData GetHqlCommand ()
		{
			return new CommandData (queryParts.BuildHQLString (), parameterAggregator.GetParameters ());
		}

		public override void VisitQueryModel (QueryModel queryModel)
		{
			queryModel.SelectClause.Accept (this, queryModel);
			queryModel.MainFromClause.Accept (this, queryModel);
			VisitBodyClauses (queryModel.BodyClauses, queryModel);
			VisitResultOperators (queryModel.ResultOperators, queryModel);
		}

		public override void VisitResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			string value;
			if (resultOperatorMap.TryGetValue (resultOperator.GetType (), out value)) {
				queryParts.SelectPart = string.Format (value, queryParts.SelectPart);
			} else {
				throw new NotSupportedException ();
			}

			base.VisitResultOperator (resultOperator, queryModel, index);
		}

		public override void VisitMainFromClause (MainFromClause fromClause, QueryModel queryModel)
		{
			queryParts.AddFromPart (fromClause);
			base.VisitMainFromClause (fromClause, queryModel);
		}

		public override void VisitSelectClause (SelectClause selectClause, QueryModel queryModel)
		{
			queryParts.SelectPart = GetYqlExpression (selectClause.Selector);
			base.VisitSelectClause (selectClause, queryModel);
		}

		public override void VisitWhereClause (WhereClause whereClause, QueryModel queryModel, int index)
		{
			queryParts.AddWherePart (GetYqlExpression (whereClause.Predicate));
			base.VisitWhereClause (whereClause, queryModel, index);
		}

		public override void VisitOrderByClause (OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			queryParts.AddOrderByPart (orderByClause.Orderings.Select (o => GetYqlExpression (o.Expression)));
			base.VisitOrderByClause (orderByClause, queryModel, index);
		}

		public override void VisitJoinClause (JoinClause joinClause, QueryModel queryModel, int index)
		{
			throw new NotSupportedException ();
			/*
			// HQL joins work differently, need to simulate using a cross join with a where condition
			queryParts.AddFromPart (joinClause);
			queryParts.AddWherePart (
				"({0} = {1})",
				GetHqlExpression (joinClause.OuterKeySelector), 
				GetHqlExpression (joinClause.InnerKeySelector));
			
			base.VisitJoinClause (joinClause, queryModel, index);
			*/
		}

		public override void VisitAdditionalFromClause (AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			queryParts.AddFromPart (fromClause);
			base.VisitAdditionalFromClause (fromClause, queryModel, index);
		}

		public override void VisitGroupJoinClause (GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
			throw new NotSupportedException ();
		}

		string GetYqlExpression (Expression expression)
		{
			return YqlGeneratorExpressionTreeVisitor.GetYqlExpression (expression, parameterAggregator);
		}
	}
}