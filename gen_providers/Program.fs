
open System
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
open System.Xml.Linq
open System.Linq

let exec = build.exec

[<EntryPoint>]
let main argv =
    let top =
        let cwd = Directory.GetCurrentDirectory()
        Path.GetFullPath(Path.Combine(cwd, ".."))
        // TODO maybe walk upward until we find the right directory

    let dir_providers = Path.Combine(top, "src", "providers")
    exec "dotnet" "restore" dir_providers

    // TODO the arg list for this function has become ridiculous
    let gen_provider provider_basename (dllimport_name:string) (subname : string option) conv kind ftr_win32dir ftr_funcptrs ftr_key ftr_loadextension =
        let dir_name = sprintf "SQLitePCLRaw.provider.%s" provider_basename
        let cs_name = 
            match subname with
            | Some subname ->
                sprintf "provider_%s_%s.cs" (provider_basename.ToLower()) (subname.ToLower())
            | None ->
                sprintf "provider_%s.cs" (provider_basename.ToLower())
        let cs_dir = Path.Combine(top, "src", dir_name, "Generated")
        Directory.CreateDirectory(cs_dir) |> ignore
        let cs_path = Path.Combine(cs_dir, cs_name)
        let dllimport_name_arg = 
            if kind = "dynamic" then "" 
            else $"-p:NAME_FOR_DLLIMPORT=%s{dllimport_name}"
        // TODO want to change this to the local tool
        let args = $"-o %s{cs_path} -p:NAME=%s{provider_basename} -p:CONV=%s{conv} -p:KIND=%s{kind} -p:FEATURE_FUNCPTRS=%s{ftr_funcptrs} -p:FEATURE_WIN32DIR=%s{ftr_win32dir} -p:FEATURE_KEY=%s{ftr_key} -p:FEATURE_LOADEXTENSION=%s{ftr_loadextension} %s{dllimport_name_arg} provider.tt"
        exec "t4" args dir_providers

    // TODO FEATURE_FUNCPTRS/plain should go away, issue #573
    gen_provider "dynamic_cdecl" null None "Cdecl" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
    gen_provider "dynamic_stdcall" null None "StdCall" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
    gen_provider "internal" "__Internal" (Some "legacy") "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
    gen_provider "internal" "__Internal" (Some "funcptrs") "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/plain" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"

    // TODO do we need prenet5 and funcptrs versions of this one?
    gen_provider "winsqlite3" "winsqlite3" None "StdCall" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"

    // for the various DllImport providers below, we generate
    // several sub-variants, which are mapped to TFMs for multi-targeting
    // by the corresponding csproj file for that provider.

    // TODO we may not need all four of the possibilities below
    let subname_prenet5_win = "prenet5_win"
    let subname_prenet5_notwin = "prenet5_notwin"
    let subname_funcptrs_win = "funcptrs_win"
    let subname_funcptrs_notwin = "funcptrs_notwin"

    for s in ["e_sqlite3"; "sqlite3"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/true"

    for s in ["e_sqlcipher"; "sqlcipher"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"

    for s in ["e_sqlite3mc"; "sqlite3mc"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"

    0 // return an integer exit code

