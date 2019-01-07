namespace FabScaffold.Auth

module Types =
    type Msg =
        | LoginMsg of Login.Types.Msg
        | SignUpMsg of SignUp.Types.Msg

    type Model =
        { LoginModel : Login.Types.Model
          SignUpModel : SignUp.Types.Model }

module State =
    open Types
    open Fabulous.Core

    let init() =
        let login, loginCmd = Login.State.init()
        let signUp, signUpCmd = SignUp.State.init()
        { LoginModel = login
          SignUpModel = signUp },
        Cmd.batch [ Cmd.map LoginMsg loginCmd
                    Cmd.map SignUpMsg signUpCmd ]

    let update msg model =
        match msg with
        | LoginMsg m ->
            let login, loginCmd = Login.State.update m model.LoginModel
            { model with LoginModel = login }, Cmd.map LoginMsg loginCmd
        | SignUpMsg m ->
            let signUp, signUpCmd = SignUp.State.update m model.SignUpModel
            { model with SignUpModel = signUp }, Cmd.map SignUpMsg signUpCmd

module View =
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types
    open Fabulous.Core

    let root model dispatch =
        View.TabbedPage
            (useSafeArea = true, backgroundImage = "prescript_bg.jpg",
             children = [ Login.View.root model.LoginModel
                              (LoginMsg >> dispatch)

                          SignUp.View.root model.SignUpModel
                              (SignUpMsg >> dispatch) ])
