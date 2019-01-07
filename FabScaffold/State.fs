namespace FabScaffold
open Fabulous.Core
open Types
open Helper
open Global

module State =
    let init() =
        Database.Init()
        let auth, authCmd = Auth.State.init()
        { PageStack =
              [ { Page = AuthenticationPage
                  PageData = AuthModel auth } ]
          IsAuthenticated = false }, Cmd.map AuthMsg authCmd

    let update msg model =
        let currentPage = model.PageStack.Head
        match msg, currentPage.PageData with
        | ApplicationMsg msg, ApplicationModel m ->
            let (a, aCmd) = Application.State.update msg m

            let i =
                model.PageStack
                |> updateHead { Page = ApplicationPage a.CurrentPage
                                PageData = ApplicationModel a }

            let (res, resCmd) =
                { model with PageStack = i }, Cmd.map ApplicationMsg aCmd
            match msg with
            | Application.Types.Msg.AddNewPage(p) ->
                let (a, aCmd) = Application.State.init (p) None

                let i =
                    { Page = ApplicationPage a.CurrentPage
                      PageData = ApplicationModel a }
                    :: model.PageStack
                { res with IsAuthenticated = true
                           PageStack = i },
                Cmd.batch [ resCmd
                            Cmd.map ApplicationMsg aCmd ]
            | Application.Types.Msg.LoggedOut ->
                init()
            | _ -> res, resCmd
        | AuthMsg msg, AuthModel m ->
            let (a, aCmd) = Auth.State.update msg m

            let i =
                model.PageStack
                |> updateHead { Page = AuthenticationPage
                                PageData = AuthModel a }

            let res, resCmd = { model with PageStack = i }, Cmd.map AuthMsg aCmd
            match msg with
            | Auth.Types.Msg.LoginMsg Auth.Login.Types.Msg.LoggedIn ->
                let (a, aCmd) = Application.State.init (HomePage) None

                let i =
                    { Page = ApplicationPage a.CurrentPage
                      PageData = ApplicationModel a }
                    :: model.PageStack
                { res with IsAuthenticated = true
                           PageStack = i },
                Cmd.batch [ resCmd
                            Cmd.map ApplicationMsg aCmd ]
            | _ -> res, resCmd
        | PagePopped, _ ->
            { model with PageStack = removeFirst model.PageStack },
            Cmd.ofMsg RefreshCurrentPage
        | RefreshCurrentPage, _ ->
            match currentPage.Page, currentPage.PageData with
            | AuthenticationPage, _ ->
                let (a, aCmd) = Auth.State.init()

                let i =
                    model.PageStack
                    |> updateHead { Page = AuthenticationPage
                                    PageData = AuthModel a }
                { model with PageStack = i }, Cmd.map AuthMsg aCmd
            | ApplicationPage a, ApplicationModel m ->
                let (a, aCmd) = Application.State.init a (Some m)

                let i =
                    model.PageStack
                    |> updateHead { Page = ApplicationPage a.CurrentPage
                                    PageData = ApplicationModel a }
                { model with PageStack = i }, Cmd.map ApplicationMsg aCmd
            | _, _ -> failwithf "%A and %A is not valid page model pair for Application-State" msg model
        | _, _ -> failwithf "%A and %A is not valid msg model pair in State" msg model
