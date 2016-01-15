
/*
*  Warewolf - The Easy Service Bus
*  Copyright 2016 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dev2.Runtime.Configuration.Properties {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class Resources {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("Dev2.Runtime.Configuration.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid FilterMode enumeration value. The value must be one of the defined AutoCompleteFilterMode values to be accepted..
        /// </summary>
        internal static string AutoComplete_OnFilterModePropertyChanged_InvalidValue {
            get {
                return ResourceManager.GetString("AutoComplete_OnFilterModePropertyChanged_InvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid maximum drop down height value &apos;{0}&apos;. The value must be greater than or equal to zero..
        /// </summary>
        internal static string AutoComplete_OnMaxDropDownHeightPropertyChanged_InvalidValue {
            get {
                return ResourceManager.GetString("AutoComplete_OnMaxDropDownHeightPropertyChanged_InvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid MinimumPopulateDelay value &apos;{0}&apos;. The value must be greater than or equal to zero..
        /// </summary>
        internal static string AutoComplete_OnMinimumPopulateDelayPropertyChanged_InvalidValue {
            get {
                return ResourceManager.GetString("AutoComplete_OnMinimumPopulateDelayPropertyChanged_InvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set read-only property SearchText..
        /// </summary>
        internal static string AutoComplete_OnSearchTextPropertyChanged_InvalidWrite {
            get {
                return ResourceManager.GetString("AutoComplete_OnSearchTextPropertyChanged_InvalidWrite", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The RoutedPropertyChangingEvent cannot be canceled..
        /// </summary>
        internal static string RoutedPropertyChangingEventArgs_CancelSet_InvalidOperation {
            get {
                return ResourceManager.GetString("RoutedPropertyChangingEventArgs_CancelSet_InvalidOperation", resourceCulture);
            }
        }
    }
}
