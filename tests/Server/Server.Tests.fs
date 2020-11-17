module Server.Tests

open Server.MemoryStringStorageTests
open Expecto

open Shared
open Server

open StackExchange.Redis

let server = testList "Server" [
    testCase "Redis" <| fun _ ->
        let connection = ConnectionMultiplexer.Connect("safeshortlink.redis.cache.windows.net:6380,password=t5Q2H4iLpyoV+vwwpgvld6xxhoZuVtXuN1pMDvDxgZM=,ssl=True,abortConnect=False")
        let cache : IDatabase = connection.GetDatabase()
        let response = cache.StringGet(RedisKey("Hello1"))
        Expect.isTrue (response.IsNull) "empty redis"

    testCase "Redis get and set" <| fun _ ->
        let connection = ConnectionMultiplexer.Connect("safeshortlink.redis.cache.windows.net:6380,password=t5Q2H4iLpyoV+vwwpgvld6xxhoZuVtXuN1pMDvDxgZM=,ssl=True,abortConnect=False")
        let cache : IDatabase = connection.GetDatabase()
        
        let r1 = cache.StringSet(RedisKey("Hello2"), RedisValue("Privet"))
        Expect.isTrue r1 "Redis value was expected to be set"

        let response = cache.StringGet(RedisKey("Hello2"))
        Expect.isFalse (response.IsNull) "empty redis"
]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
            memoryStorageUnitTests
            memoryStorageAsyncTests
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all