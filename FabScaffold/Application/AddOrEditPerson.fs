namespace FabScaffold.Application.AddOrEditPerson

open Global
open Helper
open Fabulous.Core

module Types =
    type Msg = Nothing

    type Model = int

module State =
    open Types

    let init(pkey) (m) = 0, Cmd.none
    let update msg model = model, Cmd.none

module View =
    open Types
    open Fabulous.DynamicViews
    open Xamarin.Forms

    let root model dispatch =
        View.ContentPage(content = View.Label("Application Page"))
