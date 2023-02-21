using DXLib;
using DXLib.CtyDat;
using DXLib.ScpDb;

namespace DxLib.Tests
{
    [TestClass]
    public class CtyDatTests
    {
        [TestMethod]
        [DataRow("AJ1AJ", "United States")]
        [DataRow("W1AW", "United States")]
        [DataRow("GB13COL", "England")]
        [DataRow("M0MCX", "England")]
        public void CtyDat_Check_Basic(string callsign,string expected)
        {
            CtyResult? match = Cty.MatchCall(callsign);
            string result = match!.DXCCEntityName;
            Assert.AreEqual(result, expected, $"{callsign} should be {expected}");
        }

        [TestMethod]
        [DataRow("KA7JOR")]
        [DataRow("KC0VDN")]
        public void CtyDat_Check_USA_Out_of_Area(string callsign)
        {
            const string expectedResult = "Alaska";
            CtyResult? match = Cty.MatchCall(callsign);
            var result = match!.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} should be an exact match for an out-of-area callsign");
        }

        [TestMethod]
        [DataRow("KG4ZZ")] //not (unnecessarily) referenced exactly in CtyDat
        [DataRow("KG4BP")] // referenced exactly in CtyDat
        public void CtyDat_Check_USA_Significant_Length_Exception_Positive(string callsign)
        {
            const string expectedResult = "Guantanamo Bay";
            CtyResult? match = Cty.MatchCall(callsign);
            var result = match!.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} is KG4 prefix with length 5 and should match KG4 exception");
        }
        [TestMethod]
        [DataRow("KG4WDX")]
        public void CtyDat_Check_USA_Significant_Length_Exception_Negative(string callsign)
        {
            const string expectedResult = "United States";
            CtyResult? match = Cty.MatchCall(callsign);
            var result = match!.DXCCEntityName;
            Assert.AreEqual(result, expectedResult,$"{callsign} is a 6 character callsign and should not match the KG4 exception");
        }
    }
}