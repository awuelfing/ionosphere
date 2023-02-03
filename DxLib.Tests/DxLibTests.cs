using DXLib;
using DXLib.ScpDb;

namespace DxLib.Tests
{
    [TestClass]
    public class DxLibTests
    {
        [TestMethod]
        public void MasterScp_CheckPresent()
        {
            string callsign = "AJ1AJ";
            bool result = Scp.MatchScp(callsign);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MasterScp_CheckAbsent()
        {
            string callsign = "AAAA1AAAZZZ";
            bool result = Scp.MatchScp(callsign);
            Assert.IsFalse(result);
        }
    }
}