namespace FabScaffold.Auth.SignUp

open System
open Fabulous.Core
open Auth
open Helper

module Types =
    type Msg =
        | UpdatePassword of string
        | UpdateConfirmPassword of string
        | SubmitPassword
        | PasswordSubmitted
        | CheckRegistration of bool
        | PasswordCheckFailed

    type Model =
        { Password : string
          ConfirmPassword : string
          IsRegistered : bool }

module State =
    open Types

    let init() : Model * Cmd<Msg> =
        { Password = ""
          ConfirmPassword = ""
          IsRegistered = false },
        Cmd.ofAsyncMsg (async { let! r = isCodeRegistered()
                                return CheckRegistration r })

    let update msg model =
        match msg with
        | UpdatePassword s -> { model with Password = s }, Cmd.none
        | UpdateConfirmPassword s ->
            { model with ConfirmPassword = s }, Cmd.none
        | SubmitPassword ->
            if (String.IsNullOrWhiteSpace model.Password)
               || (String.IsNullOrWhiteSpace model.ConfirmPassword)
               || (model.Password <> model.ConfirmPassword) then
                model, Cmd.ofAsyncMsg (async {
                                               //TODO: Show notification that password is failed
                                               return PasswordCheckFailed })
            else
                model,
                Cmd.ofAsyncMsg (async {
                                    do! registerCode model.Password
                                    return PasswordSubmitted
                                })
        | PasswordSubmitted ->
            let p = async { return CheckRegistration true }
            model, Cmd.ofAsyncMsg p
        | CheckRegistration r -> { model with IsRegistered = r }, Cmd.none
        | PasswordCheckFailed -> init()

module View =
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types

    let registered() =
        View.StackLayout
            (padding = 30., verticalOptions = LayoutOptions.Center,
             children = [ View.Label("Pass Code is registered - Try Loggin In") ])

    let signUpScreen model dispatch =
        View.StackLayout
            (padding = 30.0, verticalOptions = LayoutOptions.Center,
             children = [ View.Entry
                              (text = model.Password,
                               textChanged = debounce DebounceNo (fun args ->
                                                 args.NewTextValue
                                                 |> UpdatePassword
                                                 |> dispatch),
                               placeholder = "Set Password", isPassword = true)

                          View.Entry
                              (text = model.ConfirmPassword,
                               textChanged = debounce DebounceNo (fun args ->
                                                 args.NewTextValue
                                                 |> UpdateConfirmPassword
                                                 |> dispatch),
                               placeholder = "Confirm Password",
                               isPassword = true)

                          View.Button
                              (text = "Submit",
                               backgroundColor = Color.LightGreen,
                               horizontalOptions = LayoutOptions.Center,
                               command = (fun _ -> SubmitPassword |> dispatch)) ])

    let root model dispatch =
        View.ContentPage(title = "sign up",
                         content = if model.IsRegistered then registered()
                                   else signUpScreen model dispatch)
