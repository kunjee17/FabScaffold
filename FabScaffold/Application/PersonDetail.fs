namespace FabScaffold.Application.PersonDetail

open Global
open Helper
open Fabulous.Core
open Person

module Types =
    type Msg =
        | EditPerson of string option
        | PersonSelected of Person

    type Model =
        { SelectedPerson : Person }

module State =
    open Fabulous.Core
    open Types

    let init (pkey) (model : Model option) : Model * Cmd<Msg> =
        let res =
            match model with
            | Some m -> m
            | None -> { SelectedPerson = Person.Empty }
        res, Cmd.ofAsyncMsg (async { let! p = getPersonById pkey
                                     return PersonSelected p })

    let update msg model =
        match msg with
        | EditPerson pkey -> model, Cmd.none
        | PersonSelected p -> { model with SelectedPerson = p }, Cmd.none

module View =
    open Types
    open Fabulous.DynamicViews
    open Xamarin.Forms

    let header person dispatch =
        View.StackLayout
            (backgroundColor = Color.FromHex("#448cb8"),
             orientation = StackOrientation.Horizontal,
             padding = Thickness(50., 10., 20., 10.), spacing = 10.,
             children = [ View.Button
                              (text = "Edit Person",
                               backgroundColor = Color.BurlyWood,
                               horizontalOptions = LayoutOptions.Center,
                               command = (fun () ->
                               EditPerson(Some(person.Id.ToString()))
                               |> dispatch)) ])

    let personForm person =
        View.StackLayout
            (horizontalOptions = LayoutOptions.Start,
             orientation = StackOrientation.Vertical,
             margin = Thickness(0., 10., 0., 10.), spacing = 20.,
             children = [ View.Label
                              (text = "First Name: " + person.FirstName,
                               fontSize = 20)

                          View.Label
                              (text = "Last Name: " + person.LastName,
                               fontSize = 20)

                          View.Label
                              (text = "Username: " + person.UserName,
                               fontSize = 20)

                          View.Label
                              (text = "Date Of Birth: "
                                      + person.DateOfBirth.ToShortDateString(),
                               fontSize = 20)

                          View.Label
                              (text = "Age: " + person.age(), fontSize = 20)

                          View.Label
                              (text = "Email: " + person.Email, fontSize = 20)

                          View.Label
                              (text = "WebSite: " + person.WebSite,
                               fontSize = 20)

                          View.StackLayout
                              (orientation = StackOrientation.Horizontal,
                               children = [ View.Label
                                                (text = "Avatar: ",
                                                 fontSize = 20)
                                            View.Image(source = person.Avatar) ])

                          View.Label
                              (text = "Phone: " + person.Mobile, fontSize = 20)

                          View.Label
                              (text = "Gender: " + person.Gender.ToString(),
                               fontSize = 20) ])

    let companyForm company =
        View.StackLayout
            (horizontalOptions = LayoutOptions.Start,
             orientation = StackOrientation.Vertical,
             margin = Thickness(0., 10., 0., 10.), spacing = 20.,
             children = [ View.Label
                              (text = "Company Name: " + company.Name,
                               fontSize = 20)

                          View.Label
                              (text = "CP: " + company.CatchPhrase,
                               fontSize = 20)
                          View.Label(text = "BS: " + company.Bs, fontSize = 20) ])

    let root model dispatch =
        View.ContentPage
            (content = View.ScrollView
                           (content = View.StackLayout
                                          (children = [ header
                                                            model.SelectedPerson
                                                            dispatch

                                                        personForm
                                                            model.SelectedPerson

                                                        companyForm
                                                            model.SelectedPerson.Company ])))
