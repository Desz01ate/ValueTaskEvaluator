# ValueTaskEvaluator
For comparison between Task and ValueTask.

This repository is about the benchmark test between Task<T> vs ValueTask<T>
in cachable scenario and non-cache scenario to see how the memory is allocated for each setup

The result is pretty clear that ValueTask is significantly better than Task in lower-memory allocation
when the data is caching and need no async/await operation, otherwise there is no real benefit over Task.

##test
