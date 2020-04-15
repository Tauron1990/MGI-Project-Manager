using System;
using System.Collections.Generic;
using JKang.IpcServiceFramework.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JKang.IpcServiceFramework.Core.Tests
{
    [TestClass]
    public class DefaultValueConverterTest
    {
        private DefaultValueConverter _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new DefaultValueConverter();
        }

        [TestMethod]
        public void TryConvert_FloatToDouble()
        {
            var expected = 123.4f;

            var succeed = _sut.TryConvert(expected, typeof(double), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(double));
            Assert.AreEqual((double) expected, actual);
        }

        [TestMethod]
        public void TryConvert_Int32ToInt64()
        {
            var expected = 123;

            var succeed = _sut.TryConvert(expected, typeof(long), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(long));
            Assert.AreEqual((long) expected, actual);
        }

        [TestMethod]
        public void TryConvert_SameType()
        {
            var expected = DateTime.UtcNow;

            var succeed = _sut.TryConvert(expected, typeof(DateTime), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(DateTime));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryConvert_JObjectToComplexType()
        {
            var expected = new ComplexType
                           {
                               Int32Value = 123,
                               StringValue = "hello"
                           };
            var jObj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(expected));

            var succeed = _sut.TryConvert(jObj, typeof(ComplexType), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(ComplexType));
            Assert.AreEqual(expected.Int32Value, ((ComplexType) actual).Int32Value);
            Assert.AreEqual(expected.StringValue, ((ComplexType) actual).StringValue);
        }

        [TestMethod]
        public void TryConvert_Int32Array()
        {
            int[] expected = {1, 2};
            var jObj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(expected));

            var succeed = _sut.TryConvert(jObj, typeof(int[]), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(int[]));
            var actualArray = actual as int[];
            Assert.AreEqual(expected.Length, actualArray.Length);
            for (var i = 0; i < expected.Length; i++) Assert.AreEqual(expected[i], actualArray[i]);
        }

        [TestMethod]
        public void TryConvert_Int32List()
        {
            var expected = new List<int> {1, 2};
            var jObj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(expected));

            var succeed = _sut.TryConvert(jObj, typeof(List<int>), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(List<int>));
            var actualList = actual as List<int>;
            Assert.AreEqual(expected.Count, actualList.Count);
            for (var i = 0; i < expected.Count; i++) Assert.AreEqual(expected[i], actualList[i]);
        }

        [TestMethod]
        public void TryConvert_ComplexTypeArray()
        {
            ComplexType[] expected =
            {
                new ComplexType {Int32Value = 123, StringValue = "abc"},
                new ComplexType {Int32Value = 456, StringValue = "edf"}
            };
            var jObj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(expected));

            var succeed = _sut.TryConvert(jObj, typeof(ComplexType[]), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(ComplexType[]));
            var actualArray = actual as ComplexType[];
            Assert.AreEqual(expected.Length, actualArray.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsNotNull(actualArray[i]);
                Assert.AreEqual(expected[i].Int32Value, actualArray[i].Int32Value);
                Assert.AreEqual(expected[i].StringValue, actualArray[i].StringValue);
            }
        }

        [TestMethod]
        public void TryConvert_DerivedTypeToBaseType()
        {
            var succeed = _sut.TryConvert(new ComplexType(), typeof(IComplexType), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(ComplexType));
        }

        [TestMethod]
        public void TryConvert_StringToEnum()
        {
            var expected = EnumType.SecondOption;

            var succeed = _sut.TryConvert(expected.ToString(), typeof(EnumType), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(EnumType));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryConvert_Int32ToEnum()
        {
            var expected = EnumType.SecondOption;

            var succeed = _sut.TryConvert((int) expected, typeof(EnumType), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(EnumType));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryConvert_StringToGuid()
        {
            var expected = Guid.NewGuid();

            var succeed = _sut.TryConvert(expected.ToString(), typeof(Guid), out var actual);

            Assert.IsTrue(succeed);
            Assert.IsInstanceOfType(actual, typeof(Guid));
            Assert.AreEqual(expected, actual);
        }

        private interface IComplexType
        {
            int Int32Value { get; }
            string StringValue { get; }
        }

        private class ComplexType : IComplexType
        {
            public int Int32Value { get; set; }
            public string StringValue { get; set; }
        }

        private enum EnumType
        {
            FirstOption,
            SecondOption
        }
    }
}