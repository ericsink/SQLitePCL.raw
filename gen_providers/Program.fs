
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
    // TODO the net5min feature should probably be called function pointers, or similar
    let gen_provider provider_basename (dllimport_name:string) (subname : string option) conv kind ftr_win32dir ftr_net5min ftr_key =
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
        let args = $"-o %s{cs_path} -p:NAME=%s{provider_basename} -p:CONV=%s{conv} -p:KIND=%s{kind} -p:FEATURE_NET5MIN=%s{ftr_net5min} -p:FEATURE_WIN32DIR=%s{ftr_win32dir} -p:FEATURE_KEY=%s{ftr_key} %s{dllimport_name_arg} provider.tt"
        exec "t4" args dir_providers

    gen_provider "dynamic_cdecl" null None "Cdecl" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"
    gen_provider "dynamic_stdcall" null None "StdCall" "dynamic" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"
    gen_provider "internal" "__Internal" None "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"
    gen_provider "winsqlite3" "winsqlite3" None "StdCall" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/false"

    // for the various DllImport providers below, we generate
    // several sub-variants, which are mapped to TFMs for multi-targeting
    // by the corresponding csproj file for that provider.

    let subname_most = "most"
    let subname_net5min = "net5min"
    let subname_win = "win"

    gen_provider "e_sqlite3" "e_sqlite3" (Some subname_most) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/false" "FEATURE_KEY/false"
    gen_provider "e_sqlite3" "e_sqlite3" (Some subname_net5min) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/true" "FEATURE_KEY/false"
    gen_provider "e_sqlite3" "e_sqlite3" (Some subname_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/false"

    gen_provider "e_sqlcipher" "e_sqlcipher" (Some subname_most) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"
    gen_provider "e_sqlcipher" "e_sqlcipher" (Some subname_net5min) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/true" "FEATURE_KEY/true"
    gen_provider "e_sqlcipher" "e_sqlcipher" (Some subname_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"

    gen_provider "sqlcipher" "sqlcipher" (Some subname_most) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"
    gen_provider "sqlcipher" "sqlcipher" (Some subname_net5min) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/true" "FEATURE_KEY/true"
    gen_provider "sqlcipher" "sqlcipher" (Some subname_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/true"

    gen_provider "sqlite3" "sqlite3" (Some subname_most) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/false" "FEATURE_KEY/false"
    gen_provider "sqlite3" "sqlite3" (Some subname_net5min) "Cdecl" "dllimport" "FEATURE_WIN32DIR/false" "FEATURE_NET5MIN/true" "FEATURE_KEY/false"
    gen_provider "sqlite3" "sqlite3" (Some subname_win) "Cdecl" "dllimport" "FEATURE_WIN32DIR/true" "FEATURE_NET5MIN/false" "FEATURE_KEY/false"

    0 // return an integer exit code

