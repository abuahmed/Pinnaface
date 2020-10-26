using System.Windows;

namespace PinnaFace.WPF.Models
{
    public class ConditionalStyleRule
    {
        private object _Value;

        public object Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }

        private Style _Style;

        public Style Style
        {
            get { return this._Style; }
            set { this._Style = value; }
        }
    }
}