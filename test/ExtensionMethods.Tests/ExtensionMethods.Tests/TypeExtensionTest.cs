using Extensions.TypeExtensions;
using NUnit.Framework;
using Shouldly;
 
namespace ExtensionMethods.Tests
{
    [TestFixture]
    public class TypeExtensionTest
    {

        [Test]
        public void IsNullable_Test()
        {
            var nullableType = typeof(int?).IsNullable();
            var notnullableType = typeof(int).IsNullable();
            //assert
            nullableType.ShouldBe(true);
            notnullableType.ShouldBe(false);
        }
        [Test]
        public void GetCoreType_Test()
        {
            var stringType = typeof(string);
            var coreType = stringType.GetCoreType();
            coreType.ShouldBe(stringType);
        }
    }
}