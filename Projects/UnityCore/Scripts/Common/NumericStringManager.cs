namespace MonMoose.Core
{
    public class NumericStringManager : Singleton<NumericStringManager>
    {
        private const string percentageMark = "%";
        private const string negativeMark = "-";
        private string[] positiveStrs = new string[4096];
        private string[] negativeStrs = new string[4096];
        private string[] percentageStrs = new string[101];

        protected override void OnInit()
        {
            for (int i = 0; i < positiveStrs.Length; ++i)
            {
                string str = i.ToString();
                positiveStrs[i] = str;
                negativeStrs[i] = negativeMark + str;
            }
            for (int i = 0; i < percentageStrs.Length; ++i)
            {
                percentageStrs[i] = positiveStrs[i] + percentageMark;
            }
        }

        public string GetNumber(int num)
        {
            if (num >= positiveStrs.Length || num <= -negativeMark.Length)
            {
                return num.ToString();
            }
            if (num >= 0)
            {
                return positiveStrs[num];
            }
            return negativeStrs[-num];
        }

        public string GetPercentage(float value)
        {
            int num = (int)(value * 100);
            if (num > 100)
            {
                return GetNumber(num) + percentageMark;
            }
            return percentageStrs[num];
        }

    }
}
