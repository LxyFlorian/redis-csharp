using redis_csharp.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //create a default connection with localhost host and 6379 port
            Redis redis = new Redis();

            //set a foo key with bar value
            redis.Set("foo", "bar");

            bool expire = redis.Expire("foo", 300); // timeout expire after 300s
            Console.WriteLine(expire); //true

            int ttl = redis.TimeToLive("foo");
            Console.WriteLine(ttl); // 300

            //get the value of foo key
            string foo = redis.GetString("foo");
            Console.WriteLine(foo); //bar

            //delete the foo key
            bool delete = redis.Delete("foo");
            Console.WriteLine(delete); // true

            //wait until the end
            Console.ReadKey();
        }
    }
}
