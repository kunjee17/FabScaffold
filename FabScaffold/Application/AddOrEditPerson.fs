namespace FabScaffold.Application.AddOrEditPerson

open Global
open Helper
open Fabulous.Core
open Xamarin.Forms
open Person
open System

(*
type Gender =
    | Male = 0
    | Female = 1

type Geo =
    { Lat : float
      Lng : float }

type Address =
    { City : string
      State : string
      Geo : Geo
      Street : string
      Suite : string
      ZipCode : string }

type Company =
    { Bs : string
      Name : string
      CatchPhrase : string }

type Person =
    { Id : Guid
      FirstName : string
      LastName : string
      UserName : string
      WebSite : string
      Avatar : string
      Mobile : string
      Email : string
      Gender : Gender
      Address : Address
      Company : Company
      DateOfBirth : DateTime }
*)
//TODO: Add functionality for Company and Address and Geo
module Types =
    let genderItems =
        [| ("Male", Color.Black)
           ("Female", Color.Black) |]

    type Msg =
        | AddPerson
        | PersonAdded
        | UpdateFirstName of string
        | UpdateLastName of string
        | UpdateUserName of string
        | UpdateWebSite of string
        | UpdateAvatar of string
        | UpdateMobile of string
        | UpdateEmail of string
        | UpdateGender of int
        | UpdateBirthDate of DateTime
        | UpdateSelectedPerson of Person

    type Model =
        { Person : Person
          GenderIndex : int
          Title : string
          PersonUpdated : bool
          IsModelLoaded : bool }

module State =
    open Fabulous.Core
    open Types

    let editModeTitle (isEdit) =
        if isEdit then "Edit Person Detail"
        else "Fill New Person Detail"

    let init (pkey : string option) (model : Model option) : Model * Cmd<Msg> =
        let im =
            { Person = Person.Empty
              GenderIndex = 0
              Title = ""
              PersonUpdated = false
              IsModelLoaded = false }

        let res =
            match model with
            | Some m -> m
            | None -> im

        match pkey with
        | Some key ->
            { res with Title = editModeTitle true },
            Cmd.ofAsyncMsg (async { let! p = getPersonById key
                                    return UpdateSelectedPerson p })
        | None -> { res with Title = editModeTitle false; IsModelLoaded = true }, Cmd.none

    let update msg model =
        match msg with
        | AddPerson ->
            printfn "%s" (model.Person.ToString())
            if not (String.IsNullOrWhiteSpace model.Person.FirstName)
               && not (String.IsNullOrWhiteSpace model.Person.LastName) then
                let toast =
                    async {
                        do! addOrUpdatePaitent model.Person
                        //TODO: Notify user that person added Successfully
                        return PersonAdded
                    }
                model, Cmd.ofAsyncMsg toast
            else model, Cmd.none
        | PersonAdded -> { model with PersonUpdated = true }, Cmd.none
        | UpdateFirstName s ->
            { model with Person = { model.Person with FirstName = s } },
            Cmd.none
        | UpdateLastName s ->
            { model with Person = { model.Person with LastName = s } }, Cmd.none
        | UpdateUserName s ->
            { model with Person = { model.Person with UserName = s } }, Cmd.none
        | UpdateWebSite s ->
            { model with Person = { model.Person with WebSite = s } }, Cmd.none
        | UpdateAvatar s ->
            { model with Person = { model.Person with Avatar = s } }, Cmd.none
        | UpdateMobile s ->
            { model with Person = { model.Person with Mobile = s } }, Cmd.none
        | UpdateEmail s ->
            { model with Person = { model.Person with Email = s } }, Cmd.none
        | UpdateGender i ->
            { model with Person = { model.Person with Gender = enum i }
                         GenderIndex = i }, Cmd.none
        | UpdateBirthDate d ->
            { model with Person = { model.Person with DateOfBirth = d } },
            Cmd.none
        | UpdateSelectedPerson p ->
            let genderIndex =
                genderItems
                |> Array.findIndex (fun (x, _) -> x = p.Gender.ToString())
            { model with Person = p
                         GenderIndex = genderIndex
                         IsModelLoaded = true },
            Cmd.none

