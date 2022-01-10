# Redis property deserializer

## Overview
Redis has some command which follows the following format:
```
<property>:<value>
```

This package can be used to deserialize the text into and object. To get output of Redis command you can use any supported Redis package.

I use [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) package but I did not find that I could gather settings output into objects by this from cluster perspective. So I made my package to make the deserialization.

## Usage example
Let us be assumed that we have the following class:
```csharp
using OnlyAti;

internal class RedisClusterInfo
{
    [RedisPropertyName("cluster_state")]
    public string? ClusterStatus { get; set; } = null;

    [RedisPropertyName("cluster_known_nodes")]
    public int ClusterNodeCount { get; set; }

    [RedisPropertyName("cluster_size")]
    public int ClusterSize { get; set; }
}
```

We can use the deserialiazation as in the following example. In the example, I use [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library for connection and command execution.
```csharp
// Set the endpoints to the Redis instances
ConfigurationOptions redisOptions = new()
{
    EndPoints =
    {
        {"host1", 7000 },
        {"host1", 7001 },
        {"host1", 7002 },
        {"host2", 7000 },
        {"host2", 7001 },
        {"host2", 7002 }
    }
};

// Connect to Redis cluster
var conn = ConnectionMultiplexer.Connect(redisOptions);
IDatabase db = conn.GetDatabase();

// Execute CLUSTER INFO command
var status = db.Execute("cluster", "info");

// Sample output:
// cluster_state:ok
// cluster_slots_assigned: 16384
// cluster_slots_ok: 16384
// cluster_slots_pfail: 0
// cluster_slots_fail: 0
// cluster_known_nodes: 6
// cluster_size: 3
// cluster_current_epoch: 159
// cluster_my_epoch: 158
// cluster_stats_messages_ping_sent: 3761
// cluster_stats_messages_pong_sent: 4218
// cluster_stats_messages_publish_sent: 5
// cluster_stats_messages_auth - ack_sent:1
// cluster_stats_messages_sent: 7985
// cluster_stats_messages_ping_received: 4218
// cluster_stats_messages_pong_received: 3760
// cluster_stats_messages_fail_received: 1
// cluster_stats_messages_publish_received: 1
// cluster_stats_messages_auth - req_received:1
// cluster_stats_messages_received: 7981

if (!status.IsNull)
{
    // If not null, make the deserialization
    Console.WriteLine($"Satus type {status.Type}:\n{status.ToString()}");
    var ret = OnlyAti.Redis.PropertyDeserializer<RedisClusterInfo>(status.ToString());

    Console.WriteLine($"Cluster status: {ret.ClusterStatus}");
    Console.WriteLine($"Cluster node count: {ret.ClusterNodeCount}");
    Console.WriteLine($"Cluster size: {ret.ClusterSize}");
}
else
    Console.WriteLine("Status returned with null");
```
