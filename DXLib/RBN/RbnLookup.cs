using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.RBN
{
    public class RbnLookup
    {
        public static async Task<string> GetRbnQth(string input)
        {
            string[] RbnFile = await File.ReadAllLinesAsync("RBN.csv");
            string? result = RbnFile.Where(x => x.StartsWith(input)).FirstOrDefault();
            if (result == null) { return string.Empty; }
            return result.Split(',')[1];
        }
        public static async Task<RBNNode?> GetRBNNode(string input)
        {
            string[] RbnFile = await File.ReadAllLinesAsync("RBN.csv");
            string? result = RbnFile.Where(x => x.StartsWith(input)).FirstOrDefault();
            if (string.IsNullOrEmpty(result)) return null;

            string[] splitResult = result.Split(",");

            if (splitResult.Count() != 6) return null;

            return new RBNNode()
            {
                Station = splitResult[0],
                MaidenheadLocator = splitResult[1],
                PrimaryPrefix = splitResult[2],
                Continent = splitResult[3],
                CQZone = splitResult[4],
                ITUZone = splitResult[5]
            };
        }
        public static RBNNode? GetRBNNodeSync(string input)
        {
            Task<RBNNode?> t = GetRBNNode(input);
            t.Wait();
            return t.Result;
        }
    }
}
