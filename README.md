# (Un)official C# library for Redis 💓 (Just for challenge)

This library will permit you to use redis in C#. 

## What is Redis ? (seriously ?)

« Redis is an open source (BSD licensed), in-memory data structure store, used as a database, cache and message broker. It supports data structures such as strings, hashes, lists, sets, sorted sets with range queries, bitmaps, hyperloglogs, geospatial indexes with radius queries and streams. » -> [redis.io](https://www.redis.io)

## Documentation

### Connection to Redis

Connection with basic parameters : 

```
Redis redis = new Redis() // host : localhost & port : 6379
```
Connection with custom host
```
Redis redis = new Redis("redis.mywebsite.com") // host : redis.mywebsite.com & port : 6379
```
Connection with custom host and custom port
```
Redis redis = new Redis("redis.mywebsite.com", 5050) // host : redis.mywebsite.com & port : 5050
```

### Communicate with Redis 

https://redis.io/commands/set
SET : 

```
redis.Set("foo", "bar") // Will set a new key foo with bar as value.
```

https://redis.io/commands/get
GET : 

```
redis.GetString("foo") // Will return the value of foo
```

https://redis.io/commands/del
DELETE : 

```
redis.Delete("foo") // Will return true
```

https://redis.io/commands/expire
EXPIRE : 

```
redis.Expire("foo", 300) // Timeout expire after 300s
```

https://redis.io/commands/ttl
TTL : 

```
redis.TimeToLive("foo") // Will return 300
```

https://redis.io/commands/rename
RENAME : 

```
redis.Rename("foo", "bar") // Will return true
```


https://redis.io/commands/persist
PERSIST : 

```
redis.Persist("foo") // Will return true
```


https://redis.io/commands/incr
INCR : 

```
redis.Increment("foo") // Will return true
```


https://redis.io/commands/rpush
RPUSH : 

```
int length = redis.RPush("names", "John Doe") // Will push John Doe to names and return the length of the list
```


https://redis.io/commands/lpush
LPUSH : 

```
int length = redis.LPush("names", "John Doe") // Will push John Doe at the first position of names and return the length of the list
```


https://redis.io/commands/llen
LLEN : 

```
int length = redis.LLen("names") // Will return the length of the list
```


https://redis.io/commands/lpos
LPOS : 

```
int position = redis.LPos("names", "John Doe") // Will return the position of John Doe in the list
```



https://redis.io/commands/lrem
LRem : 

```
int removedElements = redis.LRem("names", 2, "John Doe") // Will remove 2 John Doe of the list
```
