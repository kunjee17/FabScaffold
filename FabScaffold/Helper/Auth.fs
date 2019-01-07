module Auth

open System
open System.Linq
open Akavache
open System.Reactive.Linq

type LoginCode = {
    Code : string
}

let MD5Hash (input : string) =
      use md5 = System.Security.Cryptography.MD5.Create()
      input
      |> System.Text.Encoding.ASCII.GetBytes
      |> md5.ComputeHash
      |> Seq.map (fun c -> c.ToString("X2"))
      |> Seq.reduce (+)

let registerCode (code) =
    async {
        do! Async.SwitchToThreadPool()
        let login = { Code = MD5Hash code}
        Database.Cache.InsertObject(Guid.NewGuid().ToString(), login).Wait() |> ignore
        return ()
    }

let checkCode (code) =
    async {
        do! Async.SwitchToThreadPool()
        let res = Database.Cache.GetAllObjects<LoginCode>().Wait()
        if res.Count() > 0 then
           return res.First().Code = MD5Hash code
        else return false
    }

let isCodeRegistered() =
    async {
        do! Async.SwitchToThreadPool()
        let res = Database.Cache.GetAllObjects<LoginCode>().Wait()
        return res.Count() > 0
    }
