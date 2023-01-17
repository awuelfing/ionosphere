using System.Globalization;
using DXLib.CtyDat;

namespace CtyConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("1 to query, 2 to run file, 3 to quit: ");
                switch(Console.ReadLine())
                {
                    case "1":                       
                        Console.Write("Enter Callsign: ");
                        CtyResult ctyResult = Cty.MatchCall(Console.ReadLine() ?? "");
                        Console.WriteLine(Cty.FormatResult(ctyResult));
                        break;
                    case "2":
                        DateTime start = DateTime.Now;
                        int records = 0;
                        var input = File.ReadAllLines("MASTER.scp").Where(x => !x.StartsWith('#'));
                        foreach(string s in input)
                        {
                            records++;
                            try
                            {
                                Cty.MatchCall(s);
                            }
                            catch
                            {
                                Console.WriteLine($"Kaboom: {s}");
                            }
                            if(records%1000==0)
                            {
                                Console.WriteLine($"Processed {records} - {(DateTime.Now - start).ToString()} elapsed ({records/ (DateTime.Now - start).TotalSeconds} records per second).");
                            }
                        }
                        Console.WriteLine($"Completed in {(DateTime.Now - start).ToString()}");
                        break;
                    default:
                    case "3":
                        return;
                }
            }
        }
    }
}