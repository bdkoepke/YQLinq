using System;
using System.Collections.Generic;
using System.Text;

namespace YQLinq
{
	public static class IListExtensions
	{
		public static bool IsEmpty<T> (this IList<T> list)
		{
			return list.Count == 0;	
		}

		public static bool IsNotEmpty<T> (this IList<T> list)
		{
			return ! IsEmpty (list);
		}
	}
	
}
