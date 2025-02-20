﻿using redis_csharp.src;
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

            //Get the length of a list
            int namesLength = redis.LLen("names");
            Console.WriteLine(String.Format("The length of names is : {0}", namesLength));

            //search for the position of a element in a key. 
            int position = redis.LPos("names", "John Doe");
            Console.WriteLine(position);

            //delete the list
            redis.Delete("names");

            //new list examples with 3 John & 1 Foo
            redis.RPush("examples", "John");
            redis.RPush("examples", "John");
            redis.RPush("examples", "Foo");
            redis.RPush("examples", "John");

            //delete 2 John in examples
            int removedElements = redis.LRem("examples", 2, "John");
            Console.WriteLine(String.Format("count of removed elements : {0}", removedElements));

            //and get the length of the list after.
            int examplesLength = redis.LLen("examples");
            Console.WriteLine(String.Format("The length of examples : {0}", examplesLength));

            //Dispose
            redis.Dispose();

            //wait until the end
            Console.ReadKey();
        }
    }
}
