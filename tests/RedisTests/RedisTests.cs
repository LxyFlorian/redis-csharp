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

        [TestMethod]
        public void SetPortWorks()
        {
            Redis redis = new Redis(5555);
            Assert.AreEqual(5555, redis.Port);
        }

        [TestMethod]
        public void SetHostWorks()
        {
            Redis redis = new Redis("redis.mywebsite.com");
            Assert.AreEqual("redis.mywebsite.com", redis.Host);
        }

        [TestMethod]
        public void SetHostSetPortWorks()
        {
            Redis redis = new Redis("redis.mywebsite.com", 5555);
            Assert.AreEqual("redis.mywebsite.com", redis.Host);
            Assert.AreEqual(5555, redis.Port);
        }

        [TestMethod]
        public void SetWorks()
        {
            Redis redis = new Redis();
            redis.Set("Tour", "Eiffel");
            Assert.AreEqual("Eiffel", redis.GetString("Tour"));
        }
    }
}
