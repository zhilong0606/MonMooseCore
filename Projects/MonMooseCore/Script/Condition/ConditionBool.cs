using System;

namespace MonMoose.Core
{
    public class ConditionBool : ConditionBase
    {
        public Action actionOnValueChanged;

        protected bool m_boolValue;

        public void SetValue(bool value, bool isSilent = false)
        {
            if (value != m_boolValue)
            {
                m_boolValue = value;
                if (!isSilent)
                {
                    actionOnValueChanged();
                }
            }
        }

        public override bool Check()
        {
            return m_boolValue;
        }
    }
}