module MemoryStringStorage

    type MemoryStorage() =

        let storage = ResizeArray<string>()
        let r = System.Random()

        let toChar = function
            | lowerCaseLetter when 0 <= lowerCaseLetter && lowerCaseLetter < 26 -> (int('a') + lowerCaseLetter) |> char
            | upperCaseLetter when 26 <= upperCaseLetter && upperCaseLetter < 52 -> (int('A') + upperCaseLetter - 26) |> char
            | digit when 52 <= digit && digit < 62 -> (int('0') + digit - 52) |> char
            | 62 -> '+'
            | 63 -> '-'

        let generate() =
            [1 .. 6] |> List.map(fun _ -> r.Next(64) |> toChar) |> System.String.Concat
            
        member __.GetUniqueString() =
            Seq.unfold(fun s -> Some(s, generate())) (generate()) |> Seq.find(__.IsUnique)

        member __.IsUnique = (=) >> storage.Exists >> not

