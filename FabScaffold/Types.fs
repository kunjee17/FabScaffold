namespace FabScaffold
open Global
open Helper

module Types =

    type Msg =
        | ApplicationMsg of Application.Types.Msg
        | AuthMsg of Auth.Types.Msg
        | PagePopped
        | RefreshCurrentPage

    type PageModel =
        | AuthModel of Auth.Types.Model
        | ApplicationModel of Application.Types.Model

    type Model =
        { PageStack : PageStack<PageModel>; IsAuthenticated : bool }
