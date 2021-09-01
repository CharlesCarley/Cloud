/*
-------------------------------------------------------------------------------
    Copyright (c) Charles Carley.

  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
-------------------------------------------------------------------------------
*/

using System;
using Cloud.Common;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloud.Utils.Test
{
    [TestClass]
    public class StringUtilsUnit {

        private static void LogMessage(string message)
        {
            ConsoleOutput.Instance.WriteLine(message, OutputLevel.Information);
        }

        [TestMethod]
        public void IsHexadecimal()
        {
            try {
                Assert.IsTrue(StringUtils.IsHex("0123456789ABCDEF"));
                Assert.IsTrue(StringUtils.IsHex("0123456789abcdef"));
                Assert.IsFalse(StringUtils.IsHex("0123456789abcdefg"));
            } catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsGuid()
        {
            try {
                Assert.IsTrue(StringUtils.IsGuid("107D5005-5CD0-413F-996D-391BAD9801FB"));
                Assert.IsTrue(StringUtils.IsGuid("9eb6446e-9828-4d6b-8ea7-328eceece916"));
                Assert.IsFalse(StringUtils.IsGuid("0123456789abcdefg"));
                Assert.IsFalse(StringUtils.IsGuid(string.Empty));
                Assert.IsFalse(StringUtils.IsGuid(null));
                Assert.IsFalse(StringUtils.IsGuid("107D5005-5CD0-413F-996D-391-BAD98-01FB"));
                Assert.IsFalse(StringUtils.IsGuid("107D5005-5CD0-413F-996D-391BAD9801FB';"));
            } catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsValidDatabaseValue()
        {
            try {
                Assert.IsTrue(StringUtils.IsValidDatabaseValue("abcdefghijklmnopqrstuvwxyz"));
                Assert.IsTrue(StringUtils.IsValidDatabaseValue("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                Assert.IsTrue(StringUtils.IsValidDatabaseValue("0123456789"));
                Assert.IsTrue(StringUtils.IsValidDatabaseValue("=/+.-"));
                Assert.IsFalse(StringUtils.IsValidDatabaseValue("' \n\t\r\"`;)(><]["));
            } catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IisValidIpOrDomain()
        {
            try {
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("0"));
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("0.0"));
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("0.0.0"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("0.0.0.0"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("www.example.com"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("rhino.acme.com"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("x.y.z.w.net"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("host.csci-470"));
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("-rhino.acme.com"));
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("rhino.acme-.com"));
                Assert.IsTrue(StringUtils.IsIpAddressOrDomain("host-csci-470"));
                Assert.IsFalse(StringUtils.IsIpAddressOrDomain("1.host.csci"));
            }

            catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ConvertToHex()
        {
            try {
                const string str = "Hello World";

                var result = StringUtils.ToHexString(str);
                Assert.AreEqual("48656C6C6F20576F726C64", result);

                var result2 = StringUtils.FromHexString("48656C6C6F20576F726C64");
                Assert.AreEqual(str, result2);

            } catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ConvertToSha()
        {
            try {
                const string str = "Hello World";

                var result = ShaHashString.Get(str);

                Assert.AreEqual(
                    "2C74FD17EDAFD80E8447B0D46741EE243B7EB74DD2149A0AB1B9246FB30382F27E853D8585719E0E67CBDA0DAA8F51671064615D645AE27ACB15BFB1447F459B",
                    result);
                var result2 = ShaHashString.GetSha1(str);
                Assert.AreEqual(
                    "0A4D55A8D778E5022FAB701977C5D840BBC486D0",
                    result2);
            } catch (Exception ex) {
                LogMessage(ex.Message);
                Assert.Fail();
            }
        }
    }
}
