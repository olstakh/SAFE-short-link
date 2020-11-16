namespace Shared

open System

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IShortURL =
    {
        generateShortURL : string -> Async<string>
        mapShortURL : string * string -> Async<bool>
    }