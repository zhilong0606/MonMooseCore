using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class UICellSelectable : UICell, IUICellSelectable
    {
        private GameObject m_onObj;
        private GameObject m_offObj;
        private bool m_isOn;

        public bool IsSelected
        {
            get { return m_isOn; }
            set
            {
                m_isOn = value;
                m_onObj.SetActiveSafely(value);
                m_offObj.SetActiveSafely(!value);
            }
        }

        //protected override void OnInit(object param)
        //{
        //    m_onObj = GetInventory().Get((int)EWidget.On);
        //    m_offObj = GetInventory().Get((int)EWidget.Off);
        //    Button btn = GetComponent<Button>();
        //    btn.onClick.AddListener(OnBtnClicked);
        //}

        private void OnBtnClicked()
        {
            Select();
        }

        private enum EWidget
        {
            Off,
            On,
        }
    }
}
