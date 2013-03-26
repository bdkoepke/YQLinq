using Remotion.Linq;
using System;

namespace YQLinq
{

	public class YqlGeneratorQueryModelVisitor : QueryModelVisitorBase
	{
		readonly QueryPartsAggregator queryParts = new QueryPartsAggregator ();
		readonly ParameterAggregator parameterAggregator = new ParameterAggregator ();

		CommandData GetYqlCommand ()
		{
			return new CommandData (queryParts.BuildYqlString (), parameterAggregator.GetParameters ());
		}

		public static CommandData GenerateYqlQuery (QueryModel queryModel)
		{
			var visitor = new YqlGeneratorQueryModelVisitor ();
			visitor.VisitQueryModel (queryModel);
			return visitor.GetYqlCommand ();
		}
	}
}