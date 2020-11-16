module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model =
    { RandomString : string }

type Msg =
    | GetRandomString
    | ReceivedRandomString of string
    
let getRandomStringApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IRandomStringApi>    

let init(): Model * Cmd<Msg> =
    let model =
        { RandomString = "" }
    model, Cmd.none

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GetRandomString ->
        let cmd = Cmd.OfAsync.perform getRandomStringApi.getUniqueString () ReceivedRandomString
        model, cmd
    | ReceivedRandomString str ->
        { model with RandomString = str }, Cmd.none

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
        Field.div [ Field.IsGrouped ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.RandomString
                  Input.Placeholder "Get random string from server"
                  Input.IsReadOnly true
                ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ -> dispatch GetRandomString)
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
