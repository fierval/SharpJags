using System.Collections.Generic;

namespace SharpJags.DataFormatters
{
	public interface IDataFormatter
	{
		FormattedData Format(Dictionary<string, object> data);
	}
}
