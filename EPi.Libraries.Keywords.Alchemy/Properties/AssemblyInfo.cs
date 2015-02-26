﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using EPiServer.Logging;

[assembly: AssemblyTitle("EPi.Libraries.Keywords.Alchemy")]
[assembly: AssemblyDescription("Alchemy provider for extracting keywords from text.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jeroen Stemerdink")]
[assembly: AssemblyProduct("EPi.Libraries.Keywords.Alchemy")]
[assembly: AssemblyCopyright("Copyright © Jeroen Stemerdink 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2378cd81-b5bc-4826-80e0-74d2ed1febd4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

[assembly: LoggerFactory(typeof(EPiServer.Logging.Log4Net.Log4NetLoggerFactory))]
