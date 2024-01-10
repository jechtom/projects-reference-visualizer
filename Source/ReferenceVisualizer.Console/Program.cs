// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using ReferenceVisualizer.Core;
using ReferenceVisualizer.Core.DotNetProjects;

Console.WriteLine("Hello, World!");

var discoveryService = new DotNetProjectsDiscoveryService()
{
    FolderPath = @"c:\cbData\CMS\"
};

var progress = new Progress<DiscoveryProgress>();
progress.ProgressChanged += (object? sender, DiscoveryProgress e) =>
{
    if (e.ErrorMessage != null)
    {
        Console.WriteLine($"[WARN] {e.CurrentItem}: {e.ErrorMessage}");
    }
};

DotNetProjectsData data = discoveryService.Discover(new CancellationToken(), progress);


Console.WriteLine("Done.");