
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

    // FEATURE_FUNCPTRS false is for platforms older than .NET 5, like .NET Framework
    // dynamic (vs dllimport) is only for .NET Framework, and is needed only for AnyCpu

    // we use win vs notwin because of sqlite3_win32_set_directory8, but do we need
    // this to be a t4-level flag now that UWP is deprecated?  similar questions for
    // load extension and sqlite3_key.  TODO  will NativeAOT have the same kind of
    // linker problems that UWP did?

    gen_provider "dynamic_cdecl" null None "Cdecl" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
    gen_provider "dynamic_stdcall" null None "StdCall" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
    gen_provider "internal" "__Internal" (Some "funcptrs") "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"

    gen_provider "winsqlite3" "winsqlite3" None "StdCall" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"

    // for the various DllImport providers below, we generate
    // several sub-variants, which are mapped to TFMs for multi-targeting
    // by the corresponding csproj file for that provider.

    // TODO we may not need all four of the possibilities below
    let subname_prenet5_win = "prenet5_win"
    let subname_prenet5_notwin = "prenet5_notwin"
    let subname_funcptrs_win = "funcptrs_win"
    let subname_funcptrs_notwin = "funcptrs_notwin"

    // the e_sqlite3 provider does include FEATURE_KEY, which means it can be used for
    // crypto-enabled builds that have the name e_sqlite3.
    for s in ["e_sqlite3"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"

    // the sqlite3 provider is intended for use with the system sqlite like on Linux or iOS.
    // no FEATURE_KEY
    for s in ["sqlite3"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/false" "FEATURE_LOADEXTENSION/true"

    // the sqlcipher provider is for use with Zetetic's official SQLCipher builds
    for s in ["sqlcipher"; ] do
        gen_provider s s (Some subname_prenet5_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_prenet5_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/false" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/false"
        gen_provider s s (Some subname_funcptrs_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"
        gen_provider s s (Some subname_funcptrs_notwin) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_FUNCPTRS/callingconv" "FEATURE_KEY/true" "FEATURE_LOADEXTENSION/true"

    0 // return an integer exit code

