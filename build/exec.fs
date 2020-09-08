
module build
    open System
    open System.Diagnostics

    let exec filename args startDir = 
        let wd = System.IO.Path.GetFullPath(startDir)
        let timer = Stopwatch.StartNew()
        let procStartInfo = 
            ProcessStartInfo(
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = filename,
                Arguments = args,
                WorkingDirectory = wd
            )

        let ProcessOutput (e : DataReceivedEventArgs) = System.Console.WriteLine(e.Data)
        let ProcessError (e : DataReceivedEventArgs) = System.Console.Error.WriteLine(e.Data)

        let p = new Process(StartInfo = procStartInfo)
        p.OutputDataReceived.Add(ProcessOutput)
        p.ErrorDataReceived.Add(ProcessError)

        let desc = sprintf "%s %s in %s" filename args wd
        printfn "-------- %s" desc
        p.Start() |> ignore
        //printfn "Started %s with pid %i" p.ProcessName p.Id
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        timer.Stop()
        printfn "Finished %s after %A milliseconds" desc timer.ElapsedMilliseconds
        let rc = p.ExitCode
        if rc <> 0 then raise(Exception())
        ()

