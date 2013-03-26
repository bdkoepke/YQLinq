using System;

namespace YQLinq
{
	public class YQueryFactory
	{
		public static YQueryable<T> Queryable<T> (ISession session)
		{
			return new YQueryable<T> (session);
		}
	}
}

