using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SnT.IO.PortSniffer.Metadata
{
    public class PortNameTypeConverter : TypeConverter
    {
        private static List<string> values = null;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                try
                {
                    values = new List<string>(Ports.Utils.PortHelper.GetPortNames());
                    values.Sort(new Helpers.NaturalComparer());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return new StandardValuesCollection(values);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if ((value != null) && (value.GetType() == typeof(string)))
            {
                return value.ToString();
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

}
