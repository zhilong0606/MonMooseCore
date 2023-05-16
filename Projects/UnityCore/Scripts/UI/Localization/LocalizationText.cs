using System;
using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class LocalizationText : LocalizationWidget
    {
        private Text m_text;
        [SerializeField] protected bool m_autoNewLine = false;
        [SerializeField] protected EValueType m_valueType = EValueType.Id;
        private string[] m_strPrms;
        private int[] m_idPrms;
        private string m_str;

        public Func<string, string> funcOnStrUpdate;

        public string text
        {
            get { return m_text.text; }
            set
            {
                m_valueType = EValueType.Str;
                m_str = value;
                SetText(value);
            }
        }

        public override int id
        {
            get { return base.id; }
            set
            {
                m_valueType = EValueType.Id;
                base.id = value;
            }
        }

        public Color color
        {
            get { return m_text.color; }
            set { m_text.color = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            m_text = GetComponent<Text>();
            UpdateWidget();
        }

        //private string GetLocalization(int id)
        //{
        //    //return LocalizationSystem.instance.GetLocalization(id);
        //}

        public void Format(int textId, params string[] values)
        {
            m_id = textId;
            m_strPrms = values;
            m_valueType = EValueType.IdFormatStrParams;
            UpdateWidget();
        }

        public void Format(int textId, params int[] values)
        {
            m_id = textId;
            m_idPrms = values;
            m_valueType = EValueType.IdFormatIdParams;
            UpdateWidget();
        }

        public void Format(string textStr, params int[] values)
        {
            m_str = textStr;
            m_idPrms = values;
            m_valueType = EValueType.StrFormatIdParams;
            UpdateWidget();
        }

        public void Format(string textStr, params string[] values)
        {
            text = string.Format(textStr, values);
        }

        public override void OnIdUpdate()
        {
            m_valueType = EValueType.Id;
            UpdateWidget();
        }

        public override void OnLanguageUpdate()
        {
            UpdateWidget();
        }

        private void UpdateWidget()
        {
            string str = m_str;
            //if (m_valueType == EValueType.Id)
            //{
            //    str = GetLocalization(m_id);
            //}
            //else if (m_valueType == EValueType.IdFormatIdParams)
            //{
            //    string[] prms = new string[m_idPrms.Length];
            //    for (int i = 0; i < m_idPrms.Length; ++i)
            //    {
            //        prms[i] = LocalizationSystem.instance.GetLocalization(m_idPrms[i]);
            //    }
            //    str = string.Format(GetLocalization(m_id), prms);
            //}
            //else if (m_valueType == EValueType.StrFormatIdParams)
            //{
            //    string[] prms = new string[m_idPrms.Length];
            //    for (int i = 0; i < m_idPrms.Length; ++i)
            //    {
            //        prms[i] = LocalizationSystem.instance.GetLocalization(m_idPrms[i]);
            //    }
            //    str = string.Format(m_str, prms);
            //}
            //else if (m_valueType == EValueType.IdFormatStrParams)
            //{
            //    str = string.Format(GetLocalization(m_id), m_strPrms);
            //}
            //if (funcOnStrUpdate != null)
            //{
            //    str = funcOnStrUpdate(str);
            //}
            SetText(str);
        }

        private void SetText(string str)
        {
            if (m_autoNewLine && str.Contains("\\n"))
            {
                str = str.Replace("\\n", "\n");
            }
            if (m_text != null)
            {
                m_text.text = str;
            }
        }

        public enum EValueType
        {
            Str,
            StrFormatIdParams,
            IdFormatStrParams,
            IdFormatIdParams,
            Id,
        }
    }
}
