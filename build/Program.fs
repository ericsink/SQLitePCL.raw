
open System
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
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

    exec "dotnet" "run .." (Path.Combine(top, "version_stamp"))

    exec "dotnet" "run .." (Path.Combine(top, "gen_lib_nuspecs"))
    exec "dotnet" "run .." (Path.Combine(top, "gen_bundle_nuspecs"))
    exec "dotnet" "run" (Path.Combine(top, "gen_providers"))

    let dir_nupkgs = Path.Combine(top, "nupkgs")
    Directory.CreateDirectory(dir_nupkgs) |> ignore
    for s in Directory.GetFiles(dir_nupkgs, "*.nupkg") do
        File.Delete(s)

    let just_build_dirs = [
        "SQLitePCLRaw.nativelibrary" 
    ]
    for s in just_build_dirs do
        exec "dotnet" "build -c Release" (Path.Combine(top, "src", s))

    let pack_dirs = [
        "SQLitePCLRaw.core"
        "SQLitePCLRaw.ugly" 
        "SQLitePCLRaw.provider.dynamic_cdecl" 
        "SQLitePCLRaw.provider.dynamic_stdcall" 
        "SQLitePCLRaw.provider.internal" 
        "SQLitePCLRaw.provider.winsqlite3" 
        "SQLitePCLRaw.provider.e_sqlite3" 
        "SQLitePCLRaw.provider.e_sqlcipher" 
        "SQLitePCLRaw.provider.sqlite3" 
        "SQLitePCLRaw.provider.sqlcipher" 
    ]
    for s in pack_dirs do
        exec "dotnet" "pack -c Release" (Path.Combine(top, "src", s))

    let batteries_dirs = [
        "e_sqlite3.dllimport"
        "e_sqlite3.dynamic"
        "e_sqlcipher.dllimport"
        "e_sqlcipher.dynamic"
        "sqlite3.dllimport"
        "sqlite3.dynamic"
        "sqlcipher.dynamic"
        "sqlcipher.dllimport"
        "winsqlite3.dllimport"
        "winsqlite3.dynamic"
        ]
    for s in batteries_dirs do
        let dir_name = sprintf "SQLitePCLRaw.batteries_v2.%s" s
        exec "dotnet" "build -c Release" (Path.Combine(top, "src", dir_name))

    let msbuild_dirs = [
        "lib.sqlcipher.ios.placeholder"
        "batteries_v2.e_sqlite3.internal.ios"
        "batteries_v2.e_sqlite3.internal.tvos"
        "batteries_v2.e_sqlcipher.internal.ios"
        "batteries_v2.sqlcipher.internal.ios"
        ]
    for s in msbuild_dirs do
        let dir_name = sprintf "SQLitePCLRaw.%s" s
        let dir = (Path.Combine(top, "src", dir_name))
        //exec "dotnet" "restore" dir
        //exec "msbuild" "/p:Configuration=Release" dir
        exec "dotnet" "build -c Release" dir

    let msbuild_pack_dirs = [
        "lib.e_sqlite3.android"
        "lib.e_sqlite3.ios"
        "lib.e_sqlite3.tvos"
        "lib.e_sqlcipher.android"
        "lib.e_sqlcipher.ios"
        ]
    for s in msbuild_pack_dirs do
        let dir_name = sprintf "SQLitePCLRaw.%s" s
        let dir = (Path.Combine(top, "src", dir_name))
        //exec "dotnet" "restore" dir
        //exec "msbuild" "/p:Configuration=Release /t:pack" dir
        exec "dotnet" "pack -c Release" dir

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
        "bundle_sqlite3"
        ]
    for s in nuspecs do
        let name = sprintf "SQLitePCLRaw.%s" s
        let dir_proj = Path.Combine(top, "src", name)
        let path_empty = Path.Combine(dir_proj, "_._")
        if not (File.Exists(path_empty)) then
            File.WriteAllText(path_empty, "")
        exec "dotnet" "pack" dir_proj

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "smoke"))

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "fsmoke"))

    let real_xunit_dirs = [
        yield "e_sqlite3"
        yield "e_sqlcipher"
        // TODO do bundle_sqlite3 real_xunit here?
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "winsqlite3"
        ]

    let fake_xunit_tfms = [
        yield "netcoreapp3.1"
        yield "net6.0"
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "net461"
        ]

    let fake_xunit_dirs = [
        yield "e_sqlite3"
        yield "e_sqlcipher"
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "winsqlite3"
        yield "sqlite3"
        ]

    for tfm in fake_xunit_tfms do
        for dir in real_xunit_dirs do
            let args = sprintf "test --framework=%s" tfm
            exec "dotnet" args (Path.Combine(top, "test_nupkgs", dir, "real_xunit"))
        for dir in fake_xunit_dirs do
            let args = sprintf "run --framework=%s" tfm
            exec "dotnet" args (Path.Combine(top, "test_nupkgs", dir, "fake_xunit"))

    printfn "generating push.bat"
    let bat = System.Collections.Generic.List<string>()
    for s in Directory.GetFiles(dir_nupkgs, "*.nupkg") do
        let fname = Path.GetFileName(s)
        let line = sprintf ".\\nuget push %s -Source https://api.nuget.org/v3/index.json -ApiKey %%1" fname
        bat.Add(line)
    File.WriteAllLines(Path.Combine(dir_nupkgs, "push.bat"), bat)

    timer.Stop()
    printfn "Total build time: %A milliseconds" timer.ElapsedMilliseconds

    0 // return an integer exit code

