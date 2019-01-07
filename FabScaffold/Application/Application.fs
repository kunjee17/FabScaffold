namespace FabScaffold.Application

open Global
open Helper
open Fabulous.Core

module Types =
   type PageModel =
        | HomeModel of Home.Types.Model
        | AddOrEditPersonModel of AddOrEditPerson.Types.Model
        | PersonDetailModel of PersonDetail.Types.Model

    type Msg =
        | HomeMsg of Home.Types.Msg
        | AddOrEditPersonMsg of AddOrEditPerson.Types.Msg
        | PersonDetailMsg of PersonDetail.Types.Msg
        | AddNewPage of ApplicationPage
        | LoggedOut

    type Model =
        { CurrentPage : ApplicationPage
          CurrentPageModel : PageModel }


module State =
    open Types

    let init (initialPage : ApplicationPage) (model : Model option) : Model * Cmd<Msg> =
        match model with
        | Some m ->
            match initialPage, m.CurrentPageModel with
            | HomePage, HomeModel m ->
                let (a, aCmd) = Home.State.init (Some m)
                { CurrentPage = HomePage
                  CurrentPageModel = HomeModel a }, Cmd.map HomeMsg aCmd
            | AddOrEditPersonPage pKey, AddOrEditPersonModel m ->
                let (a, aCmd) = AddOrEditPerson.State.init (pKey) (Some m)
                { CurrentPage = AddOrEditPersonPage pKey
                  CurrentPageModel = AddOrEditPersonModel a },
                Cmd.map AddOrEditPersonMsg aCmd
            | PersonDetailPage pkey, PersonDetailModel m ->
                let (a, aCmd) = PersonDetail.State.init (pkey) (Some m)
                { CurrentPage = PersonDetailPage pkey
                  CurrentPageModel = PersonDetailModel a },
                Cmd.map PersonDetailMsg aCmd
            | _, _ ->
                failwithf "%A and %A is not valid page model pair" initialPage
                    model
        | None ->
            match initialPage with
            | HomePage ->
                let (a, aCmd) = Home.State.init (None)
                { CurrentPage = HomePage
                  CurrentPageModel = HomeModel a }, Cmd.map HomeMsg aCmd
            | AddOrEditPersonPage pKey ->
                let (a, aCmd) = AddOrEditPerson.State.init (pKey) None
                { CurrentPage = AddOrEditPersonPage pKey
                  CurrentPageModel = AddOrEditPersonModel a },
                Cmd.map AddOrEditPersonMsg aCmd
            | PersonDetailPage pkey ->
                let (a, aCmd) = PersonDetail.State.init (pkey) None
                { CurrentPage = PersonDetailPage pkey
                  CurrentPageModel = PersonDetailModel a },
                Cmd.map PersonDetailMsg aCmd

    let update msg model =
        match msg, model.CurrentPageModel with
        | HomeMsg msg, HomeModel m ->
            let (a, aCmd) = Home.State.update msg m
            let (res, resCmd) =
                { model with CurrentPageModel = HomeModel a },
                Cmd.map HomeMsg aCmd
            match msg with
            | Home.Types.Msg.NewPerson pkey ->
                res,
                Cmd.batch [ resCmd
                            Cmd.ofMsg (AddNewPage(AddOrEditPersonPage pkey)) ]
            | Home.Types.Msg.SelectedPersonId pKey ->
                res,
                Cmd.batch [ resCmd
                            Cmd.ofMsg (AddNewPage(PersonDetailPage pKey)) ]
            | Home.Types.Msg.LoggedOut -> model, Cmd.ofMsg LoggedOut
            | _ -> res, resCmd
        | PersonDetailMsg msg, PersonDetailModel m ->
            let (a, aCmd) = PersonDetail.State.update msg m
            let (res, resCmd) =
                { model with CurrentPageModel = PersonDetailModel a },
                Cmd.map PersonDetailMsg aCmd
            match msg with
            | PersonDetail.Types.Msg.EditPerson pkey ->
                res,
                Cmd.batch [ resCmd
                            Cmd.ofMsg (AddNewPage(AddOrEditPersonPage pkey)) ]
            | _ -> res, resCmd
        | AddOrEditPersonMsg msg, AddOrEditPersonModel m ->
            let (a, aCmd) = AddOrEditPerson.State.update msg m
            let (res, resCmd) =
                { model with CurrentPageModel = AddOrEditPersonModel a },
                Cmd.map AddOrEditPersonMsg aCmd
            res, resCmd
        | AddNewPage _, _ -> model, Cmd.none
        | LoggedOut, _ -> model, Cmd.none
        | _, _ ->
            failwithf "%A and %A is not valid msg model pair in Application" msg
                model

module View =
    open Fabulous.DynamicViews
    open Xamarin.Forms
    open Types

    let root model dispatch =
        match model.CurrentPage, model.CurrentPageModel with
        | HomePage, HomeModel m -> Home.View.root m (HomeMsg >> dispatch)
        | AddOrEditPersonPage _, AddOrEditPersonModel m ->
            AddOrEditPerson.View.root m (AddOrEditPersonMsg >> dispatch)
        | PersonDetailPage _, PersonDetailModel m ->
            PersonDetail.View.root m (PersonDetailMsg >> dispatch)
        | _, _ -> failwith "Wrong model view combination in Application"
