using System;
using Extensions.DateTimeExtensions;
using Shouldly;
using Xunit;

namespace ExtensionMethods.Tests {
    public class DateTimeExtensionTest {

        [Fact]
        public void Between_True_Test() {

            var dateTime = new DateTime(2013, 1, 1);
            var result = dateTime.Between(new DateTime(2010, 1, 1), DateTime.Now);
            //assert
            result.ShouldBe(true);
        }
        [Fact]
        public void Between_False_Test() {

            var dateTime = new DateTime(2019, 1, 1);
            var result = dateTime.Between(new DateTime(2010, 1, 1), DateTime.Now);
            //assert
            result.ShouldBe(false);
        }

        [Fact]
        public void StartOfWeek_Test()
        {
            var dateTime = new DateTime(2017,3,1);
            var startOfWeek = dateTime.StartOfWeek(DayOfWeek.Monday);
            startOfWeek.ShouldBe(new DateTime(2017, 2, 27));
        }
    }
}