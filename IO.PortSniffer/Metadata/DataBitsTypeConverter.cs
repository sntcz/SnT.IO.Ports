using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SnT.IO.PortSniffer.Metadata
{
    public class DataBitsTypeConverter : TypeConverter
    {
        private static readonly int[] standardBaudRates = new int[] { 5, 6, 7, 8 };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(standardBaudRates);
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
                int retVal = Int32.Parse((string)value);
                if ((retVal < 5) || (retVal > 8))
                    throw new ArgumentOutOfRangeException();
                return retVal;
            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}
