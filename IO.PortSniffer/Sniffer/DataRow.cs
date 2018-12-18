using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SnT.IO.Ports.Utils;
using System.Globalization;

namespace SnT.IO.PortSniffer.Sniffer
{
    class DataRow
    {
        public Direction Direction { get; private set; }
        public ByteArray Buffer { get; private set; }

        public string Text { get { return ToString(); } }

        public DataRow(Direction direction)
        {
            Direction = direction;
            Buffer = new ByteArray();
        }

        private static string ToLiteral(ByteArray buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Count; i++)
            {
                sb.AppendFormat(" {0:x2}", buffer[i]); 
            }
            return sb.ToString();
        }

        private static string ToLiteral(string input, Encoding encoding)
        {
            if (String.IsNullOrEmpty(input))
                return input;            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {                
                switch (CharUnicodeInfo.GetUnicodeCategory(input, i))
                {
                    case UnicodeCategory.ClosePunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.ConnectorPunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.Control:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.CurrencySymbol:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.DashPunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.DecimalDigitNumber:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.EnclosingMark:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.FinalQuotePunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.Format:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.InitialQuotePunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.LetterNumber:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.LineSeparator:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.LowercaseLetter:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.MathSymbol:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.ModifierLetter:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);                        
                        break;
                    case UnicodeCategory.ModifierSymbol:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.NonSpacingMark:
                        break;
                    case UnicodeCategory.OpenPunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.OtherLetter:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.OtherNotAssigned:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.OtherNumber:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.OtherPunctuation:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.OtherSymbol:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.ParagraphSeparator:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.PrivateUse:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.SpaceSeparator:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.SpacingCombiningMark:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.Surrogate:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                    case UnicodeCategory.TitlecaseLetter:
                        sb.Append(input, i, 1);
                        break;
                    case UnicodeCategory.UppercaseLetter:
                        sb.Append(input, i, 1);
                        break;
                    default:
                        sb.AppendFormat("\\x{0:x2}", encoding.GetBytes(input.Substring(i, 1))[0]);
                        break;
                } 
            }
            return sb.ToString();
        }                

        public override string ToString()
        {
            return String.Format("{0} {1}",
                Direction == Direction.LeftToRight ? ">" :
                Direction == Direction.RightToLeft ? "<" : "?",
                //ToLiteral(Buffer.GetString(Encoding.Default), Encoding.Default));
                ToLiteral(Buffer));
        }

    }
}
