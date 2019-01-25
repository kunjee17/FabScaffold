module Person

open System
open System.Linq
open Akavache
open System.Reactive.Linq

let [<Literal>] generatedPerson = 50

type Gender =
    | Male = 0
    | Female = 1

type Geo =
    { Lat : float
      Lng : float }
    static member Empty =
        { Lat = 0.
          Lng = 0. }

type Address =
    { City : string
      State : string
      Geo : Geo
      Street : string
      Suite : string
      ZipCode : string }
    static member Empty =
        { City = ""
          State = ""
          Geo = Geo.Empty
          Street = ""
          Suite = ""
          ZipCode = "" }

type Company =
    { Bs : string
      Name : string
      CatchPhrase : string }
    static member Empty =
        { Bs = ""
          Name = ""
          CatchPhrase = "" }

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

    static member Empty =
        { Id = Guid.NewGuid()
          FirstName = ""
          LastName = ""
          UserName = ""
          WebSite = ""
          Avatar = ""
          Mobile = ""
          Email = ""
          Gender = Gender.Male
          DateOfBirth = DateTime.Today
          Address = Address.Empty
          Company = Company.Empty }

    member x.age() =
        let getAge (d : DateTime) =
            let d' = DateTime.Now
            match d' > d with
            | true ->
                let months = 12 * (d'.Year - d.Year) + (d'.Month - d.Month)
                match d'.Day < d.Day with
                | true ->
                    let days =
                        DateTime.DaysInMonth(d.Year, d.Month) - d.Day + d'.Day
                    let years = (months - 1) / 12
                    let months' = (months - 1) - years * 12
                    (years, months', days)
                | false ->
                    let days = d'.Day - d.Day
                    let years = months / 12
                    let months' = months - years * 12
                    (years, months', days)
            | false -> (0, 0, 0)

        let y, m, d = getAge x.DateOfBirth
        sprintf "%i years & %i months & %i days" y m d

    override x.ToString() =
        sprintf "%s %s - %s - (%s)" x.FirstName x.LastName (x.Gender.ToString())
            (x.age())

let getPersons() =
    async {
        do! Async.SwitchToThreadPool()
        return Database.Cache.GetAllObjects<Person>().Wait().ToArray()
               |> Array.sortBy (fun p -> p.FirstName)
    }

let getPersonsGroup (p) =
    async {
        do! Async.SwitchToThreadPool()
        return p |> Array.groupBy (fun i -> i.FirstName.ToUpper().[0])
    }

let getPersonCount() =
    async {
        do! Async.SwitchToThreadPool()
        return Database.Cache.GetAllObjects<Person>().Wait().Count()
    }

let addOrUpdatePaitent (p : Person) =
    async {
        do! Async.SwitchToThreadPool()
        Database.Cache.InsertObject<Person>((p.Id.ToString()), p) |> ignore
        return ()
    }

let getPersonById key =
    async {
        do! Async.SwitchToThreadPool()
        return Database.Cache.GetObject<Person>(key).Wait()
    }

let private PersonFaker() =
    let g =
        Bogus.Faker<Person>("en_IND").CustomInstantiator(fun f ->
            { Id = Guid.NewGuid()
              FirstName = f.Person.FirstName
              LastName = f.Person.LastName
              UserName = f.Person.UserName
              WebSite = f.Person.Website
              Avatar = f.Person.Avatar
              Mobile = f.Person.Phone
              Email = f.Person.Email
              Gender = enum (int f.Person.Gender)
              DateOfBirth = f.Person.DateOfBirth
              Address =
                  { City = f.Person.Address.City
                    State = f.Person.Address.State
                    Geo =
                        { Lat = f.Person.Address.Geo.Lat
                          Lng = f.Person.Address.Geo.Lng }
                    Street = f.Person.Address.Street
                    Suite = f.Person.Address.Suite
                    ZipCode = f.Person.Address.ZipCode }
              Company =
                  { Bs = f.Person.Company.Bs
                    CatchPhrase = f.Person.Company.CatchPhrase
                    Name = f.Person.Company.Name } })
    g.Generate()

let insertDummyData() =
    //If there is no data then and only then add Dummy don't keep adding them
    //Only add for Debug
    let createDummyPerson() =
        async { return! (PersonFaker() |> addOrUpdatePaitent) }
#if DEBUG
    if (getPersonCount() |> Async.RunSynchronously) = 0 then
        Seq.init generatedPerson (fun i -> i)
        |> Seq.map (fun _ -> async { do! createDummyPerson() })
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously
    else ()
#endif

    ()
(**
Some Random Logic Testing
*)

















// let p = [| "Kunjan" |] |> Array.groupBy (fun i -> i.ToUpper().[0])
// let p1 = [| "Kunjan" |] |> Array.filter (fun x -> x.ToUpper().Contains(""))
