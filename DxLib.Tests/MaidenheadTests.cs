using DXLib.CtyDat;
using DXLib.Maidenhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.Tests
{
    [TestClass]
    public class MaidenheadTests
    {
        [TestMethod]
        [DataRow("FN42")]
        public void Maidenhead_Check_From_Valid_Four(string input)
        {
            var expectedResult = (42.5, -71);
            var result = MaidenheadLocator.FromMaidenhead(input);
            Assert.AreEqual(expectedResult, result);
        }
        [TestMethod]
        [DataRow("FN42ew")]
        public void Maidenhead_Check_From_Valid_Six(string input)
        {
            var expectedResult = (42.9375, -71.625);
            var result = MaidenheadLocator.FromMaidenhead(input);
            Assert.AreEqual(expectedResult, result);
        }
        [TestMethod]
        [DataRow("FN42ew33")]
        public void Maidenhead_Check_From_Valid_Eight(string input)
        {
            var expectedResult = (42.93125, -71.6375);
            var result = MaidenheadLocator.FromMaidenhead(input);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("ASD")]
        [DataRow("FN42E")]
        [DataRow("FN42E433")]
        [DataRow("FN42EW3")]
        [DataRow("VN42EW")]
        public void Maidenhead_Check_From_Invalid(string input)
        {
            //awkward but AAA
            Exception? e = null;
            try
            {
                MaidenheadLocator.FromMaidenhead(input);
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Assert.IsNotNull(e);
        }

    }
}

