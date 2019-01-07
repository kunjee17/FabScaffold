module Global

type ApplicationPage =
    | HomePage
    | AddOrEditPersonPage of string option
    | PersonDetailPage of string

type AppPage =
    | AuthenticationPage
    | ApplicationPage of ApplicationPage
