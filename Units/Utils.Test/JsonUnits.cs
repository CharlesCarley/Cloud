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
    public class JsonUnits {
        static void LogMessage(string message)
        {
            ConsoleOutput.Instance.WriteLine(message, OutputLevel.Information);
        }

        public string TestDir = $"{Environment.CurrentDirectory}/../../../";

        [TestMethod]
        public void LoadNonExistentFile()
        {
            var jObject = JsonObject.ParseFile($"{TestDir}tests/fooFile.json");
            Assert.IsNotNull(jObject);
            Assert.AreEqual(0, jObject.Nodes.Count);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var jObject = JsonObject.ParseFile($"{TestDir}tests/array.json");
            Assert.IsNotNull(jObject);
            Assert.IsTrue(jObject.HasKey("objects"));
            var t1 = jObject.AsObject("objects");
            Assert.IsNotNull(t1);
            Assert.IsTrue(t1.IsArray);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/blank.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/double.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/false.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.TryParseFile($"{TestDir}tests/incomplete.json");
            Assert.IsNull(jObject);

            jObject = JsonObject.ParseFile($"{TestDir}tests/integer.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.TryParseFile($"{TestDir}tests/malFormed.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/null.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/string.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/string_array.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());

            jObject = JsonObject.ParseFile($"{TestDir}tests/true.json");
            Assert.IsNotNull(jObject);
            LogMessage(jObject.AsCompactPrint());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var obj = JsonObject.ParseFile($"{TestDir}tests/array.json");
            Assert.IsNotNull(obj);

            LogMessage(obj.AsPrettyPrint());
            LogMessage(obj.AsCompactPrint());
        }

        [TestMethod]
        public void TestMethod3()
        {
            var obj = JsonObject.ParseFile($"{TestDir}tests/AllTypes.json");
            Assert.IsNotNull(obj);

            LogMessage(obj.AsPrettyPrint());
            LogMessage(obj.AsCompactPrint());
        }
        [TestMethod]
        public void TestMethod4()
        {
            var obj = JsonObject.ParseFile($"{TestDir}tests/multiValue.json");
            Assert.IsNotNull(obj);

            LogMessage(obj.AsPrettyPrint());
            LogMessage(obj.AsCompactPrint());
        }

        [TestMethod]
        public void TestDocumentationCode()
        {
            var obj = JsonObject.ParseFile($"{TestDir}tests/test.json");
            Assert.IsNotNull(obj);

            var message = obj.AsString("Message");
            Assert.AreEqual("Hello world", message);

            var integer = obj.AsInt("Integer");
            Assert.AreEqual(123, integer);

            var real = obj.AsDouble("Real");
            Assert.AreEqual(1.56789, real);

            var booleanTrue = obj.AsBool("BoolTrue");
            Assert.AreEqual(true, booleanTrue);

            var booleanFalse = obj.AsBool("BoolFalse");
            Assert.AreEqual(false, booleanFalse);
        }

        private class TestWrapObject {
            public string A { get; set; }
            public int    B { get; set; }
            public float  C { get; set; }
        }

        [TestMethod]
        public void TestWrap()
        {
            var obj = new TestWrapObject {
                A = "SomeString",
                B = 7765432,
                C = 1.1457890f
            };

            var str = JsonParser.Wrap(obj);
            LogMessage(str);

            var obj1 = (TestWrapObject)JsonParser.Unwrap(str, typeof(TestWrapObject), false);
            Assert.AreEqual("SomeString", obj1.A);
            Assert.AreEqual(7765432, obj1.B);
            Assert.IsTrue(Math.Abs(1.1457890f - obj.C) < 0.0001f);
        }


    }
}
