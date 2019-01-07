namespace FabScaffold.Application.Home

open Global
open Helper
open Fabulous.Core
open Person
open System

module Types =
    type Msg =
        | NewPerson of string option
        | SelectedPersonId of string
        | UpdateSearch of string
        | SearchUpdated of (char * Person []) []
        | UpdatePersons of Person [] * int
        | LoggedOut

    type Model =
        { Persons : Person []
          FilteredPersons : (char * Person []) []
          PersonCount : int
          SearchText : string
          IsLoaded : bool }

module State =
    open Types

    let init (model : Model option) : Model * Cmd<Msg> =
        insertDummyData() //will only add for debug
        let res =
            match model with
            | Some m -> m
            | None ->
                { Persons = [||]
                  FilteredPersons = [||]
                  PersonCount = 0
                  SearchText = ""
                  IsLoaded = false }
        res, Cmd.ofAsyncMsg (async { let! p = getPersons()
                                     let! c = getPersonCount()
                                     return (UpdatePersons(p, c)) })

    let update msg model =
        match msg with
        | NewPerson _ -> model, Cmd.none //Captured outside
        | SelectedPersonId _ -> model, Cmd.none //Captured outside
        | UpdateSearch s ->
            let res =
                if (String.IsNullOrEmpty s) then ""
                else s
            { model with SearchText = res },
            Cmd.ofAsyncMsg (async {
                                let! res = model.Persons
                                           |> Array.filter
                                                  (fun x ->
                                                  x.FirstName.ToUpper()
                                                   .Contains(res.ToUpper()))
                                           |> getPersonsGroup
                                return SearchUpdated res
                            })
        | UpdatePersons(p, c) ->
            { model with Persons = p
                         PersonCount = c },
            Cmd.ofMsg (UpdateSearch model.SearchText)
        | SearchUpdated p ->
            { model with FilteredPersons = p
                         IsLoaded = true }, Cmd.none
        | LoggedOut -> model, Cmd.none

module View =
    open Types
    open Fabulous.DynamicViews
    open Xamarin.Forms

    let createPersonList (persons : (char * Person []) []) =
        let avatar p =
            View.StackLayout(children = [ View.Image(source = p.Avatar) ])

        let detail p =
            View.StackLayout(orientation = StackOrientation.Vertical,
                             children = [ View.Label
                                              ("Name: " + p.FirstName + " "
                                               + p.LastName)

                                          View.Label
                                              ("Gender: " + p.Gender.ToString())
                                          View.Label("Age: " + p.age()) ])
        [ for i in persons do
              let (x, y) = i
              let title = x.ToString()
              yield (title,
                     View.Label
                         (text = title, backgroundColor = Color.LightBlue,
                          textColor = Color.Black, fontSize = 20),
                     [ for j in y do
                           yield View.StackLayout
                                     (orientation = StackOrientation.Vertical,
                                      children = [ View.StackLayout
                                                       (orientation = StackOrientation.Horizontal,
                                                        children = [ avatar j
                                                                     detail j ])
                                                   View.Label(text = "     ") ]) ]) ]

    let search model dispatch =
        View.SearchBar
            (text = model.SearchText, placeholder = "Search..",
             backgroundColor = Color.BurlyWood,
             textChanged = debounce DebounceNo (fun args ->
                               args.NewTextValue
                               |> UpdateSearch
                               |> dispatch),
             searchCommand = (UpdateSearch >> dispatch),
             cancelButtonColor = Color.Accent)

    let isLoading = View.Label("Loading...")

    let plist model dispatch =
        View.ListViewGrouped
            (hasUnevenRows = true, rowHeight = -1, showJumpList = true,
             items = createPersonList model.FilteredPersons,
             itemTapped = (fun (x, y) ->
             let pg = model.FilteredPersons.[x]
             let (_, p) = pg
             p.[y].Id.ToString()
             |> SelectedPersonId
             |> dispatch), horizontalOptions = LayoutOptions.CenterAndExpand)

    let PersonList model dispatch =
        View.StackLayout(children = [ yield search model dispatch
                                      if model.IsLoaded then
                                          yield plist model dispatch
                                      else yield isLoading ])

    let root model dispatch =
        View.ContentPage(content = PersonList model dispatch)
            .HasBackButton(false)
            .ToolbarItems([ View.ToolbarItem
                                (text = "Add Person",
                                 command = (fun () -> NewPerson None |> dispatch))

                            View.ToolbarItem
                                (text = "logout",
                                 command = (fun _ -> LoggedOut |> dispatch)) ])
