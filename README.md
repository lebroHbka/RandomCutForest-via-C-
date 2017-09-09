# Random Cut Forest

Algorithm to find anomaly in multidimensional data stream.

-----
### Handle your data.

- All points might has same dimensions count.

- Points that encapsulated in Data class might be unique(at least in one dimension with some value), try timestamp or id.
>**Note**: Make sure that timestamp or id changes over time, much smaller than possible changes in value part of data.
>
>**Example:**
Points timestamp increase for every new point for 100.
And new data arriving with value part increased for 20,
even if it 300% of normal data and it for sure anomaly, that will not be treated like that, cuz timestamp changes for 100. 