module RedisCacheStorage

    open StackExchange.Redis

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

        let connection = ConnectionMultiplexer.Connect(System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_RedisCacheConnectionString"))
        let cache : IDatabase = connection.GetDatabase()

        member __.TryGetValue key =
            let v = key |> RedisKey |> cache.StringGet
            (not v.IsNull, v.ToString())

        member __.TryAdd (key, value) =
            cache.StringSet(
                RedisKey(key),
                RedisValue(value),
                System.Nullable<System.TimeSpan>(),
                When.NotExists
            )

        member __.AddValueAndGenerateKey value =
            Seq.unfold(fun key -> Some((key, value), generate())) (generate())
            |> Seq.find(__.TryAdd)
            |> fst
