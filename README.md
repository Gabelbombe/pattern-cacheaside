
### Server Design Pattern: Cache-Aside

Applications that rely heavily on a data-store usually can benefit greatly from using the Cache-Aside Pattern. If used correctly, __this pattern can improve performance and help maintain consistency between the cache and the underlying data store.__

#### Reading Data

Using the Cache-Aside Pattern dictates that when you want to retrieve an item from the Data Store, __first you check in your cache. If the item exists in the cache, you can use that. If the item does not exist in the cache, you have to query the data store, however on the way back you drop the item in the cache.__

![Reading Data using the Cache-Aside Pattern - Flow Diagram](https://github.com/ehime/pattern-cacheaside/blob/master/assets/ca-flow-diagram.png?raw=true "Reading Data using the Cache-Aside Pattern - Flow Diagram")

Reading Data using the Cache-Aside Pattern - Flow Diagram In C#, you can implement something of this sort, where `Func<T> execute` is the function that returns data from your data store:

```cs
private static ConcurrentDictionary<string, object> concurrentDictionary = new ConcurrentDictionary<string, object>();

public static T CacheAside<T>(this ICacheManager cacheManager, Func<T> execute, TimeSpan? expiresIn, string key)
{
    var cached = cacheManager.Get(key);

    if (EqualityComparer<T>.Default.Equals(cached, default(T)))
    {
        object lockOn = concurrentDictionary.GetOrAdd(key, new object());

        lock (lockOn)
        {
            cached = cacheManager.Get(key);

            if (EqualityComparer<T>.Default.Equals(cached, default(T)))
            {
                var executed = execute();

                if (expiresIn.HasValue)
                    cacheManager.Set(key, executed, expiresIn.Value);
                else
                    cacheManager.Set(key, executed);

                return executed;
            }
            else
            {
                return cached;
            }
        }
    }
    else
    {
        return cached;
    }
}
```


#### Updating Data

When you are updating data in your data store it is __very important to also invalidate the data in your cache,__ to keep consistency.

![Updating Data using the Cache-Aside Pattern - Flow Diagram(https://github.com/ehime/pattern-cacheaside/blob/master/assets/ca-updating-diagram.png?raw=true "Updating Data using the Cache-Aside Pattern - Flow Diagram")

#### Caveats

It is important to keep in mind some of the issues that might arise when applying this pattern.

#### Lifetime of Cached Data

For Cache-Aside to be effective, __you need to ensure that the expiration policy of your data matches the pattern of access.__ If the expiration period is too short, the application will continuously retrieve data from the data store. Similarly, if the expiration period is too long and you don't invalidate the cache when updating, the cached data is likely to become stale.

#### Evicting Data

Caches are usually not as large as Data Stores, so whenever they are short in space they have to evict data. __Make sure to configure the eviction policy__ depending on the data you will be retrieving from the data store. It may be beneficial to have costly Data Store items be retained at the expense of more frequently accessed but less costly items.

#### Priming the Cache

It may be beneficial to have the application prepopulate the cache as part of the startup process. The Cache-Aside pattern will then be useful when items are evicted or expired.

#### Consistency

Implementing __the Cache-Aside pattern does not guarantee consistency between the data store and the cache.__ An item in the data store may be changed at any time by an external process, and this change might not be reflected in the cache until the next time the item is loaded into the cache. It is very important to make sure that the cache is invalidated anytime the content in the data store changes. It may be also beneficial to use a the Pub/Sub Pattern which we will talk about later on.

#### Local Caching

__Distributed applications that have local or in-memory caching are at great risk of having inconsistencies.__ In these scenarios you should use shared or distributed caching.

#### Static Data

If the data access patterns of your application shows that you only have static data,then the Cache-Aside Pattern is not that useful. If the data fits into the available cache space, prime the cache with the data on startup and apply a policy that prevents the data from expiring.
