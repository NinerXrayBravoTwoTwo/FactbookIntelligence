using System;
using System.Diagnostics.CodeAnalysis;
using iTextSharp.text;

namespace MergePowerData.IntelMath
{
    /// <summary>
    /// 
    /// I am a value-alignment class
    /// o I help align cells in tables for PDF document generation in iTextSharp.text 
    /// o I contain a string value for a cell
    /// o I contain a text justification value (left|center|right)
    /// o I contain a border set value for this instance (top|right|bottom|left)
    /// 
    /// I'm useful in defining elements of a table where the border and cells are aligned according to their position in the list.
    /// 
    /// </summary>
    public class Valign
    {
        [Flags]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum BorderFlg : short
        {
            none = 0,
            T = 1 << 0,
            R = 1 << 1,
            B = 1 << 2,
            L = 1 << 3,
            BOLD = 1 << 4,
            MARK = 1 << 5,

        }

        public enum LeftRight { L, R, C }

        public Valign(BorderFlg border, int? colspan = null)
        {
            Border = border;
            ColSpan = colspan ?? 1;
            Align = LeftRight.L;
            Value = string.Empty;
        }

        public Valign(string value, BorderFlg border, int? colspan = null)
        {
            Border = border;

            ColSpan = colspan ?? 1;
            Align = LeftRight.L;
            Value = value;
        }

        public Valign(LeftRight align, string value, BorderFlg border, int? colspan = null)
        {
            Border = border;

            ColSpan = colspan ?? 1;
            Align = align;
            Value = value;
        }

        public BorderFlg Border { get; set; }

        public int ColSpan { get; set; }

        public LeftRight Align { get; set; }
        public int ElementAlign
        {
            get
            {
                var result = Element.ALIGN_LEFT;

                switch (Align)
                {
                    case LeftRight.L:
                        result = Element.ALIGN_LEFT;
                        break;
                    case LeftRight.R:
                        result = Element.ALIGN_RIGHT;
                        break;
                    case LeftRight.C:
                        result = Element.ALIGN_CENTER;
                        break;
                }
                return result;
            }
        }
        public string Value { get; set; }

        public bool IsTop => (Border & BorderFlg.T) == BorderFlg.T;
        public bool IsRight => (Border & BorderFlg.R) == BorderFlg.R;
        public bool IsBot => (Border & BorderFlg.B) == BorderFlg.B;
        public bool IsLeft => (Border & BorderFlg.L) == BorderFlg.L;
        public bool IsBold => (Border & BorderFlg.BOLD) == BorderFlg.BOLD;
        public bool IsMarked => (Border & BorderFlg.MARK) == BorderFlg.MARK;
    }
}