using System.Text.RegularExpressions;

namespace DXLib.CtyDat
{
    public class Cty
    {
        private static readonly string CtyDatInput;
        private static readonly string DXCCExtraction = @"^(?<Name>.*?)\:\s+(?<CQZone>.\d{1,2})\:\s+(?<ITUZone>\d{1,2}):\s+(?<Continent>\w{1,2}):\s+(?<Lat>.*?):\s+(?<Long>.*?):\s+(?<Offset>.*?):\s+(?<Primary>.*?):(?<Prefixes>.*?);";
        private static readonly string prefixExtraction = @"^(?<Exact>=){0,1}(?<Prefix>[a-zA-Z0-9\/]+)\({0,1}(?<CQ>\d){0,1}\){0,1}\){0,1}\[{0,1}(?<ITU>\d+){0,1}\]{0,1}\]{0,1}";

        static Cty()
        {
            CtyDatInput = File.ReadAllText(AppContext.BaseDirectory + "cty.dat");
        }

        /*todo (not comprehensive:
         * US/Canada reciprocity format
         * US stations operating domestically from other DXCC entities
         * basically, the minority of situations where the stroke appears after the
         * callsign AND we actually care about it
         * I guess maybe some additional logic could be done to better derive the
         * ITU zone and CQ zone from post-slashed callsigns as well, probably
         * useful in contests
         * */
        public static CtyResult? MatchCall(String callsign)
        {
            string matchedPrefix = string.Empty;

            CtyResult ctyResult = new CtyResult();
            ctyResult.Callsign = callsign;

            MatchCollection DXCCMatchCollection = Regex.Matches(CtyDatInput, DXCCExtraction, RegexOptions.Multiline | RegexOptions.Singleline);
            foreach (Match DXCCMatch in DXCCMatchCollection)
            {
                foreach (String prefixBlob in DXCCMatch.Groups["Prefixes"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    foreach (Match prefixMatch in Regex.Matches(prefixBlob, prefixExtraction, RegexOptions.Multiline))
                    {
                        if (prefixMatch.Groups["Exact"].Success)
                        {
                            if (callsign.Equals(prefixMatch.Groups["Prefix"].Value))
                            {
                                matchedPrefix = prefixMatch.Groups["Prefix"].Value;

                                ctyResult.ExactMatch = true;
                                ctyResult.DXCCEntityName = DXCCMatch.Groups["Name"].Value;
                                ctyResult.CQZone = prefixMatch.Groups["CQ"].Success ? prefixMatch.Groups["CQ"].Value : DXCCMatch.Groups["CQZone"].Value;
                                ctyResult.ITUZone = prefixMatch.Groups["ITU"].Success ? prefixMatch.Groups["ITU"].Value : DXCCMatch.Groups["ITUZone"].Value;
                                ctyResult.Continent = DXCCMatch.Groups["Continent"].Value;
                                ctyResult.Lat = DXCCMatch.Groups["Lat"].Value;
                                ctyResult.Long = DXCCMatch.Groups["Long"].Value;
                                ctyResult.TZOffset = DXCCMatch.Groups["Offset"].Value;
                                ctyResult.PrimaryPrefix = DXCCMatch.Groups["Primary"].Value;

                                //per spec, not an an optimization although i guess it is
                                return(ctyResult);
                            }
                        }
                        else
                        {
                            if (callsign.StartsWith(prefixMatch.Groups["Prefix"].Value))
                            {
                                if (prefixMatch.Groups["Prefix"].Value == "KG4" && callsign.Length != 5) continue;

                                if (prefixMatch.Groups["Prefix"].Value.Length > matchedPrefix.Length)
                                {
                                    matchedPrefix = prefixMatch.Groups["Prefix"].Value;

                                    ctyResult.ExactMatch = false;
                                    ctyResult.DXCCEntityName = DXCCMatch.Groups["Name"].Value;
                                    ctyResult.CQZone = prefixMatch.Groups["CQ"].Success ? prefixMatch.Groups["CQ"].Value : DXCCMatch.Groups["CQZone"].Value;
                                    ctyResult.ITUZone = prefixMatch.Groups["ITU"].Success ? prefixMatch.Groups["ITU"].Value : DXCCMatch.Groups["ITUZone"].Value;
                                    ctyResult.Continent = DXCCMatch.Groups["Continent"].Value;
                                    ctyResult.Lat = DXCCMatch.Groups["Lat"].Value;
                                    ctyResult.Long = DXCCMatch.Groups["Long"].Value;
                                    ctyResult.TZOffset = DXCCMatch.Groups["Offset"].Value;
                                    ctyResult.PrimaryPrefix = DXCCMatch.Groups["Primary"].Value;

                                }
                            }
                        }
                    }
                }
            }

            if (matchedPrefix == string.Empty) return null;
            return(ctyResult);
        }
        public static string FormatResult(CtyResult input)
        {
            return $"Callsign:\t\t\t{input.Callsign}" + Environment.NewLine +
                $"\tCountry:\t\t{input.DXCCEntityName}" + Environment.NewLine +
                $"\tCQ Zone:\t\t{input.CQZone}" + Environment.NewLine +
                $"\tITU Zone:\t\t{input.ITUZone}" + Environment.NewLine +
                $"\tContinent:\t\t{input.Continent}" + Environment.NewLine +
                $"\tLatitude:\t\t{input.Lat}" + Environment.NewLine +
                $"\tLongitude:\t\t{input.Long}" + Environment.NewLine +
                $"\tTZ Offset:\t\t{input.TZOffset}" + Environment.NewLine +
                $"\tPrimary Prefix:\t\t{input.PrimaryPrefix}" + Environment.NewLine + Environment.NewLine;
        }
    }
}