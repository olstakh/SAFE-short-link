module Server.Tests

open Server.MemoryStringStorageTests
open Expecto

open Shared
open Server

let server = testList "Server" []

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