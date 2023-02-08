using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DXLib.RBN;

namespace DxLib.Tests
{
    [TestClass]
    public class RbnTests
    {
        [TestMethod]
        [DataRow("KM3T", "FN42ET")]
        [DataRow("DL1REM", "JO32PC")]
        [DataRow("W1NT-6", "FN42LU")]
        public async Task RbnLookup_Test_Valid_Node(string input,string expected)
        {
            var result = await RbnLookup.GetRbnQth(input);
            Assert.AreEqual(expected, result);
        }
    }
}
