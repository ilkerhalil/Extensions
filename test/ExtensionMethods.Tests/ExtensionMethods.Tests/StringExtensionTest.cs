using System;
using System.Xml.Serialization;
using Extensions.IEnumerableExtensions;
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

        [Fact]
        public void IEnumerable_ToString_Test()
        {
            var ints = new []{1,3,5,7,9,11,13,15};
            var result = ints.ToString("-");

            result.ShouldBe("1-3-5-7-9-11-13-15");

        }

        [Fact]
        public void AppendJokerSqlText_Test()
        {
            var searchParameter = "ürgüp göreme";
            var result = searchParameter.AppendJokerSqlText();

            result.ShouldBe("[uüUÜ]r[gGðÐ][uüUÜ]p% [gGðÐ][oöOÖ]reme");
        }

        [Fact]
        public void GetRandomText_Test()
        {
            var result = "My second home with my big bro".GetRandomText(10, false);
            string.IsNullOrWhiteSpace(result).ShouldBe(false);

            result.Length.ShouldBe(10);
        }

        [Fact]
        public void GetCountryAndCityRequest_Test()
        {
            var countryAndCityRequest = "88.232.220.78".GetCountryAndCityRequest();
            countryAndCityRequest.ShouldNotBeNull();
            countryAndCityRequest.Status.ShouldBe("OK");
        }

    }
}