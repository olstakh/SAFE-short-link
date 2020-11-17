module MemoryStringStorage

    open System.Collections.Concurrent

    let toChar = function
        | lowerCaseLetter when 0 <= lowerCaseLetter && lowerCaseLetter < 26 -> (int('a') + lowerCaseLetter) |> char
        | upperCaseLetter when 26 <= upperCaseLetter && upperCaseLetter < 52 -> (int('A') + upperCaseLetter - 26) |> char
        | digit when 52 <= digit && digit < 62 -> (int('0') + digit - 52) |> char
        | 62 -> '+'
        | 63 -> '-'
        | unsupported -> failwithf "%d is an unsupported generated number" unsupported

    let r = System.Random()

    let generate() =
        [1 .. 6] |> List.map(fun _ -> r.Next(64) |> toChar) |> System.String.Concat

    type MemoryStorage() =

        let storage = ConcurrentDictionary<string, string>()
           
        member __.AddValueAndGenerateKey value =
            Seq.unfold(fun key -> Some((key, value), generate())) (generate())
            |> Seq.find(storage.TryAdd)
            |> fst

        member __.TryGetValue = storage.TryGetValue

        member __.TryAdd (key, value) = storage.TryAdd(key, value)


