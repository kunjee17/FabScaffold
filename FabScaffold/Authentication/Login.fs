namespace FabScaffold.Auth.Login

open Helper
open Auth

module Types =
    type Msg =
        | ChangePassword of string
        | Login
        | LoggedIn
        | UpdateError of string list


    type Model =
        { Password : string
          Errs : string list
           }

module State =
    open Fabulous.Core
    open Types
    open Helper

    let init() : Model * Cmd<Msg> =
        { Password = ""
          Errs = []
           }, Cmd.none

    let update msg model =
        match msg with
        | ChangePassword p -> { model with Password = p }, Cmd.none
        | Login ->
            let p =
                async {
                    let! isAuthenticated = checkCode model.Password
                    if isAuthenticated then return [ LoggedIn ]
                    else
                        return [ ChangePassword ""

                                 UpdateError
                                     [ "Wrong Passcode please try again" ] ]
                }
            model, Helper.ofAsyncMsgList p
        | LoggedIn -> { model with Errs = [] }, Cmd.none
        | UpdateError s -> { model with Errs = s }, Cmd.none


module View =
    open Fabulous.Core
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types
    open ViewHelper

    let root model dispatch =
        View.ContentPage
            (title = " Log In ",
             content = View.StackLayout
                           (padding = 30.0,
                            verticalOptions = LayoutOptions.Center,
                            children = [ toastLabels model.Errs Toast.Error

                                         View.Entry
                                             (text = model.Password,
                                              placeholder = "Enter Passcode",
                                              textChanged = debounce DebounceNo (fun args ->
                                                                args.NewTextValue
                                                                |> ChangePassword
                                                                |> dispatch),
                                              isPassword = true)

                                         View.Button
                                             (text = " Log in ",
                                              backgroundColor = Color.CadetBlue,
                                              textColor = Color.White,
                                              horizontalOptions = LayoutOptions.Center,
                                              command = (fun () ->
                                              dispatch Login)) ]))
