
open System
open System.Diagnostics
open System.IO
open System.Xml.Linq
open System.Linq

let exec = build.exec

[<EntryPoint>]
let main argv =
    let timer = Stopwatch.StartNew()

    let top =
        let cwd = Directory.GetCurrentDirectory()
        Path.GetFullPath(Path.Combine(cwd, ".."))
        // TODO maybe walk upward until we find the right directory

    exec "dotnet" "run .." (Path.Combine(top, "gen_nuspecs"))

    let dir_nupkgs = Path.Combine(top, "nupkgs")
    Directory.CreateDirectory(dir_nupkgs)
    for s in Directory.GetFiles(dir_nupkgs, "*.nupkg") do
        File.Delete(s)

    let dir_gen_providers = Path.Combine(top, "gen_providers")
    exec "dotnet" "restore" dir_gen_providers

    let gen_provider dir_basename (name:string) conv kind =
        let path_tt = Path.Combine(top, "src", "common", "provider.tt")
        let dir_name = sprintf "SQLitePCLRaw.provider.%s" dir_basename
        let cs_name = sprintf "provider_%s.cs" (name.ToLower())
        let cs_path = Path.Combine(top, "src", dir_name, "Generated", cs_name)
        let dllimport_name_arg = 
            if kind = "dynamic" 
            then "" 
            else 
                if name = "internal"
                then (sprintf "-p:NAME_FOR_DLLIMPORT=%s" "__Internal")
                else (sprintf "-p:NAME_FOR_DLLIMPORT=%s" name)
        // TODO want to change this to the local tool
        let args = sprintf "-o %s -p:NAME=%s -p:CONV=%s -p:KIND=%s %s %s" cs_path name conv kind dllimport_name_arg path_tt
        exec "t4" args dir_gen_providers

    gen_provider "dynamic" "Cdecl" "Cdecl" "dynamic"
    gen_provider "dynamic" "StdCall" "StdCall" "dynamic"
    gen_provider "e_sqlite3" "e_sqlite3" "Cdecl" "dllimport"
    gen_provider "e_sqlcipher" "e_sqlcipher" "Cdecl" "dllimport"
    gen_provider "sqlite3" "sqlite3" "Cdecl" "dllimport"
    gen_provider "sqlcipher" "sqlcipher" "Cdecl" "dllimport"
    gen_provider "winsqlite3" "winsqlite3" "StdCall" "dllimport"
    gen_provider "internal" "internal" "Cdecl" "dllimport"

    let pack_dirs = [
        "SQLitePCLRaw.core"
        "SQLitePCLRaw.ugly" 
        "SQLitePCLRaw.impl.callbacks" 
        "SQLitePCLRaw.provider.dynamic" 
        "SQLitePCLRaw.provider.internal" 
        "SQLitePCLRaw.provider.e_sqlite3" 
        "SQLitePCLRaw.provider.e_sqlcipher" 
        "SQLitePCLRaw.provider.sqlite3" 
        "SQLitePCLRaw.provider.sqlcipher" 
        "SQLitePCLRaw.provider.winsqlite3" 
    ]
    for s in pack_dirs do
        exec "dotnet" "pack -c Release" (Path.Combine(top, "src", s))

    let batteries_dirs = [
        "e_sqlite3.dllimport"
        "e_sqlite3.dynamic"
        "e_sqlcipher.dllimport"
        "e_sqlcipher.dynamic"
        "sqlite3"
        "sqlcipher.dynamic"
        "sqlcipher.dllimport"
        "winsqlite3"
        ]
    for s in batteries_dirs do
        let dir_name = sprintf "SQLitePCLRaw.batteries_v2.%s" s
        exec "dotnet" "build -c Release" (Path.Combine(top, "src", dir_name))

    let msbuild_dirs = [
        "lib.e_sqlite3.android"
        "lib.e_sqlite3.ios"
        "lib.e_sqlcipher.android"
        "lib.e_sqlcipher.ios"
        "lib.sqlcipher.ios.placeholder"
        "batteries_v2.e_sqlite3.internal.ios"
        "batteries_v2.e_sqlcipher.internal.ios"
        "batteries_v2.sqlcipher.internal.ios"
        ]
    for s in msbuild_dirs do
        let dir_name = sprintf "SQLitePCLRaw.%s" s
        let dir = (Path.Combine(top, "src", dir_name))
        exec "dotnet" "restore" dir
        exec "msbuild" "/p:Configuration=Release" dir

    let get_build_prop p =
        let path_xml = Path.Combine(top, "Directory.Build.props")
        let xml = XElement.Load(path_xml);
        let props = xml.Elements(XName.Get "PropertyGroup").First()
        let ver = props.Elements(XName.Get p).First()
        ver.Value

    let version = get_build_prop "Version"

    printfn "%s" version

    let nuspecs = [
        "lib.e_sqlite3"
        "lib.e_sqlcipher"
        "bundle_green"
        "bundle_e_sqlite3"
        "bundle_e_sqlcipher"
        "bundle_zetetic"
        "bundle_winsqlite3"
        ]
    for s in nuspecs do
        let name = sprintf "SQLitePCLRaw.%s" s
        let dir_proj = Path.Combine(top, "src", name)
        Directory.CreateDirectory(Path.Combine(dir_proj, "empty"))
        exec "dotnet" "pack" dir_proj

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "smoke"))

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "fsmoke"))

    exec "dotnet" "test" (Path.Combine(top, "test_nupkgs", "with_xunit"))

    timer.Stop()
    printfn "Total build time: %A milliseconds" timer.ElapsedMilliseconds

    0 // return an integer exit code

