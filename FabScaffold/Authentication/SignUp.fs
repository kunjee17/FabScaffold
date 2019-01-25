namespace FabScaffold.Auth.SignUp

open System
open Fabulous.Core
open Auth
open Helper
open ViewHelper

module Types =
    type Msg =
        | UpdatePassword of string
        | UpdateConfirmPassword of string
        | SubmitPassword
        | PasswordSubmitted
        | PasswordCheckFailed of string list

    type Model =
        { Password : string
          ConfirmPassword : string
          IsRegistered : bool
          Errs : string list }

module State =
    open Types

    let init (errs) : Model * Cmd<Msg> =
        { Password = ""
          ConfirmPassword = ""
          IsRegistered = false
          Errs = errs }, Cmd.none

    let update msg model =
        match msg with
        | UpdatePassword s -> { model with Password = s }, Cmd.none
        | UpdateConfirmPassword s ->
            { model with ConfirmPassword = s }, Cmd.none
        | SubmitPassword ->
            if (String.IsNullOrWhiteSpace model.Password)
               || (String.IsNullOrWhiteSpace model.ConfirmPassword)
               || (model.Password <> model.ConfirmPassword) then
                model,
                Cmd.ofAsyncMsg
                    (async
                         {
                         return PasswordCheckFailed
                                    [ "Either one of the field is empty or Both are not same. Please Try Again!" ] })
            else
                model,
                Cmd.ofAsyncMsg (async {
                                    do! registerCode model.Password
                                    return PasswordSubmitted
                                })
        | PasswordSubmitted ->
            { model with Errs = [] }, Cmd.none
        | PasswordCheckFailed errs -> init (errs)

module View =
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types
    open ViewHelper

    let registered() =
        View.StackLayout
            (padding = 30., verticalOptions = LayoutOptions.Center,
             children = [ View.Label("Pass Code is registered - Try Loggin In") ])

    let signUpScreen model dispatch =
        View.StackLayout
            (padding = 30.0, verticalOptions = LayoutOptions.Center,
             children = [
                          View.Label
                              (text = "Please register your pass code here.",
                               horizontalTextAlignment = TextAlignment.Center,
                               textColor = Color.DeepSkyBlue)

                          toastLabels model.Errs Toast.Error

                          View.Entry
                              (text = model.Password,
                               textChanged = debounce DebounceNo (fun args ->
                                                 args.NewTextValue
                                                 |> UpdatePassword
                                                 |> dispatch),
                               placeholder = "Set Pass Code", isPassword = true)

                          View.Entry
                              (text = model.ConfirmPassword,
                               textChanged = debounce DebounceNo (fun args ->
                                                 args.NewTextValue
                                                 |> UpdateConfirmPassword
                                                 |> dispatch),
                               placeholder = "Confirm Pass Code",
                               isPassword = true)

                          View.Button
                              (text = " Submit ",
                               backgroundColor = Color.CadetBlue,
                               textColor = Color.White,
                               horizontalOptions = LayoutOptions.Center,
                               command = (fun _ -> SubmitPassword |> dispatch)) ])

    let root model dispatch =
        View.ContentPage(title = " Sign Up ",
                         content = if model.IsRegistered then registered()
                                   else signUpScreen model dispatch)
