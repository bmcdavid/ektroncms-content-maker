# WSOL Ektron CMS content maker
The Ektron CMS content maker is designed to make EPiServer like views from model classes that wrap content types used for strongly typed smart forms.

## Getting Started

Add WSOL's NuGet feed as a package source in Visual Studio. Below is the feed source:
http://nuget.wsol.com/api/

Instructions for adding package sources can be found at:
https://docs.nuget.org/consume/package-manager-dialog#package-sources

## Build Instructions
Checkout this solution and install the following NuGet packages to the solution:
* WSOL.IocContainer - DLL required to run
* WSOL.MSBuild.AutoVersion.Git - required to build only, development dependendy only
* WSOL.ReferencePackages.EktronV87 - required to build only, development dependendy only
 
Build and deploy the following files to an Ektron CMS site
* WSOL.EktronCms.ContentMaker\bin\WSOL.IoCContainer.dll
* WSOL.EktronCms.ContentMaker\bin\WSOL.EktronCms.ContentMaker.dll
* WSOL.EktronCms.ContentMaker\Views\*
* WSOL.EktronCms.ContentMaker40\WSOL\* to App_Code folder
 
## Alternative
Obtain the built Nuget Package from the WSOL NuGet feed for:
WSOL.EktronCms.ContentMaker

## Code Samples
Usage examples can be found in the WSOL.EktronCms.ContentMaker.Samples project source code.
