using System;
using Extensions.StringExtensions;
using Shouldly;
using NUnit.Framework;

namespace ExtensionMethods.Tests
{
    [TestFixture]
    public class StringExtensionTest
    {

        [Test]
        public void IsNumeric_Test()
        {
            var number = "0.70";
            var result = number.IsNumeric();
            result.ShouldBe(true);
        }

        [Test]
        public void ConvertTurkishToEnglish_Test()
        {
            var sampleTest = "Ýlker Halil Türer";
            var result = sampleTest.ConvertTurkishToEnglish();
            result.ShouldBe("Ilker Halil Turer");
        }

        [Test]
        public void RegExControl_Test()
        {
            var sampleRegEx = "(.*)(\\d+)(.*)";
            var line = "This order was placed for QT3000!OK ? ";
            var result = line.RegExControl(sampleRegEx);
            result.ShouldBe(true);

        }

        [Test]
        public void IsNullOrEmpty_Test()
        {
            var empty = string.Empty;
            var result = empty.IsNullOrEmpty();
            result.ShouldBe(true);
        }

        [Test]
        public void FormatString_Test()
        {
            var format = "{0}-{1}-{2}";
            var result = format.FormatString(1, 2, 3);
            result.ShouldBe("1-2-3");
        }

        [Test]
        public void ToNullableDate_Test()
        {
            var exampleDate = "01.01.2016";
            var nullableDate = exampleDate.ToNullableDate();
            nullableDate.ShouldBe(new DateTime(2016, 1, 1));
        }

        [Test]
        public void IEnumerable_ToString_Test()
        {
            var ints = new[] { 1, 3, 5, 7, 9, 11, 13, 15 };
            var result = ints.ToString("-");

            result.ShouldBe("1-3-5-7-9-11-13-15");

        }

        [Test]
        public void AppendJokerSqlText_Test()
        {
            var searchParameter = "ürgüp göreme";
            var result = searchParameter.AppendJokerSqlText();

            result.ShouldBe("[uüUÜ]r[gGðÐ][uüUÜ]p% [gGðÐ][oöOÖ]reme");
        }

        [Test]
        public void GetRandomText_Test()
        {
            var result = "My second home with my big bro".GetRandomText(10, false);
            string.IsNullOrWhiteSpace(result).ShouldBe(false);

            result.Length.ShouldBe(10);
        }

        [Test]
        public void GetCountryAndCityRequest_Test()
        {
            var countryAndCityRequest = "88.232.220.78".GetCountryAndCityRequest();
            countryAndCityRequest.ShouldNotBeNull();
            countryAndCityRequest.Status.ShouldBe("OK");
        }

    }
}