using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq;

namespace YQLinq
{
	public class YQLinqQueryProvider : QueryProviderBase
	{
		public YQLinqQueryProvider (IQueryParser queryParser, IQueryExecutor executor) : base(queryParser, executor)
		{
		}

		#region implemented abstract members of QueryProviderBase
		public override IQueryable<T> CreateQuery<T> (Expression expression)
		{
			throw new System.NotImplementedException ();
		}
		#endregion
	}
}