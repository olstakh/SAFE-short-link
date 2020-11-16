module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model =
    {   LongURL : string
        ShortURL : string }

type Msg =
    | ShortenURL
    | ReceivedShortURL of string
    | SetLongURL of string
    
let getShortUrlApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IShortURL>

let init(): Model * Cmd<Msg> =
    let model =
        { ShortURL = ""; LongURL = "" }
    model, Cmd.none

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | ShortenURL ->
        let cmd = Cmd.OfAsync.perform getShortUrlApi.generateShortURL (model.LongURL) ReceivedShortURL
        model, cmd
    | ReceivedShortURL str ->
        { model with ShortURL = str }, Cmd.none
    | SetLongURL url ->
        { model with LongURL = url }, Cmd.none

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Field.div [ ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.LongURL
                  Input.Placeholder "URL to shorten"
                  Input.OnChange(fun x -> SetLongURL x.Value |> dispatch)
                ]
            ]
            Control.p [ ] [
                Input.text [
                  Input.Value model.ShortURL
                  Input.Placeholder "Short link"
                ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ -> dispatch ShortenURL)
                ] [
                    str "Get random string"
                ]
            ]
        ]
    ]

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "SAFE" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
