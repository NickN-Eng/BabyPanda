using Microsoft.Data.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BabyPanda.Tests
{
    [TestClass]
    public class Double_TypeFunction_Tests
    {
        [TestMethod]
        public void TryConvertColumn_DoubleDataFrameColumnToIntStringDouble_Success()
        {
            var dtypeFunc = (ITypeFunction)(new Double_TypeFunction());
            var doubleCol = new DoubleDataFrameColumn("col", new double?[] { 2, 45, null, 454.43453 });
            var dresult = dtypeFunc.TryConvert_Column(doubleCol , typeof(Int32DataFrameColumn ), out DataFrameColumn  dresultCol1);
            var dresult2 = dtypeFunc.TryConvert_Column(doubleCol, typeof(StringDataFrameColumn), out DataFrameColumn  dresultCol2);
            var dresult3 = dtypeFunc.TryConvert_Column(doubleCol, typeof(DoubleDataFrameColumn), out DataFrameColumn  dresultCol3);

            var dresultCol1_cast = (Int32DataFrameColumn )dresultCol1;
            var dresultCol2_cast = (StringDataFrameColumn)dresultCol2;
            var dresultCol3_cast = (DoubleDataFrameColumn)dresultCol3;

            Assert.IsTrue(dresult);
            Assert.IsTrue(dresult2);
            Assert.IsTrue(dresult3);
            Assert.AreEqual(2   ,dresultCol1_cast[0]);
            Assert.AreEqual(null,dresultCol1_cast[2]);
            Assert.AreEqual(454 ,dresultCol1_cast[3]);
            Assert.AreEqual("2" ,dresultCol2_cast[0]);
            Assert.AreEqual(null,dresultCol2_cast[2]);
            Assert.AreEqual(2   ,dresultCol3_cast[0]);
            Assert.AreEqual(null,dresultCol3_cast[2]);
        }


        [TestMethod]
        public void TryConvertObject_DoubleToIntDoubleString_Success()
        {
            double? dobjValue = 5.45;
            var dtypeFunc = (ITypeFunction)(new Double_TypeFunction());
            var dresult1 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(int), out object dresultValue1);
            var dresult2 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(double), out object dresultValue2);
            var dresult3 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(string), out object dresultValue3);

            var dresultValue1_cast = (int)dresultValue1;
            var dresultValue2_cast = (double)dresultValue2;
            var dresultValue3_cast = (string)dresultValue3;

            Assert.IsTrue(dresult1);
            Assert.IsTrue(dresult2);
            Assert.IsTrue(dresult3);
            Assert.AreEqual(5,      dresultValue1_cast);
            Assert.AreEqual(5.45,   dresultValue2_cast);
            Assert.AreEqual("5.45", dresultValue3_cast);
        }

        public void TryConvertObject_DoubleRoundingToInt_Success()
        {
            //WIPPPP
            double? dobjValue = (double?)5.45;
            var dtypeFunc = (ITypeFunction)(new Double_TypeFunction());
            var dresult1 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(int), out object dresultValue1);
            var dresult2 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(double), out object dresultValue2);
            var dresult3 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(string), out object dresultValue3);

            var dresultValue1_cast = (int)dresultValue1;
            var dresultValue2_cast = (double)dresultValue2;
            var dresultValue3_cast = (string)dresultValue3;

            Assert.IsTrue(dresult1);
            Assert.IsTrue(dresult2);
            Assert.IsTrue(dresult3);
            Assert.AreEqual(5, dresultValue1_cast);
            Assert.AreEqual(5.45, dresultValue2_cast);
            Assert.AreEqual("5.45", dresultValue3_cast);
        }

        [TestMethod]
        public void TryConvertObject_NullToIntDoubleString_Success()
        {
            double? dobjValue = null;
            var dtypeFunc = (ITypeFunction)(new Double_TypeFunction());
            var dresult1 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(int?), out object dresultValue1);
            var dresult2 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(double?), out object dresultValue2);
            var dresult3 = dtypeFunc.TryConvert_ToObject(dobjValue, typeof(string), out object dresultValue3);

            Assert.IsFalse(dresult1);
            Assert.IsFalse(dresult2);
            Assert.IsFalse(dresult3);
            Assert.AreEqual(null, dresultValue1);
            Assert.AreEqual(null, dresultValue2);
            Assert.AreEqual(null, dresultValue3);
        }

    }

    [TestClass]
    public class Int_TypeFunction_Tests
    {
        [TestMethod]
        public void TryConvertColumn_IntDataFrameColumnToIntStringDouble_Success()
        {
            var typeFunc = (ITypeFunction)(new Integer_TypeFunction());
            var intCol = new Int32DataFrameColumn("col", new int?[] { 2, 45, null, 454 });
            var result = typeFunc.TryConvert_Column(intCol, typeof(Int32DataFrameColumn), out   DataFrameColumn resultCol1);
            var result2 = typeFunc.TryConvert_Column(intCol, typeof(StringDataFrameColumn), out DataFrameColumn resultCol2);
            var result3 = typeFunc.TryConvert_Column(intCol, typeof(DoubleDataFrameColumn), out DataFrameColumn resultCol3);

            var dresultCol1_cast = (Int32DataFrameColumn)resultCol1;
            var dresultCol2_cast = (StringDataFrameColumn)resultCol2;
            var dresultCol3_cast = (DoubleDataFrameColumn)resultCol3;

            Assert.IsTrue(result);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.AreEqual(2, dresultCol1_cast[0]);
            Assert.AreEqual(null, dresultCol1_cast[2]);
            Assert.AreEqual(454, dresultCol1_cast[3]);
            Assert.AreEqual("2", dresultCol2_cast[0]);
            Assert.AreEqual(null, dresultCol2_cast[2]);
            Assert.AreEqual(2, dresultCol3_cast[0]);
            Assert.AreEqual(null, dresultCol3_cast[2]);
        }


        [TestMethod]
        public void TryConvertObject_IntToIntDoubleString_Success()
        {
            int? objValue = (int?)5;
            var typeFunc = (ITypeFunction)(new Double_TypeFunction());
            var result1 = typeFunc.TryConvert_ToObject(objValue, typeof(int), out object resultValue1);
            var result2 = typeFunc.TryConvert_ToObject(objValue, typeof(double), out object resultValue2);
            var result3 = typeFunc.TryConvert_ToObject(objValue, typeof(string), out object resultValue3);

            var dresultValue1_cast = (int)resultValue1;
            var dresultValue2_cast = (double)resultValue2;
            var dresultValue3_cast = (string)resultValue3;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.AreEqual(5, dresultValue1_cast);
            Assert.AreEqual(5, dresultValue2_cast);
            Assert.AreEqual("5", dresultValue3_cast);
        }

        [TestMethod]
        public void TryConvertObject_NullToIntDoubleString_Success()
        {
            int? objValue = null;
            var typeFunc = (ITypeFunction)(new Integer_TypeFunction());
            var result1 = typeFunc.TryConvert_ToObject(objValue, typeof(int?), out object resultValue1);
            var result2 = typeFunc.TryConvert_ToObject(objValue, typeof(double?), out object resultValue2);
            var result3 = typeFunc.TryConvert_ToObject(objValue, typeof(string), out object resultValue3);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
            Assert.AreEqual(null, resultValue1);
            Assert.AreEqual(null, resultValue2);
            Assert.AreEqual(null, resultValue3);
        }

    }
}
