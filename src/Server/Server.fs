module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Giraffe

open Shared
open MemoryStringStorage

let storage = MemoryStorage()

let randomStringApi =
    {
        getUniqueString = fun () -> async { return storage.GetUniqueString() }
        isStringUsed = fun str -> async { return storage.IsUnique str }
    }

let webApi_randomString =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue randomStringApi
    |> Remoting.buildHttpHandler

let webApp = choose [ webApi_randomString ]    

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
