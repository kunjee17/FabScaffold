module ViewHelper

open Fabulous.Core
open Xamarin.Forms
open Fabulous.DynamicViews

let boldLabel txt = View.Label(text = txt, fontAttributes = FontAttributes.Bold)
let blankLabel = View.Label(text = "         ")

type Toast =
    | Success
    | Error
    | Warning
    | Info

let toastLabels data t =
    let c =
        match t with
        | Success -> Color.MediumSeaGreen
        | Error -> Color.IndianRed
        | Warning -> Color.SandyBrown
        | Info -> Color.DeepSkyBlue
    View.StackLayout
        (orientation = StackOrientation.Vertical,
         children = [ for d in data do
                          yield View.Label
                                    (text = d, textColor = c,
                                     horizontalTextAlignment = TextAlignment.Center) ])




[<Literal>]
let private imagePath = "FabScaffold."
let imageSourceFromName (name:string) =
    let res = name.Replace('/','.')
    ImageSource.FromResource ( imagePath + res, typeof<Toast>.Assembly)
