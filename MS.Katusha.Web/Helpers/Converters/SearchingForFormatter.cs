using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Web.Helpers.Converters
{
    public class SearchingForFormatter : IValueFormatter
    {
        public string FormatValue(ResolutionContext context)
        {
            var items = context.DestinationValue as IList<SearchingFor>;
            if (items != null) {
                var list = new List<string>(items.Count);
                list.AddRange(items.Select(item => item.Search.ToString(CultureInfo.InvariantCulture)));
                return string.Join("", list);
            }
            return "";
        }
    }
}