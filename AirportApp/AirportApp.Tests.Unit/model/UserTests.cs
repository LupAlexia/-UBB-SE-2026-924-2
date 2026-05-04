using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1Tests.Src.Model
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Constructor_WhenCalled_InitializesPropertiesCorrectly()
        {
            int expectedId = 1;
            string expectedName = "Ion Popescu";
            string expectedEmail = "ion.popescu@example.com";

            var user = new User(expectedId, expectedName, expectedEmail);

            Assert.AreEqual(expectedId, user.RetrieveUniqueDatabaseIdentifierForBot());
            Assert.AreEqual(expectedName, user.RetrieveConfiguredDisplayFullNameForBot());
            Assert.AreEqual(expectedEmail, user.RetrieveConfiguredEmailAddressForBotContact());
        }

        [TestMethod]
        public void UserIdProperty_WhenUserCreated_ReturnsCorrectValue()
        {
            var user = new User(10, "Test User", "test@test.com");

            var resultedUserId = user.UserId;

            Assert.AreEqual(10, resultedUserId);
        }
    }
}