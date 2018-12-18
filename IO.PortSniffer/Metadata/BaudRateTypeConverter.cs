using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SnT.IO.PortSniffer.Metadata
{
    public class BaudRateTypeConverter : TypeConverter
    {
        private static readonly int[] standardBaudRates = new int[] { 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200, 230400 };

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
                if (retVal < 110)
                    throw new ArgumentOutOfRangeException();
                return retVal;
            }
            return base.ConvertFrom(context, culture, value);
        }

    }

}
