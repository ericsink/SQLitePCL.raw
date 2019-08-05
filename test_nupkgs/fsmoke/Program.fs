// Learn more about F# at http://fsharp.org

open System
open SQLitePCL.Ugly

[<EntryPoint>]
let main argv =
    SQLitePCL.Batteries_V2.Init()
    use db = ugly.``open``(":memory:")
    let s = db.query_scalar<string>("SELECT sqlite_version()")
    Console.WriteLine("{0}", s)
    0 // return an integer exit code
