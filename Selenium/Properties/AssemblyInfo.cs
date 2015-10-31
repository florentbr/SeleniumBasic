using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Selenium.Internal;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Selenium .Net/COM binding")]
[assembly: AssemblyDescription("Selenium Type Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Florent BREHERET")]
[assembly: AssemblyProduct("SeleniumBasic")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyURL(@"https://github.com/florentbr/SeleniumBasic")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0277FC34-FD1B-4616-BB19-A9AABCAF2A70")]

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
[assembly: AssemblyVersion("2.0.6.0")]
[assembly: AssemblyFileVersion("2.0.6.0")]
[assembly: TypeLibVersion(2, 0)]

[assembly: NeutralResourcesLanguage("en-US")]

#if Test
[assembly: InternalsVisibleTo("Selenium.Tests")]
#endif

