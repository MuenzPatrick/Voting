
using Microsoft.Research.SEAL;
using Voting.Server;
using Voting.Shared;

namespace VotingTests
{
    [TestClass]
    public class SealManagerTests
    {
        [TestMethod]
        public void TestEncrypt()
        {
            var SealManager = new SealManager();
            var cipher = SealManager.Encrypt(6);
        }

        [TestMethod]
        public void TestDecrypt()
        {
            int value = 6;
            var SealManager = new SealManager();
            var cipher = SealManager.Encrypt(value);
            var result = SealManager.Decrypt(cipher);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void AddCiphers()
        {
            int value1 = 6;
            int value2 = 4;
            var SealManager = new SealManager();
            var cipher1 = SealManager.Encrypt(value1);
            var cipher2 = SealManager.Encrypt(value2);
            var cipherResult = SealManager.AddCiphers(new List<Ciphertext> {cipher1, cipher2});
            var result = SealManager.Decrypt(cipherResult);
            Assert.AreEqual(value1 + value2, result);
        }
    }
}