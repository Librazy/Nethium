using System.Diagnostics.CodeAnalysis;

namespace Nethium.Demo.Abstraction
{
    public class AggregateResult
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private AggregateResult()
        {
            AddResult = "";
            MulResult = "";
        }

        public AggregateResult(int baseNum, string addResult, string mulResult)
        {
            BaseNum = baseNum;
            AddResult = addResult;
            MulResult = mulResult;
        }

        public int BaseNum { get; set; }

        public string AddResult { get; set; }

        public string MulResult { get; set; }
    }
}