module View =
    open Fabulous.DynamicViews
    open Types

    (**
    * Conditional Events
    *   let handlerOpt = if condition then None else Some (fun args -> ())
        View.Entry(?textChanged=handlerOpt)
    * Another way to look at it
    * match condition with
        | true -> View.Entry(...).TextChanged((fun args -> ()))
        | false -> View.Entry(...)
    *)
    //TODO: refactor to view helper
    let attachChange (condition) (changeEvent) =
        if condition then debounce DebounceNo changeEvent |> Some
        else None

    let root model (dispatch : Msg -> unit) =
        View.ContentPage
            (View.ScrollView
                 (View.StackLayout
                      (children = [ View.Label
                                        (text = model.Title,
                                         textColor = Color.Black,
                                         horizontalTextAlignment = TextAlignment.Center,
                                         fontSize = 20)

                                    View.Entry
                                        (text = model.Person.FirstName,
                                         placeholder = "Enter First Name",
                                         ?textChanged = attachChange
                                                            model.IsModelLoaded (fun (args : TextChangedEventArgs) ->
                                                            args.NewTextValue
                                                            |> UpdateFirstName
                                                            |> dispatch),
                                         completed = (UpdateFirstName
                                                      >> dispatch))
                                    View.Entry
                                        (text = model.Person.LastName,
                                         placeholder = "Enter Last Name",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateLastName
                                                           |> dispatch),
                                         completed = (UpdateLastName >> dispatch))
                                    View.Entry
                                        (text = model.Person.UserName,
                                         placeholder = "Enter Last Name",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateUserName
                                                           |> dispatch),
                                         completed = (UpdateUserName >> dispatch))
                                    View.Entry
                                        (text = model.Person.WebSite,
                                         placeholder = "Enter WebSite",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateWebSite
                                                           |> dispatch),
                                         completed = (UpdateWebSite >> dispatch))
                                    View.Entry
                                        (text = model.Person.Avatar,
                                         placeholder = "Enter Avatar",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateAvatar
                                                           |> dispatch),
                                         completed = (UpdateAvatar >> dispatch))
                                    View.Entry
                                        (text = model.Person.Mobile,
                                         placeholder = "Enter Mobile Number",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateMobile
                                                           |> dispatch),
                                         completed = (UpdateMobile >> dispatch))
                                    View.Entry
                                        (text = model.Person.Email,
                                         placeholder = "Enter email",
                                         ?textChanged = attachChange model.IsModelLoaded (fun args ->
                                                           args.NewTextValue
                                                           |> UpdateEmail
                                                           |> dispatch),
                                         completed = (UpdateEmail >> dispatch))
                                    View.Label(text = "Enter Birth Date")

                                    View.DatePicker
                                        (date = model.Person.DateOfBirth,
                                         ?dateSelected = attachChange model.IsModelLoaded (fun args ->
                                         dispatch (UpdateBirthDate args.NewDate)),
                                         maximumDate = DateTime.Today,
                                         horizontalOptions = LayoutOptions.Fill)
                                    View.Picker
                                        (title = "Choose Gender",
                                         textColor = snd
                                                         genderItems.[model.GenderIndex],
                                         selectedIndex = model.GenderIndex,
                                         itemsSource = Array.map fst genderItems,
                                         ?selectedIndexChanged = attachChange model.IsModelLoaded (fun (i, item) ->
                                         dispatch (UpdateGender i)))

                                    View.StackLayout
                                        (orientation = StackOrientation.Horizontal,
                                         children = [ View.Button
                                                          (text = "Submit Person",
                                                           backgroundColor = Color.LightGreen,
                                                           //cornerRadius = 50,
                                                           horizontalOptions = LayoutOptions.Center,
                                                           command = (fun _ ->
                                                           AddPerson |> dispatch)) ]) ])))
