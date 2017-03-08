using Extensions.TypeExtensions;
using Shouldly;
using Xunit;

namespace ExtensionMethods.Tests {
    public class TypeExtensionTest {

        [Fact]
        public void IsNullable_Test() {
            var nullableType = typeof(int?).IsNullable();
            var notnullableType = typeof(int).IsNullable();
            //assert
            nullableType.ShouldBe(true);
            notnullableType.ShouldBe(false);
        }
        [Fact]
        public void GetCoreType_Test() {
            var stringType = typeof(string);
            var coreType = stringType.GetCoreType();
            coreType.ShouldBe(stringType);
        }
    }
}