---
name: Bug report
about: Report a problem
title: ''
labels: ''
assignees: ''

---

SQLitePCLRaw handles many platforms and configurations.  If you are having trouble, we typically need a LOT of information from you in order to help figure out what is going wrong.

The following questions are provided to help you see what kinds of information we need:

What version of SQLitePCLRaw are you using?

If you are using one of the SQLitePCLRaw bundle packages, which one?

What platform are you running on?  What operating system?  Which version?  What CPU?

What target framework are you building for?

Are you on .NET Framework or the newer stuff (.NET Core, .NET 5+, etc)?

Are you using the command line, or an IDE?  Which IDE?  Which version of that IDE?

Is this problem something that just started happening?  For example, were things working well for you and then you updated something and then the problem showed up?  What changed?

What is the exact error message you are seeing when the problem happens?

Are you using `PackageReference` or `packages.config`?

If you are using mobile platforms, are you on classic/legacy Xamarin or on .NET 6 and higher?

Sometimes other packages using SQLitePCLRaw cause problems when they are mixed together.  What other packages are you including in your project?

How can we reproduce the problem you are seeing?  Your issue will get attention much faster if you attach a minimal reproduction sample project.
