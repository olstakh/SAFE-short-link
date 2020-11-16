module MemoryStringStorage

    open System.Collections.Generic
    open System.Collections.Concurrent

    type MemoryStorage() =

        let storage = Dictionary<string, string>()
        let r = System.Random()

        let toChar = function
            | lowerCaseLetter when 0 <= lowerCaseLetter && lowerCaseLetter < 26 -> (int('a') + lowerCaseLetter) |> char
            | upperCaseLetter when 26 <= upperCaseLetter && upperCaseLetter < 52 -> (int('A') + upperCaseLetter - 26) |> char
            | digit when 52 <= digit && digit < 62 -> (int('0') + digit - 52) |> char
            | 62 -> '+'
            | 63 -> '-'
            | unsupported -> failwithf "%d is an unsupported generated number" unsupported

        let generate() =
            [1 .. 6] |> List.map(fun _ -> r.Next(64) |> toChar) |> System.String.Concat
            
        member __.AddValueAndGenerateKey value =
            let generatedString = Seq.unfold(fun s -> Some(s, generate())) (generate()) |> Seq.find(__.IsUnique)
            storage.Add(generatedString, value)
            generatedString

        member __.IsUnique = storage.ContainsKey >> not
            
        member __.TryGetValue = storage.TryGetValue

