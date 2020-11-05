using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using redis_csharp.src;

namespace Tests
{
    [TestClass]
    public class CommandsTests
    {
        [TestMethod]
        public void SetWorks()
        {
            Redis redis = new Redis();
            redis.Set("Tour", "Eiffel");
            Assert.AreEqual("Eiffel", redis.GetString("Tour"));
        }

        [TestMethod]
        public void DeleteWorks()
        {
            Redis redis = new Redis();
            bool delete = redis.Delete("Tour");
            Assert.AreEqual(true, delete);
        }

        [TestMethod]
        public void ExistsWorks()
        {
            Redis redis = new Redis();
            redis.Set("exists", "true");
            bool exists = redis.Exists("exists");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ExpireWorks()
        {
            Redis redis = new Redis();
            redis.Set("foo", "bar");
            bool expire = redis.Expire("foo", 300);
            Assert.IsTrue(expire);

        }

        [TestMethod]
        public void TimeToLiveWorks()
        {
            Redis redis = new Redis();
            Assert.AreEqual(300, redis.TimeToLive("faa"));
        }

        [TestMethod]
        public void RenameWorks()
        {
            Redis redis = new Redis();
            bool rename = redis.Rename("foo", "faa");
            Assert.IsTrue(rename);
        }

        [TestMethod]
        public void PersistWorks()
        {
            Redis redis = new Redis();
            bool persist = redis.Persist("faa");
            Assert.IsTrue(persist);
            int ttl = redis.TimeToLive("faa");
            Assert.AreEqual(-1, ttl);
        }

        [TestMethod]
        public void IncrementWorks()
        {
            Redis redis = new Redis();
            redis.Set("increment", "2");
            int increment = redis.Increment("increment");
            Assert.AreEqual(3, increment);
        }
    }
}
