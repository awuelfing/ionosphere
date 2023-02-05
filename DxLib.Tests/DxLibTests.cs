using DXLib;
using DXLib.CtyDat;
using DXLib.ScpDb;

namespace DxLib.Tests
{
    [TestClass]
    public class DxLibTests
    {
        [TestMethod]
        [DataRow("AJ1AJ")]
        [DataRow("W1AW")]
        public void MasterScp_CheckPresent(string callsign)
        {
            bool result = Scp.MatchScp(callsign);
            Assert.IsTrue(result,$"{callsign} should be present in MasterScp");
        }

        [TestMethod]
        [DataRow("AAAAAA111NOTAMATCH")]
        [DataRow("123123ZZZ")]
        public void MasterScp_CheckAbsent(string callsign)
        {
            bool result = Scp.MatchScp(callsign);
            Assert.IsFalse(result,$"{callsign} should not be present in MasterScp");
        }

        [TestMethod]
        [DataRow("AJ1AJ")]
        [DataRow("W1AW")]
        public void CtyDat_Check_Basic_USA(string callsign)
        {
            const string expectedResult = "United States";
            var match = Cty.MatchCall(callsign);
            var result = match.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} should be US");
        }

        [TestMethod]
        [DataRow("KA7JOR")]
        [DataRow("KC0VDN")]
        public void CtyDat_Check_USA_Out_of_Area(string callsign)
        {
            const string expectedResult = "Alaska";
            var match = Cty.MatchCall(callsign);
            var result = match.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} should be an exact match for an out-of-area callsign");
        }

        [TestMethod]
        [DataRow("GB13COL")]
        [DataRow("M0MCX")]
        public void CtyDat_Check_England(string callsign)
        {
            const string expectedResult = "England";
            var match = Cty.MatchCall(callsign);
            var result = match.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} should be England");
        }
        [TestMethod]
        [DataRow("KG4ZZ")] //not (unnecessarily) referenced exactly in CtyDat
        [DataRow("KG4BP")] // referenced exactly in CtyDat
        public void CtyDat_Check_USA_Significant_Length_Exception_Positive(string callsign)
        {
            const string expectedResult = "Guantanamo Bay";
            var match = Cty.MatchCall(callsign);
            var result = match.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} is KG4 prefix with length 5 and should match KG4 exception");
        }
        [TestMethod]
        [DataRow("KG4WDX")]
        public void CtyDat_Check_USA_Significant_Length_Exception_Negative(string callsign)
        {
            const string expectedResult = "United States";
            var match = Cty.MatchCall(callsign);
            var result = match.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} is a 6 character callsign and should not match the KG4 exception");
        }
    }
}