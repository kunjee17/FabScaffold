namespace FabScaffold.Auth

module Types =
    type Msg =
        | LoginMsg of Login.Types.Msg
        | SignUpMsg of SignUp.Types.Msg
        | CheckCodeIsRegister
        | CodeReigstreationChecked of bool

    type Model =
        { LoginModel : Login.Types.Model
          SignUpModel : SignUp.Types.Model
          IsPassCodeRegistered : bool }

module State =
    open Types
    open Fabulous.Core
    open Auth

    let init() =
        let login, loginCmd = Login.State.init()
        let signUp, signUpCmd = SignUp.State.init []
        { LoginModel = login
          SignUpModel = signUp
          IsPassCodeRegistered = false },
        Cmd.batch [ Cmd.map LoginMsg loginCmd
                    Cmd.map SignUpMsg signUpCmd
                    Cmd.ofMsg CheckCodeIsRegister ]

    let update msg model =
        match msg with
        | LoginMsg m ->
            let login, loginCmd = Login.State.update m model.LoginModel
            { model with LoginModel = login }, Cmd.map LoginMsg loginCmd
        | SignUpMsg m ->
            let signUp, signUpCmd = SignUp.State.update m model.SignUpModel
            let (res, resCmd) =
                { model with SignUpModel = signUp }, Cmd.map SignUpMsg signUpCmd

            match m with
            | SignUp.Types.Msg.PasswordSubmitted ->
                res, Cmd.batch [
                    resCmd
                    Cmd.ofMsg CheckCodeIsRegister
                ]
            | _ -> (res, resCmd)
        | CheckCodeIsRegister ->
            let res = async { let! r = isCodeRegistered()
                              return CodeReigstreationChecked r }
            model, Cmd.ofAsyncMsg res
        | CodeReigstreationChecked b ->
            { model with IsPassCodeRegistered = b }, Cmd.none

module View =
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types
    open Fabulous.Core

    let root model dispatch =
        if model.IsPassCodeRegistered then
            Login.View.root model.LoginModel (LoginMsg >> dispatch)
        else SignUp.View.root model.SignUpModel (SignUpMsg >> dispatch)
