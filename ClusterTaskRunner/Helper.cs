using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    internal class Helper
    {
        public static (DateTime Start,DateTime End) CalcCurrentWindowUtc(int seconds)
        {
            DateTime now = DateTime.UtcNow;
            int secondsFromHourBottom = (now.Minute * 60) + now.Second;
            int startPoint = secondsFromHourBottom / seconds;

            DateTime WindowStart = now.AddSeconds(-secondsFromHourBottom).AddSeconds(startPoint * seconds);
            DateTime WindowEnd = now.AddSeconds(-secondsFromHourBottom).AddSeconds((startPoint + 1) * seconds);

            return (WindowStart, WindowEnd);
        }
    }
}
