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

SET : 

```
redis.Set("foo", "bar") // Will set a new key foo with bar as value.
```

GET : 

```
redis.GetString("foo") // Will return the value of foo
```
