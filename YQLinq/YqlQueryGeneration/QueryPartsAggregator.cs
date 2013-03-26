using System;
using System.Collections.Generic;
using System.Text;

namespace YQLinq
{
	public class QueryPartsAggregator
	{
		public QueryPartsAggregator ()
		{
			FromParts = new List<string> ();
			WhereParts = new List<string> ();
			OrderByParts = new List<string> ();
		}
		
		public string SelectPart { get; set; }

		List<string> FromParts;

		List<string> WhereParts;

		List<string> OrderByParts;

		public void AddFromPart (IQuerySource querySource)
		{
			FromParts.Add (string.Format ("{0} as {1}", GetEntityName (querySource), querySource.ItemName));
		}
		
		public void AddWherePart (string formatString, params object[] args)
		{
			WhereParts.Add (string.Format (formatString, args));
		}
		
		public void AddOrderByPart (IEnumerable<string> orderings)
		{
			OrderByParts.Insert (0, string.Join (',', orderings));
		}
		
		public string BuildHQLString ()
		{
			var stringBuilder = new StringBuilder ();
			
			if (string.IsNullOrEmpty (SelectPart) || FromParts.IsEmpty ()) {
				throw new InvalidOperationException ("A query must have a select part and at least one from part.");
			}

			stringBuilder.AppendFormat ("select {0}", SelectPart);
			stringBuilder.AppendFormat (" from {0}", string.Join (',', FromParts));
			
			if (WhereParts.IsNotEmpty ()) {
				stringBuilder.AppendFormat (" where {0}", string.Join (',', WhereParts));
			}

			if (OrderByParts.IsNotEmpty ()) {
				stringBuilder.AppendFormat (" order by {0}", string.Join (',', OrderByParts));
			}

			return stringBuilder.ToString ();
		}
		
		string GetEntityName (IQuerySource querySource)
		{
			return NHibernateUtil.Entity (querySource.ItemType).Name;
		}
	}
}
