using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public class PrintMapper<TSource, TDestination>
        where TDestination : new()
    {
        private readonly Dictionary<string, Func<TSource, object>> _customMappings =
            new Dictionary<string, Func<TSource, object>>();

        public PrintMapper<TSource, TDestination> ForMember(string destinationProperty, Func<TSource, object> mapFunc)
        {
            _customMappings[destinationProperty] = mapFunc;
            return this;
        }

        public TDestination Map(TSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var destination = new TDestination();
            var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var srcProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var destProp in destProps)
            {
                if (!destProp.CanWrite)
                    continue;

                // If custom rule exists
                if (_customMappings.ContainsKey(destProp.Name))
                {
                    destProp.SetValue(destination, _customMappings[destProp.Name](source), null);
                    continue;
                }

                // Match by name and type
                var srcProp = srcProps.FirstOrDefault(p =>
                    p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase) &&
                    p.PropertyType == destProp.PropertyType);

                if (srcProp != null && srcProp.CanRead)
                {
                    var value = srcProp.GetValue(source, null);
                    destProp.SetValue(destination, value, null);
                }
            }

            return destination;
        }
    }

    public static class SimpleMap
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            var mapper = new PrintMapper<TSource, TDestination>();
            return mapper.Map(source);
        }
    }
}
