﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ResourceCompiler {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ResourceCompiler.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace ${Namespace}
        ///{
        ///	public static class ${ClassName}
        ///	{
        ///		${PrivateImpl}
        ///
        ///		${PublicImpl}
        ///
        ///		private static string GetString(byte []bytes)
        ///		{
        ///			var builder = new StringBuilder();
        ///			foreach(byte b in bytes) 
        ///				builder.Append((char)b);
        ///			return builder.ToString();
        ///		}
        ///	}
        ///}
        ///.
        /// </summary>
        internal static string Main {
            get {
                return ResourceManager.GetString("Main", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to         private static readonly byte[] _${BaseName} = new byte[${Count}] {
        ///            ${Bytes}
        ///        };
        ///.
        /// </summary>
        internal static string PrivateImpl {
            get {
                return ResourceManager.GetString("PrivateImpl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ResourceCompiler &lt;options&gt; -o &lt;output file&gt; source[0], ... , source[N] 
        ///
        ///    where &lt;options&gt; is one or more of the following:
        ///
        ///         -n - Sets the output namespace name.
        ///         -w - Sets the width of the output byte array.
        ///.
        /// </summary>
        internal static string Usage {
            get {
                return ResourceManager.GetString("Usage", resourceCulture);
            }
        }
    }
}
