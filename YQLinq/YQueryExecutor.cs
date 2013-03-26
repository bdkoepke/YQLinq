using Remotion.Linq;
using System.Linq;

namespace YQLinq
{
	/// <summary>
	/// Called by Re-Linq when a query is to be executed
	/// </summary>
	public class YQueryExecutor : IQueryExecutor
	{
		readonly ISession session;

		/// <summary>
		/// Initializes a new instance of the <see cref="YQLinq.YQueryExecutor"/> class.
		/// </summary>
		/// <param name='session'>
		/// Session.
		/// </param>
		public YQueryExecutor (ISession session)
		{
			this.session = session;
		}

		#region IQueryExecutor implementation
		/// <inheritdoc/>
		public T ExecuteScalar<T> (QueryModel queryModel)
		{
			return ExecuteCollection<T> (queryModel).Single ();
		}

		/// <inheritdoc/>
		public T ExecuteSingle<T> (QueryModel queryModel, bool returnDefaultWhenEmpty)
		{
			return returnDefaultWhenEmpty
				? ExecuteCollection<T> (queryModel).SingleOrDefault ()
				: ExecuteCollection<T> (queryModel).Single ();
		}

		/// <inheritdoc/>
		public System.Collections.Generic.IEnumerable<T> ExecuteCollection<T> (QueryModel queryModel)
		{
			var commandData = YqlGeneratorQueryModelVisitor.GenerateYqlQuery (queryModel);
			var query = commandData.CreateQuery (session);
			return query.Enumerable<T> ();
		}
		#endregion
	}
}