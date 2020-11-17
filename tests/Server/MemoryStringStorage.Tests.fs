module Server.MemoryStringStorageTests

open Expecto

open MemoryStringStorage

let memoryStorageUnitTests = testList "MemoryStorageUnitTests" [
    testCase "Add an item" <| fun _ ->
        Expect.isTrue (MemoryStorage().TryAdd("key", "value")) "Expected to successfuly add a value to an empty storage"

    testCase "Add and retrieve an item" <| fun _ ->
        let s = MemoryStorage()
        s.TryAdd("key", "value") |> ignore

        let (isSuccess, value) = s.TryGetValue("key")
        Expect.isTrue isSuccess "Expected to successfuly retrieve added value"
        Expect.equal value "value" "Not expected value was retrieved from a storage"

    testCase "Retrieve an item from an empty storage" <| fun _ ->
        let (isSuccess, value) = MemoryStorage().TryGetValue("key")
        Expect.isFalse isSuccess "Expected to fail to retrieve a value from an empty storage"
        Expect.isNull value "Value should be null for an entry, not present in the storage"

    testCase "Retrieve non existing item" <| fun _ ->
        let s = MemoryStorage()
        s.TryAdd("key", "value") |> ignore
        let (isSuccess, value) = s.TryGetValue("key1")
        Expect.isFalse isSuccess "Expected to fail to retrieve a value by non-existing key"
        Expect.isNull value "Value should be null for an entry, not present in the storage"
]

let memoryStorageAsyncTests = testList "MemoryStorageAsyncTests" [
    testCase "Add items with different keys in parallel" <| fun _ ->

        let itemsToAdd = 100

        let s = MemoryStorage()

        let itemsAdded =
            [| 1 .. itemsToAdd |]
            |> Array.map(fun idx -> ("key" + idx.ToString(), "value" + idx.ToString()))
            |> Array.Parallel.map (s.TryAdd)
            |> Array.filter id
            |> Array.length

        Expect.equal itemsAdded itemsToAdd "Expected all items to be added to the storage, since all keys are unique"

        ()

    testCase "Add items with same key in parallel" <| fun _ ->

        let itemsToAdd = 100

        let s = MemoryStorage()

        let itemsAdded =
            [| 1 .. itemsToAdd |]
            |> Array.map(fun idx -> ("key", "value" + idx.ToString()))
            |> Array.Parallel.map (s.TryAdd)
            |> Array.filter id
            |> Array.length

        Expect.equal itemsAdded 1 "Expected to successfuly add one entry to the storage, since all attempts had the same key"

        ()        
]