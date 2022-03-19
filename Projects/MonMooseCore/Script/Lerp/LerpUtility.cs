using System.Collections.Generic;

namespace MonMoose.Core
{
    public enum EBaseLerpFuncType
    {
        Linear,
        Sin,
        Square,
    }

    public static class LerpUtility
    {
        private static Dictionary<int, ILerpFunc> m_baseFuncMap = new Dictionary<int, ILerpFunc>();
        private static Dictionary<int, ILerpFunc> m_customFuncMap = new Dictionary<int, ILerpFunc>();

        static LerpUtility()
        {
            m_baseFuncMap.Add((int)EBaseLerpFuncType.Linear, new LinearLerpFunc());
            m_baseFuncMap.Add((int)EBaseLerpFuncType.Sin, new SinLerpFunc());
            m_baseFuncMap.Add((int)EBaseLerpFuncType.Square, new SquareLerpFunc());
        }

        public static ILerpFunc GetBaseFunc(EBaseLerpFuncType funcType)
        {
            return m_baseFuncMap.GetClassValue((int)funcType);
        }

        public static void AddCustomFunc(int id, ILerpFunc func)
        {
            if (!m_customFuncMap.ContainsKey(id))
            {
                m_customFuncMap.Add(id, func);
            }
        }

        public static ILerpFunc GetCustomFunc(int id)
        {
            return m_customFuncMap.GetClassValue(id);
        }
    }
}