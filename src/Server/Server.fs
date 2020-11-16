module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Giraffe
open Microsoft.AspNetCore.Http

open Shared
open MemoryStringStorage

let storage = MemoryStorage()

let shortUrlApi (ctx : HttpContext) =
    {
        generateShortURL = fun longUrl -> async { return ctx.Request.Host.Value + "/" + storage.AddValueAndGenerateKey(longUrl) }
        mapShortURL = fun (longUrl, shortUrlSuffix) -> async { return true }
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
