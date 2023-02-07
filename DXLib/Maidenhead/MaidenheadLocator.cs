using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXLib.Maidenhead
{
    public class MaidenheadLocator
    {
        public static readonly string MaidenheadValidationRegex = @"[A-R]{2}\d{2}([A-X]{2})?(\d{2})?$";
        //apparently both Perl and Python have a function of this name
        public static int Ord(char input)
        {
            return (int)input;
        }
        public static bool ValidateMaidenhead(string input)
        {
            var result = Regex.Match(input, MaidenheadValidationRegex);
            return result.Success;
        }
        public static (double latitude, double longitude) FromMaidenhead(string input)
        {
            string ml = input.ToUpper().Trim();
            // todo revisit this way of handling it once i've used it a bit
            if(!ValidateMaidenhead(ml)) throw new Exception("Invalid maidenhead");

            double lon = -180.0;
            double lat = -90.0;

            lon += (Ord(ml[0]) - Ord('A')) * 20;
            lat += (Ord(ml[1]) - Ord('A')) * 10;

            if(ml.Length >= 4)
            {
                lon += int.Parse(ml[2].ToString()) * 2;
                lat += int.Parse(ml[3].ToString()) * 1;
            }
            if(ml.Length >= 6)
            {
                lon += (Ord(ml[4]) - Ord('A')) * 5.0 / 60;
                lat += (Ord(ml[5]) - Ord('A')) * 2.5 / 60;
            }
            if (ml.Length == 8)
            {
                lon += int.Parse(ml[6].ToString()) * 5.0/600;
                lat += int.Parse(ml[7].ToString()) * 2.5/600;
            }

            //up to here is the southwest corner

            if(ml.Length == 2)
            {
                lon += 20 / 2;
                lat += 10 / 2;
            }
            else if(ml.Length == 4)
            {
                lon += 2 / 2;
                lat += 1.0 / 2;
            }
            else if (ml.Length == 6)
            {
                lon += 5.0 / 60 / 2;
                lat += 2.5 / 60 / 2;
            }
            else if (ml.Length == 8)
            {
                lon += 5.0 / 600 / 2;
                lat += 2.5 / 600 / 2;
            }

            //now this is the center of the grid
            return (lat, lon);
        }
    }
}
