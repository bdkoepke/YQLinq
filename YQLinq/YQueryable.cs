using Remotion.Linq.Parsing.Structure;
using Remotion.Linq;

namespace YQLinq
{
	public class YQueryable<T> : QueryableBase<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="YQLinq.YQLinqQueryable`1"/> class.
		/// This constructor is called indirectly by Linq.
		/// </summary>
		/// <param name='queryParser'>
		/// Query parser.
		/// </param>
		/// <param name='executor'>
		/// Executor.
		/// </param>
		public YQueryable (IQueryParser queryParser, IQueryExecutor executor)
			: base(queryParser, executor)
		{
		}

		/// <summary>
		/// Creates the executor.
		/// </summary>
		/// <returns>
		/// The executor.
		/// </returns>
		/// <param name='session'>
		/// Session.
		/// </param>
		static IQueryExecutor CreateExecutor (ISession session)
		{
			return new YQueryExecutor ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YQLinq.YQueryable`1"/> class.
		/// </summary>
		/// <param name='session'>
		/// Session.
		/// </param>
		public YQueryable (ISession session) : base(CreateExecutor(session))
		{
		}
	}
}