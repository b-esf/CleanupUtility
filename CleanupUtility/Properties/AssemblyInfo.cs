using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CleanupUtility")]
[assembly: AssemblyDescription("Clean up Utility")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Visual Statement")]
[assembly: AssemblyProduct("CleanupUtility")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a1f63775-4e90-4d7a-b98d-d6ebda36c6f8")]

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
[assembly: AssemblyVersion("1.0.0.6")]
[assembly: AssemblyFileVersion("1.0.0.6")]

// 1.0.0.6 - Krypto - Cleaned up app.config, removed unnecessary tags that are no longer used.
//                 - Added message for invalid directories in the logbox.
// 1.0.0.5 - Krypto - Changed setting parsing to use XML instead of app.config.
//                   - Added checkbox for activating/deactivating each entry's archiving.
// 1.0.0.4 - Krypto - Fixed bug with directory index number that didn't take double digits into consideration.
//                 - Added .csv files to types that are zipped.
//                 - Changed formatting in linkbox to draw the checkboxes before the direcotry label.
//                 - Added Tooltips for checkboxes.
// 1.0.0.3 - Krypto - Changed the way info is parsed from config file.
//                  - Now puts data into custom objects instead of lists/arrays.
//                  - Objects are used for populating/saving folder information on the form.
//                  - Added error handling for missing data for each entry, in config.
// 1.0.0.2 - Krypto - Added timestamps to the log box messages.
//                 - Handled case where folder is empty and was throwing null reference error.
//                 - Modified Zip/Delete event handlers to scroll to end of logBox.
// 1.0.0.1 - Krypto - Changed sorting to only include .txt, .xml, and .log files.
//                 - Added the ability to do bimonthly archives through app.config.
//                 - Modified user messages to be more descriptive.
// 1.0.0.0 - Krypto - First version of the utility.
