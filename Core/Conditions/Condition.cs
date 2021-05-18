using System.Xml.Serialization;

namespace OneQuick.Core.Conditions
{
    [XmlInclude(typeof(ConditionList))]
    [XmlInclude(typeof(MouseAppPosCodition))]
    [XmlInclude(typeof(MouseScreenPosCodition))]
    [XmlInclude(typeof(ProgramCodition))]
    public abstract class Condition
    {
        public bool Reverse { get; set; }

        public bool Fit()
        {
            if (Reverse)
            {
                return !IsFit();
            }
            return IsFit();
        }

        protected abstract bool IsFit();
    }
}
