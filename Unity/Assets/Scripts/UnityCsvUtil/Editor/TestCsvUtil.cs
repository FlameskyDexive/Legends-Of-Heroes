using NUnit.Framework;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sinbad {

	public class TestCsv {

		public class TestObject {
			public string StringField;
			public int IntField;
			public float FloatField;
			public enum Colour {
				Red = 1,
				Green = 2,
				Blue = 3,
				Purple = 15
			}
			public Colour EnumField;

			public TestObject() {}
			public TestObject(string s, int i, float f, Colour c) {
				StringField = s;
				IntField = i;
				FloatField = f;
				EnumField = c;
			}
		}

        public class TestPropertiesObject {
			public string StringProperty { get; private set; }
			public int IntProperty { get; private set; }
            public float FloatProperty { get; private set; }
            public enum Colour {
                Red = 1,
                Green = 2,
                Blue = 3,
                Purple = 15
            }
            public Colour EnumProperty { get; private set; }

            public TestPropertiesObject() {}
			public TestPropertiesObject(string s, int i, float f, Colour c) {
				StringProperty = s;
				IntProperty = i;
				FloatProperty = f;
                EnumProperty = c;
            }
		}

        public class TestPropertiesAndFieldsObject {
            public string StringMember { get; protected set; }
            private string stringMember;
            public string GetStringField() { return stringMember; }

            public int IntMember { get; set; }
            protected int intMember;
            public int GetIntField() { return intMember; }

            public float FloatMember { get; private set; }
            public float floatMember;
            public float GetFloatField() { return floatMember; }

            public enum Colour {
                Red = 1,
                Green = 2,
                Blue = 3,
                Purple = 15
            }
            public Colour EnumMember { get; private set; }
            private Colour enumMember;
            public Colour GetEnumField() { return enumMember; }

            public TestPropertiesAndFieldsObject() { }
            public TestPropertiesAndFieldsObject(string s, int i, float f, Colour c) {
                StringMember = stringMember = s;
                IntMember = intMember = i;
                FloatMember = floatMember = f;
                EnumMember = enumMember = c;
            }
        }


        [Test]
		public void TestLoadSingle() {
			// It's ok to have newline with padding at start, should trim field names (not values)
			// Include spaces in string field value to prove all content preserved
			// Also put fields out of order
			string csvData = @"StringField, Hello World  ,This is an ignored description,also ignored
			EnumField,Blue,Something Something
			IntField,1234,Comment here
			FloatField,1.5,More commenting";

			TestObject t = new TestObject();

			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
				using (var sr = new StreamReader(ms)) {
					CsvUtil.LoadObject(sr, ref t);
				}
			}

			Assert.AreEqual(" Hello World  ", t.StringField);
			Assert.AreEqual(1234, t.IntField);
			Assert.That(t.FloatField, Is.InRange(1.4999f, 1.5001f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Blue, t.EnumField);
		}

		[Test]
		public void TestLoadSingleWithHeader() {
			// Test that we can include a header line if we want
			string csvData = @"#Field,#Value,#Description
			StringField, Hello World  ,This is an ignored description,also ignored
			EnumField,Blue,Something Something
			IntField,1234,Comment here
			FloatField,1.5,More commenting";

			TestObject t = new TestObject();

			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
				using (var sr = new StreamReader(ms)) {
					CsvUtil.LoadObject(sr, ref t);
				}
			}

			Assert.AreEqual(" Hello World  ", t.StringField);
			Assert.AreEqual(1234, t.IntField);
			Assert.That(t.FloatField, Is.InRange(1.4999f, 1.5001f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Blue, t.EnumField);
		}

		[Test]
		public void TestLoadSingleEmbeddedCommas() {
			// It's ok to have newline with padding at start, should trim field names (not values)
			// Include spaces in string field value to prove all content preserved
			// Also put fields out of order
			string csvData = @"StringField,""Commas, commas everywhere,abcd"",Ignored,ignored
			EnumField,Purple,Something Something
			IntField,-5002,Comment here
			FloatField,-3.142,Pi Pi Baby";

			TestObject t = new TestObject();

			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
				using (var sr = new StreamReader(ms)) {
					CsvUtil.LoadObject(sr, ref t);
				}
			}

			Assert.AreEqual("Commas, commas everywhere,abcd", t.StringField);
			Assert.AreEqual(-5002, t.IntField);
			Assert.That(t.FloatField, Is.InRange(-3.142001f, -3.141999f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Purple, t.EnumField);
		}

        [Test]
        public void TestLoadSingleWithSpacesInHeader() {
            // It's ok to have newline with padding at start, should trim field names (not values)
            // Include spaces in string field value to prove all content preserved
            string csvData = @"StringProperty, Hello World  ,This is an ignored description,also ignored
			  Enum Property,Blue,Something Something
			 Int  Property  ,1234,Comment here
			FloatProperty  ,1.5,More commenting";

            TestPropertiesObject t = new TestPropertiesObject();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
                using (var sr = new StreamReader(ms)) {
                    CsvUtil.LoadObject(sr, ref t);
                }
            }

            Assert.AreEqual(" Hello World  ", t.StringProperty);
            Assert.AreEqual(1234, t.IntProperty);
            Assert.That(t.FloatProperty, Is.InRange(1.4999f, 1.5001f)); // float imprecision
            Assert.AreEqual(TestPropertiesObject.Colour.Blue, t.EnumProperty);
        }

        [Test]
		public void TestLoadMulti() {
			// Header first, then N values
			// #Field headers are ignored
			// This time we don't want any prefixing since not trimmed
			string csvData = @"StringField,FloatField,#Description,IntField,EnumField
""This,has,commas,in it"",2.34,Something ignored,35,Red
Hello World,256.25,""Notes here"",10003,Purple
Zaphod Beeblebrox,3.1,""Amazingly amazing"",000359,Green";

			List<TestObject> objs;
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
				using (var sr = new StreamReader(ms)) {
					objs = CsvUtil.LoadObjects<TestObject>(sr);
				}
			}

			Assert.That(objs, Has.Count.EqualTo(3));
			TestObject t = objs[0];
			Assert.AreEqual("This,has,commas,in it", t.StringField);
			Assert.AreEqual(35, t.IntField);
			Assert.That(t.FloatField, Is.InRange(2.33999f, 2.340001f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Red, t.EnumField);
			t = objs[1];
			Assert.AreEqual("Hello World", t.StringField);
			Assert.AreEqual(10003, t.IntField);
			Assert.That(t.FloatField, Is.InRange(256.24999f, 256.25001f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Purple, t.EnumField);
			t = objs[2];
			Assert.AreEqual("Zaphod Beeblebrox", t.StringField);
			Assert.AreEqual(359, t.IntField);
			Assert.That(t.FloatField, Is.InRange(3.09999f, 3.10001f)); // float imprecision
			Assert.AreEqual(TestObject.Colour.Green, t.EnumField);

		}

        [Test]
        public void TestLoadMultiWithSpacesInHeader()
        {
            // Header first, then N values
            // #Field headers are ignored
            // This time we don't want any prefixing since not trimmed
            string csvData = @"String Field, FloatField ,#Description, Int  Field ,   EnumField
""This,has,commas,in it"",2.34,Something ignored,35,Red
Hello World,256.25,""Notes here"",10003,Purple
Zaphod Beeblebrox,3.1,""Amazingly amazing"",000359,Green";

            List<TestObject> objs;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
                using (var sr = new StreamReader(ms)) {
                    objs = CsvUtil.LoadObjects<TestObject>(sr);
                }
            }

            Assert.That(objs, Has.Count.EqualTo(3));
            TestObject t = objs[0];
            Assert.AreEqual("This,has,commas,in it", t.StringField);
            Assert.AreEqual(35, t.IntField);
            Assert.That(t.FloatField, Is.InRange(2.33999f, 2.340001f)); // float imprecision
            Assert.AreEqual(TestObject.Colour.Red, t.EnumField);
            t = objs[1];
            Assert.AreEqual("Hello World", t.StringField);
            Assert.AreEqual(10003, t.IntField);
            Assert.That(t.FloatField, Is.InRange(256.24999f, 256.25001f)); // float imprecision
            Assert.AreEqual(TestObject.Colour.Purple, t.EnumField);
            t = objs[2];
            Assert.AreEqual("Zaphod Beeblebrox", t.StringField);
            Assert.AreEqual(359, t.IntField);
            Assert.That(t.FloatField, Is.InRange(3.09999f, 3.10001f)); // float imprecision
            Assert.AreEqual(TestObject.Colour.Green, t.EnumField);
        }

        [Test]
        public void TestLoadProperties()  {
            // It's ok to have newline with padding at start, should trim field names (not values)
            // Include spaces in string field value to prove all content preserved
            string csvData = @"StringProperty, Hello World  ,This is an ignored description,also ignored
			EnumProperty,Blue,Something Something
			IntProperty,1234,Comment here
			FloatProperty,1.5,More commenting";

            TestPropertiesObject t = new TestPropertiesObject();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
                using (var sr = new StreamReader(ms)) {
                    CsvUtil.LoadObject(sr, ref t);
                }
            }

            Assert.AreEqual(" Hello World  ", t.StringProperty);
            Assert.AreEqual(1234, t.IntProperty);
            Assert.That(t.FloatProperty, Is.InRange(1.4999f, 1.5001f)); // float imprecision
            Assert.AreEqual(TestPropertiesObject.Colour.Blue, t.EnumProperty);
        }

        [Test]
        public void TestLoadPropertiesAndFieldsSameName() {
            // It's ok to have newline with padding at start, should trim field names (not values)
            // Include spaces in string field value to prove all content preserved
            string csvData = @"StringMember, Hello World  ,This is an ignored description,also ignored
			EnumMember,Blue,Something Something
			IntMember,1234,Comment here
			FloatMember,1.5,More commenting";

            TestPropertiesAndFieldsObject t = new TestPropertiesAndFieldsObject();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
                using (var sr = new StreamReader(ms)) {
                    CsvUtil.LoadObject(sr, ref t);
                }
            }

            Assert.AreEqual(" Hello World  ", t.StringMember);
            Assert.AreEqual(" Hello World  ", t.GetStringField());
            Assert.AreEqual(1234, t.IntMember);
            Assert.AreEqual(1234, t.GetIntField());
            Assert.That(t.FloatMember, Is.InRange(1.4999f, 1.5001f)); // float imprecision
            Assert.That(t.GetFloatField(), Is.InRange(1.4999f, 1.5001f)); // float imprecision
            Assert.AreEqual(TestPropertiesAndFieldsObject.Colour.Blue, t.EnumMember);
            Assert.AreEqual(TestPropertiesAndFieldsObject.Colour.Blue, t.GetEnumField());
        }

        [Test]
		public void TestSaveSingle() {

			var obj = new TestObject("Hello there", 123, 300.2f, TestObject.Colour.Blue);
			using (var stream = new MemoryStream(256)) {
				using (var w = new StreamWriter(stream)) {
					CsvUtil.SaveObject(obj, w);
					w.Flush();

					stream.Seek(0, SeekOrigin.Begin);
					var r = new StreamReader(stream);
					var content = r.ReadToEnd();
					var expected = @"StringField,Hello there
IntField,123
FloatField,300.2
EnumField,Blue";
					Assert.AreEqual(expected, content);
				}
			}
		}

		[Test]
		public void TestSaveSingleQuoted() {

			var obj = new TestObject("Hello, there", 123, 300.2f, TestObject.Colour.Blue);
			using (var stream = new MemoryStream(256)) {
				using (var w = new StreamWriter(stream)) {
					CsvUtil.SaveObject(obj, w);
					w.Flush();

					stream.Seek(0, SeekOrigin.Begin);
					var r = new StreamReader(stream);
					var content = r.ReadToEnd();
					var expected = @"StringField,""Hello, there""
IntField,123
FloatField,300.2
EnumField,Blue";
					Assert.AreEqual(expected, content);
				}
			}
		}

		[Test]
		public void TestSaveMulti() {

			List<TestObject> objs = new List<TestObject>() {
				new TestObject("Hello there", 123, 300.2f, TestObject.Colour.Blue),
				new TestObject("This,has,commas", 42, 12.123f, TestObject.Colour.Purple),
				new TestObject("Semi;colons", 40001, -75.2f, TestObject.Colour.Green),
			};
			using (var stream = new MemoryStream(256)) {
				using (var w = new StreamWriter(stream)) {
					CsvUtil.SaveObjects(objs, w);
					w.Flush();

					stream.Seek(0, SeekOrigin.Begin);
					var r = new StreamReader(stream);
					var content = r.ReadToEnd();
					var expected = @"StringField,IntField,FloatField,EnumField
Hello there,123,300.2,Blue
""This,has,commas"",42,12.123,Purple
""Semi;colons"",40001,-75.2,Green";
					Assert.AreEqual(expected, content);
				}
			}
		}

		public struct TestStruct {
			public float f;
			public string s;
		}

		[Test]
		public void TestLoadStruct() {
			string csvData = @"s, Hello World  ,This is an ignored description,also ignored
			f,1234.5,Comment here";

			TestStruct t = new TestStruct();

			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData))) {
				using (var sr = new StreamReader(ms)) {
					CsvUtil.LoadObject(sr, ref t);
				}
			}

			Assert.AreEqual(" Hello World  ", t.s);
			Assert.That(t.f, Is.InRange(1234.4999f, 1234.5001f)); // float imprecision

		}


	}
}