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
    }
}
