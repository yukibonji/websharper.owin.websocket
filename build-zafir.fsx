System.Environment.SetEnvironmentVariable("INTELLIFACTORY", "")
#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Owin.WebSocket")
        .VersionFrom("WebSharper")
        .WithFSharpVersion(FSharpVersion.FSharp31)
        .WithFramework(fun fw -> fw.Net45)

open System.IO

let MPServiceLocation =
    Path.Combine(__SOURCE_DIRECTORY__,
        @"packages\CommonServiceLocator.1.3\lib\portable-net4+sl5+netcore45+wpa81+wp8\Microsoft.Practices.ServiceLocation.dll")

let main =
    bt.WebSharper4.Library("WebSharper.Owin.WebSocket")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.NuGet("Owin").ForceFoundVersion().Reference()
                r.NuGet("Microsoft.Owin").ForceFoundVersion().Reference()
                r.NuGet("Owin.WebSocket").Version("1.6.0").Reference()
                r.NuGet("CommonServiceLocator").ForceFoundVersion().Reference()
                r.NuGet("WebSharper.Owin").Latest(true).ForceFoundVersion().Reference()
                r.File(MPServiceLocation)
                r.Assembly("System.Configuration")
                r.Assembly "System.Web"
            ])

let test =
    bt.WebSharper4.Executable("WebSharper.Owin.WebSocket.Test")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.NuGet("Owin").Reference().CopyLocal()
                r.NuGet("Microsoft.Owin").Reference().CopyLocal()
                r.NuGet("Owin.WebSocket").Version("1.6.0").Reference().CopyLocal()
                r.NuGet("WebSharper.Html").Latest(true).Reference().CopyLocal()
                r.NuGet("WebSharper.Owin").Latest(true).Reference().CopyLocal()
                r.NuGet("Microsoft.Owin.Hosting").Reference().CopyLocal()
                r.NuGet("Microsoft.Owin.StaticFiles").Reference().CopyLocal()
                r.NuGet("Microsoft.Owin.FileSystems").Reference().CopyLocal()
                r.NuGet("Microsoft.Owin.Host.HttpListener").Reference().CopyLocal()
                r.NuGet("Microsoft.Owin.Diagnostics").Reference().CopyLocal()
                r.Project(main).CopyLocal()
                r.File(MPServiceLocation).CopyLocal()
                r.Assembly("System.Configuration")
                r.Assembly("System.Web")
            ])
    |> FSharpConfig.OutputPath.Custom
        (__SOURCE_DIRECTORY__ + "/WebSharper.Owin.WebSocket.Test/bin/WebSharper.Owin.WebSocket.Test.exe")

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper.Owin.WebSocket"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.owin.websocket"
                Description = "WebSocket support for WebSharper with Owin 1.0"
                RequiresLicenseAcceptance = true })
        .AddFile("build/net45/Owin.WebSocket.dll","lib/net45/Owin.WebSocket.dll")
        .Add(main)
]
|> bt.Dispatch
