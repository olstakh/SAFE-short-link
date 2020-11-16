namespace Shared

open System

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IRandomStringApi =
    {
        getUniqueString : unit -> Async<string>
        isStringUsed : string -> Async<bool>
    }      