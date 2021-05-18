using System;
using System.Collections.Generic;

namespace OneQuick.Core.Conditions
{
    public enum Operator
    {
        And,
        Or
    }

    public class ConditionList : Condition
    {
        public Operator Operator { get; set; }

        public List<Condition> Conditions { get; set; }

        protected override bool IsFit()
        {
            if (Operator == Operator.And)
            {
                using (List<Condition>.Enumerator enumerator = Conditions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.Fit())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (Operator == Operator.Or)
            {
                using (List<Condition>.Enumerator enumerator = Conditions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Fit())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            throw new Exception();
        }

        public override string ToString()
        {
            return string.Join(Operator.ToString(), Conditions);
        }
    }
}
