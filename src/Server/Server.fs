module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Giraffe
open Microsoft.AspNetCore.Http

open Shared
open MemoryStringStorage

let storage = MemoryStorage()

let addHost (ctx : HttpContext) v = ctx.Request.Host.Value + "/" + v

let shortUrlApi (ctx : HttpContext) =
    {
        generateShortURL =
            fun longUrl ->
                async {
                    return storage.AddValueAndGenerateKey(longUrl) |> addHost ctx
                }

        mapShortURL =
            fun (longUrl, shortUrlSuffix) ->
                async {
                    return storage.TryAdd(shortUrlSuffix, longUrl) |> 
                           function
                           | false -> None
                           | true -> shortUrlSuffix |> addHost ctx |> Some
                }
    }

let webApi_shortURL =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext shortUrlApi
    |> Remoting.buildHttpHandler

let redirectShortLink = router {
    getf "/%s" (
        fun t ->
            let url = (storage.TryGetValue t) |> snd
            printfn "%s" url
            redirectTo true url
    )
}


let webApp = choose [
    webApi_shortURL
    redirectShortLink
]    

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
