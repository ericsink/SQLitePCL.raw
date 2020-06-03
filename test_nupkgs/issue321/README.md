
This is a repro project for #321

It was originally provided by @jay1891

I have removed WPF, changed it to a command-line exe, and converted it to an SDK-style csproj with PackageReference.

By modifying the target framework in the csproj, it is easy to change back and forth between targeting netcore3.1 and net461.

When targeting netcore3.1, I have run the test several times and never seen it fail.

When targeting net461, I have run the test several times and never seen it succeed.

