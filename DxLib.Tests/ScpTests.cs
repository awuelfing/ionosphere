using DXLib.ScpDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.Tests
{
    [TestClass]
    public class ScpTests
    {
        [TestMethod]
        [DataRow("AJ1AJ")]
        [DataRow("W1AW")]
        public void MasterScp_CheckPresent(string callsign)
        {
            bool result = Scp.MatchScp(callsign);
            Assert.IsTrue(result, $"{callsign} should be present in MasterScp");
        }

        [TestMethod]
        [DataRow("AAAAAA111NOTAMATCH")]
        [DataRow("123123ZZZ")]
        public void MasterScp_CheckAbsent(string callsign)
        {
            bool result = Scp.MatchScp(callsign);
            Assert.IsFalse(result, $"{callsign} should not be present in MasterScp");
        }

    }
}
