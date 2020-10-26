using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace PinnaFace.WPF.Models
{
    public class ConditionalStyleSelector : StyleSelector
    {
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            object conditionValue = this.ConditionConverter.Convert(item, null, null, null);
            foreach (ConditionalStyleRule rule in this.Rules)
            {
                if (Equals(rule.Value, conditionValue))
                {
                    return rule.Style;
                }
            }

            return base.SelectStyle(item, container);
        }

        private List<ConditionalStyleRule> _Rules;

        public List<ConditionalStyleRule> Rules
        {
            get
            {
                if (this._Rules == null)
                {
                    this._Rules = new List<ConditionalStyleRule>();
                }

                return this._Rules;
            }
        }

        private IValueConverter _ConditionConverter;

        public IValueConverter ConditionConverter
        {
            get { return this._ConditionConverter; }
            set { this._ConditionConverter = value; }
        }
    }
}