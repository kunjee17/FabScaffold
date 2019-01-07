[<RequireQualifiedAccess>]
module Database

open Akavache

let Init() = Akavache.Registrations.Start("PrescriptionApp")
#if DEBUG
let Cache = BlobCache.InMemory
#else
let Cache = BlobCache.LocalMachine
#endif
