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

            bool persist = redis.Persist("foo");
            Console.WriteLine(persist); // true

            bool rename = redis.Rename("foo", "faa");
            Console.WriteLine(rename); //true

            //get the value of foo key
            string foo = redis.GetString("faa");
            Console.WriteLine(foo); //bar

            //delete the foo key
            bool delete = redis.Delete("faa");
            Console.WriteLine(delete); // true

            //set a key increment with 2 as value
            redis.Set("increment", "2");

            //increment the key "increment"
            int increment = redis.Increment("increment");
            Console.WriteLine(increment); // 3

            //Delete increment;
            redis.Delete("increment");

            //push John Doe to list names and return the length of the list
            int length = redis.RPush("names", "John Doe");
            Console.WriteLine(length); // 1;

            //Push Mr X at the first position of names
            redis.LPush("names", "Mr X");

            //Dispose
            redis.Dispose();

            //wait until the end
            Console.ReadKey();
        }
    }
}
