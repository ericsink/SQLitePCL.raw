
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

    exec "dotnet" "run .." (Path.Combine(top, "gen_nuspecs"))

    let dir_nupkgs = Path.Combine(top, "nupkgs")
    Directory.CreateDirectory(dir_nupkgs) |> ignore
    for s in Directory.GetFiles(dir_nupkgs, "*.nupkg") do
        File.Delete(s)

    let dir_providers = Path.Combine(top, "src", "providers")
    exec "dotnet" "restore" dir_providers

    // TODO the arg list for this function has become ridiculous
    let gen_provider dir_basename (dllimport_name:string) (provider_basename:string) conv kind tfm ftr_key =
        let dir_name = sprintf "SQLitePCLRaw.provider.%s" dir_basename
        let cs_name = sprintf "provider_%s.cs" (provider_basename.ToLower())
        let cs_path = Path.Combine(top, "src", dir_name, "Generated", cs_name)
        let dllimport_name_arg = 
            if kind = "dynamic" 
            then "" 
            else sprintf "-p:NAME_FOR_DLLIMPORT=%s" dllimport_name
        // TODO want to change this to the local tool
        let args = sprintf "-o %s -p:NAME=%s -p:CONV=%s -p:KIND=%s -p:TFM=%s -p:FEATURE_KEY=%s %s provider.tt" cs_path provider_basename conv kind tfm ftr_key dllimport_name_arg
        exec "t4" args dir_providers

    gen_provider "dynamic_cdecl" null "dynamic_cdecl" "Cdecl" "dynamic" "netstandard2.0" "true"
    gen_provider "dynamic_stdcall" null "dynamic_stdcall" "StdCall" "dynamic" "netstandard2.0" "true"

    gen_provider "e_sqlite3.most" "e_sqlite3" "e_sqlite3" "Cdecl" "dllimport" "netstandard2.0" "false"
    gen_provider "e_sqlite3.net5.0" "e_sqlite3" "e_sqlite3" "Cdecl" "dllimport" "net5.0" "false"
    gen_provider "e_sqlite3.uwp" "e_sqlite3" "e_sqlite3" "Cdecl" "dllimport" "uap10.0" "false"

    gen_provider "e_sqlcipher.most" "e_sqlcipher" "e_sqlcipher" "Cdecl" "dllimport" "netstandard2.0" "true"
    gen_provider "e_sqlcipher.net5.0" "e_sqlcipher" "e_sqlcipher" "Cdecl" "dllimport" "net5.0" "true"
    gen_provider "e_sqlcipher.uwp" "e_sqlcipher" "e_sqlcipher" "Cdecl" "dllimport" "uap10.0" "true"

    gen_provider "sqlcipher.most" "sqlcipher" "sqlcipher" "Cdecl" "dllimport" "netstandard2.0" "true"
    gen_provider "sqlcipher.net5.0" "sqlcipher" "sqlcipher" "Cdecl" "dllimport" "net5.0" "true"
    gen_provider "sqlcipher.uwp" "sqlcipher" "sqlcipher" "Cdecl" "dllimport" "uap10.0" "true"

    gen_provider "sqlite3.most" "sqlite3" "sqlite3" "Cdecl" "dllimport" "netstandard2.0" "false"
    gen_provider "sqlite3.net5.0" "sqlite3" "sqlite3" "Cdecl" "dllimport" "net5.0" "false"
    gen_provider "sqlite3.uwp" "sqlite3" "sqlite3" "Cdecl" "dllimport" "uap10.0" "false"

    gen_provider "internal" "__Internal" "internal" "Cdecl" "dllimport" "netstandard2.0" "true"

    gen_provider "winsqlite3" "winsqlite3" "winsqlite3" "StdCall" "dllimport" "uap10.0" "false" // TODO why was uwp here?

    let just_build_dirs = [
        "SQLitePCLRaw.nativelibrary" 

        "SQLitePCLRaw.provider.e_sqlite3.most" 
        "SQLitePCLRaw.provider.e_sqlcipher.most" 
        "SQLitePCLRaw.provider.sqlcipher.most" 
        "SQLitePCLRaw.provider.sqlite3.most" 

        "SQLitePCLRaw.provider.e_sqlite3.net5.0" 
        "SQLitePCLRaw.provider.e_sqlcipher.net5.0" 
        "SQLitePCLRaw.provider.sqlcipher.net5.0" 
        "SQLitePCLRaw.provider.sqlite3.net5.0" 

        "SQLitePCLRaw.provider.e_sqlite3.uwp" 
        "SQLitePCLRaw.provider.e_sqlcipher.uwp" 
        "SQLitePCLRaw.provider.sqlcipher.uwp" 
        "SQLitePCLRaw.provider.sqlite3.uwp" 
    ]
    for s in just_build_dirs do
        exec "dotnet" "build -c Release" (Path.Combine(top, "src", s))

    let pack_dirs = [
        "SQLitePCLRaw.core"
        "SQLitePCLRaw.ugly" 
        "SQLitePCLRaw.provider.dynamic_cdecl" 
        "SQLitePCLRaw.provider.dynamic_stdcall" 
        "SQLitePCLRaw.provider.internal" 
        // "SQLitePCLRaw.provider.sqlite3" 
        "SQLitePCLRaw.provider.winsqlite3" 
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
        "provider.e_sqlite3"
        "provider.e_sqlcipher"
        "provider.sqlcipher"
        "provider.sqlite3"
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

    //exec "dotnet" (sprintf "run --framework=%s" "net5.0") (Path.Combine(top, "test_nupkgs", "cil", "fake_xunit"))

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

