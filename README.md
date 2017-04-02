
### Server Design Pattern: Cache-Aside

Applications that rely heavily on a data-store usually can benefit greatly from using the Cache-Aside Pattern. If used correctly, _this pattern can improve performance and help maintain consistency between the cache and the underlying data store._

#### Reading Data

Using the Cache-Aside Pattern dictates that when you want to retrieve an item from the Data Store, __first you check in your cache. If the item exists in the cache, you can use that. If the item does not exist in the cache, you have to query the data store, however on the way back you drop the item in the cache.__

![Reading Data using the Cache-Aside Pattern - Flow Diagram](https://raw.githubusercontent.com/ehime/patter-cacheaside/master/assets/ca-flow-diagram.png "Reading Data using the Cache-Aside Pattern - Flow Diagram")

Reading Data using the Cache-Aside Pattern - Flow Diagram In C#, you can implement something of this sort, where `Func<T> execute` is the function that returns data from your data store:

<script src="https://gist.github.com/ehime/adb19f48af4744866edfb7232d71c294.js"></script>



#### Updating Data
