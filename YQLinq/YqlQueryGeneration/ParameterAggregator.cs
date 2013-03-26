using System.Collections.Generic;

namespace YQLinq
{
	public class ParameterAggregator
	{
		readonly List<NamedParameter> parameters = new List<NamedParameter> ();

		public NamedParameter AddParameter (object value)
		{
			var parameter = new NamedParameter ("p" + (parameters.Count + 1), value);
			parameters.Add (parameter);
			return parameter;
		}

		public NamedParameter[] GetParameters ()
		{
			return parameters.ToArray ();
		}
	}
}