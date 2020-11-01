using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using redis_csharp.src;

namespace redis_csharp_tests
{
    [TestClass]
    public class RedisCSharpTests
    {
        [TestMethod]
        public void RedisHostWorks()
        {
            Redis redis = new Redis();
            Assert.AreEqual("localhost", redis.Host);
        }

        [TestMethod]
        public void RedisPortWorks()
        {
            Redis redis = new Redis();
            Assert.AreEqual(6379, redis.Port);
        }
    }
}
