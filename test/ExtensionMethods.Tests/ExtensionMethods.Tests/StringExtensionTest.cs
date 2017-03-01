using System;
using Extensions.StringExtensions;
using Shouldly;
using Xunit;

namespace ExtensionMethods.Tests {
    public class StringExtensionTest {

        [Fact]
        public void IsNumeric_Test() {
            var number = "0.70";
            var result = number.IsNumeric();
            result.ShouldBe(true);
        }

        [Fact]
        public void ConvertTurkishToEnglish_Test() {
            var sampleTest = "Ýlker Halil Türer";
            var result = sampleTest.ConvertTurkishToEnglish();
            result.ShouldBe("Ilker Halil Turer");
        }

        [Fact]
        public void RegExControl_Test() {
            var sampleRegEx = "(.*)(\\d+)(.*)";
            var line = "This order was placed for QT3000!OK ? ";
            var result = line.RegExControl(sampleRegEx);
            result.ShouldBe(true);

        }

        [Fact]
        public void IsNullOrEmpty_Test() {
            var empty = string.Empty;
            var result = empty.IsNullOrEmpty();
            result.ShouldBe(true);
        }

        [Fact]
        public void FormatString_Test() {
            var format = "{0}-{1}-{2}";
            var result = format.FormatString(1, 2, 3);
            result.ShouldBe("1-2-3");
        }

        [Fact]
        public void ToNullableDate_Test() {
            var exampleDate = "01.01.2016";
            var nullableDate = exampleDate.ToNullableDate();
            nullableDate.ShouldBe(new DateTime(2016, 1, 1));
        }


    }
}