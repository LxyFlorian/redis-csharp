using redis_csharp.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redis_csharp.Examples
{
    public class example
    {
        Redis redis = new Redis(); //by default : "localhost":6379

        public void ListExamples()
        {
            //Set a key : 
            redis.Set("foo", "bar");

            //Get a key : 
            redis.GetString("foo"); // = bar
        }
    }
}
