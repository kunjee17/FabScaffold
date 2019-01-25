[<RequireQualifiedAccess>]
module Database

open Akavache

let Init() = Akavache.Registrations.Start("FabScaffoldApp")
#if DEBUG
let Cache = BlobCache.LocalMachine
// let Cache = BlobCache.InMemory
#else
let Cache = BlobCache.LocalMachine
#endif
