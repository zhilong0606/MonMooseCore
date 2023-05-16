using UnityEngine;

namespace MonMoose.Core
{
    public class LocalizationWidget : MonoBehaviour
    {
        [SerializeField] protected int m_id;
        protected int m_language;

        public int language
        {
            set
            {
                if (value != m_language)
                {
                    m_language = value;
                    OnLanguageUpdate();
                }
            }
        }

        public virtual int id
        {
            get { return m_id; }
            set
            {
                if (value != m_id)
                {
                    m_id = value;
                    OnIdUpdate();
                }
            }
        }

        protected virtual void Awake()
        {
            //SettingManager.instance.RegisterListener(ESettingKey.Language, OnSettingLanguageChanged);
        }

        protected virtual void OnDestroy()
        {
            //SettingManager.instance.UnRegisterListener(ESettingKey.Language, OnSettingLanguageChanged);
        }

        private void OnSettingLanguageChanged()
        {
            //language = (ELanguage)SettingManager.instance.GetEnumValue(ESettingKey.Language);
        }

        public virtual void OnLanguageUpdate()
        {

        }

        public virtual void OnIdUpdate()
        {

        }
    }
}
