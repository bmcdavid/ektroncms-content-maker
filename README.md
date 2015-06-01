# WSOL Ektron CMS content maker
The Ektron CMS content maker is designed to make EPiServer like views from model classes that wrap content types used for strongly typed smart forms.

## Getting Started
Add WSOL's NuGet feed as a package source in Visual Studio. Below is the feed source:

http://nuget.wsol.com/api/

Instructions for adding package sources can be found at:

https://docs.nuget.org/consume/package-manager-dialog#package-sources

## Build Instructions
Checkout this solution and install the following NuGet packages:
* WSOL.IocContainer - DLL required to run
* WSOL.MSBuild.AutoVersion.Git - required to build only, development dependency only
* WSOL.ReferencePackages.EktronV87 - required to build only, development dependency only
 
Build and deploy the following files to an Ektron CMS site
* WSOL.EktronCms.ContentMaker\bin\WSOL.IoCContainer.dll
* WSOL.EktronCms.ContentMaker\bin\WSOL.EktronCms.ContentMaker.dll
* WSOL.EktronCms.ContentMaker\Views\*
* WSOL.EktronCms.ContentMaker40\WSOL\* to App_Code folder
 
## Or NuGet Install
Alternatively, obtain the built Nuget package from the WSOL NuGet feed for:

WSOL.EktronCms.ContentMaker

## Code Samples
Usage examples can be found in the WSOL.EktronCms.ContentMaker.Samples project source code.

* [Content Type created by the XSD utility](https://github.com/bmcdavid/ektroncms-content-maker/blob/master/WSOL.EktronCms.ContentMaker.Samples/WSOL/Custom/ContentMaker/Samples/ContentTypes/ArticleContent.designer.cs)
* [Content Model](https://github.com/bmcdavid/ektroncms-content-maker/blob/master/WSOL.EktronCms.ContentMaker.Samples/WSOL/Custom/ContentMaker/Samples/Models/ArticleContent.cs)
* [Content Views](https://github.com/bmcdavid/ektroncms-content-maker/tree/master/WSOL.EktronCms.ContentMaker.Samples/Views/ArticleContent)
* [Content Renderer Web Control](https://github.com/bmcdavid/ektroncms-content-maker/blob/master/WSOL.EktronCms.ContentMaker.Samples/ContentRenderSamples.aspx)
* [Content Renderer Code Behind](https://github.com/bmcdavid/ektroncms-content-maker/blob/master/WSOL.EktronCms.ContentMaker.Samples/ContentRenderSamples.aspx.cs)
* [C# Code Extension Usage](https://github.com/bmcdavid/ektroncms-content-maker/blob/master/WSOL.EktronCms.ContentMaker.Samples/CodeSamples.cs)
