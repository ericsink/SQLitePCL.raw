
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

    exec "dotnet" "run" (Path.Combine(top, "gen_providers"))

    let dir_nupkgs = Path.Combine(top, "nupkgs")
    Directory.CreateDirectory(dir_nupkgs) |> ignore
    for s in Directory.GetFiles(dir_nupkgs, "*.nupkg") do
        File.Delete(s)

    let pack_dirs = [
        "core"
        "ugly" 
        "provider.dynamic_cdecl" 
        "provider.dynamic_stdcall" 
        "provider.internal" 
        "provider.winsqlite3" 
        "provider.e_sqlite3" 
        "provider.sqlite3" 
        "provider.sqlcipher" 
        "config.e_sqlite3"
        "bundle_e_sqlite3"
    ]
    for s in pack_dirs do
        let dir_name = sprintf "SQLitePCLRaw.%s" s
        exec "dotnet" "pack -c Release" (Path.Combine(top, "src", dir_name))

    let get_build_prop p =
        let path_xml = Path.Combine(top, "Directory.Build.props")
        let xml = XElement.Load(path_xml);
        let props = xml.Elements(XName.Get "PropertyGroup").First()
        let ver = props.Elements(XName.Get p).First()
        ver.Value

    let version = get_build_prop "Version"

    printfn "%s" version

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "smoke"))

    exec "dotnet" "run" (Path.Combine(top, "test_nupkgs", "fsmoke"))

    let real_xunit_dirs = [
        yield "e_sqlite3"
        //yield "e_sqlite3mc"
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "winsqlite3"
        ]

    let fake_xunit_tfms = [
        yield "net8.0"
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "net471"
        ]

    let fake_xunit_dirs = [
        yield "e_sqlite3"
        //yield "e_sqlite3mc"
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then yield "winsqlite3"
        ]

    for tfm in fake_xunit_tfms do
        for dir in real_xunit_dirs do
            let args = sprintf "test --framework=%s" tfm
            exec "dotnet" args (Path.Combine(top, "test_nupkgs", dir, "real_xunit"))
        for dir in fake_xunit_dirs do
            let args = sprintf "run --framework=%s" tfm
            exec "dotnet" args (Path.Combine(top, "test_nupkgs", dir, "fake_xunit"))

#if not // TODO currently fails in the GitHub Action, not yet sure why
    if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
        let args = "run -f net6.0-windows -r win-x86 --no-self-contained"
        exec "dotnet" args (Path.Combine(top, "test_nupkgs", "e_sqlite3", "fake_xunit"))
        exec "dotnet" args (Path.Combine(top, "test_nupkgs", "e_sqlite3mc", "fake_xunit"))
#endif

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

