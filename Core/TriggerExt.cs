using OneQuick.Core.Conditions;
using OneQuick.Core.Triggers;

namespace OneQuick.Core
{
    internal static class TriggerExt
    {
        public static int Priority(this PositionCodition pos)
        {
            PositionEnum position = pos.Position.Position;
            int num = 0;
            if (position.IncludedBy(PositionEnum.Corner))
            {
                num += 100;
            }
            else if (position.OverlapTo(PositionEnum.Corner))
            {
                num += 60;
            }
            if (position.IncludedBy(PositionEnum.Edge))
            {
                num += 10;
            }
            if (position.OverlapTo(PositionEnum.Edge))
            {
                num += 2;
            }
            int num2 = position.PartsCount();
            return num * (10 - num2);
        }

        public static int Priority(this Condition condition)
        {
            if (condition != null)
            {
                MouseScreenPosCodition mouseScreenPosCodition;
                if ((mouseScreenPosCodition = condition as MouseScreenPosCodition) != null)
                {
                    MouseScreenPosCodition pos = mouseScreenPosCodition;
                    return 5 * pos.Priority();
                }
                MouseAppPosCodition mouseAppPosCodition;
                if ((mouseAppPosCodition = condition as MouseAppPosCodition) != null)
                {
                    MouseAppPosCodition pos2 = mouseAppPosCodition;
                    return 3 * pos2.Priority();
                }
            }
            return 0;
        }

        public static int Priority(this Trigger trigger)
        {
            if (trigger.Condition != null)
            {
                return trigger.Condition.Priority();
            }
            return 0;
        }
    }
}
