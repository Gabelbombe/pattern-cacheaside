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
