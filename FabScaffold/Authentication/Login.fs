namespace FabScaffold.Auth.Login

open Helper
open Auth

module Types =
    type Msg =
        | ChangePassword of string
        | Login
        | LoggedIn

    type Model =
        { Password : string }

module State =
    open Fabulous.Core
    open Types
    open Helper

    let init() : Model * Cmd<Msg> = { Password = "" }, Cmd.none

    let update msg model =
        match msg with
        | ChangePassword p -> { model with Password = p }, Cmd.none
        | Login ->
            let p =
                async {
                    let! isAuthenticated = checkCode model.Password
                    if isAuthenticated then return LoggedIn
                    else
                        //TODO: notifiy user that password is not correct
                        return ChangePassword ""
                }
            model, Cmd.ofAsyncMsg p
        | LoggedIn -> model, Cmd.none

module View =
    open Fabulous.Core
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types

    let root model dispatch =
        View.ContentPage
            (title = "log in", backgroundColor = Color.AliceBlue,
             content = View.StackLayout
                           (padding = 30.0,
                            verticalOptions = LayoutOptions.Center,
                            children = [ View.Entry
                                             (text = model.Password,
                                              placeholder = "Enter Password",
                                              textChanged = debounce DebounceNo (fun args ->
                                                                args.NewTextValue
                                                                |> ChangePassword
                                                                |> dispatch),
                                              isPassword = true)

                                         View.Button
                                             (text = "Log in",
                                              backgroundColor = Color.LightGreen,
                                              horizontalOptions = LayoutOptions.Center,
                                              command = (fun () ->
                                              dispatch Login)) ]))
