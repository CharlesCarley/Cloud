﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cloud.Generator.ClientSQLite {
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
    internal class Templates {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Templates() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cloud.Generator.ClientSQLite.Templates", typeof(Templates).Assembly);
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
        ///   Looks up a localized string similar to     /// &lt;summary&gt;
        ///    /// Auto generated backend implementation for the ${InterfaceName} class.
        ///    /// Do not edit directly..
        ///    /// &lt;/summary&gt;
        ///    public class ${ClassName} : ${InterfaceName} {
        ///        /// &lt;summary&gt;
        ///        /// This defines the table id for the ${ClassName} database table.
        ///        /// It is used as part of the route for the ReST server,
        ///        /// and it should be set to something unique when defining the API code. 
        ///        /// &lt;remarks&gt;
        ///        /// The server&apos;s route is defin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Class {
            get {
                return ResourceManager.GetString("Class", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to     /// &lt;summary&gt;
        ///    /// Provides the means log a message without knowing
        ///    /// a specific output method.
        ///    /// &lt;/summary&gt;
        ///    public interface ILog {
        ///        void WriteLine(string message);
        ///    }
        ///
        ///    namespace Detail {
        ///        /// &lt;summary&gt;
        ///        /// Provides support for a default logger if one is not set; goes through System.Console.
        ///        /// &lt;/summary&gt;
        ///        public class ConsoleLogger : ILog {
        ///            public void WriteLine(string message) {
        ///                LogUtils.Log(mess [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Connection {
            get {
                return ResourceManager.GetString("Connection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*
        ///-------------------------------------------------------------------------------
        ///    Copyright (c) Charles Carley.
        ///
        ///  This software is provided &apos;as-is&apos;, without any express or implied
        ///  warranty. In no event will the authors be held liable for any damages
        ///  arising from the use of this software.
        ///
        ///  Permission is granted to anyone to use this software for any purpose,
        ///  including commercial applications, and to alter it and redistribute it
        ///  freely, subject to the following restrictions:
        ///
        ///  1.  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Header {
            get {
                return ResourceManager.GetString("Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	
        ///
        ///	public class ${ClassName}Transaction {
        ///		private readonly Bundle _bundle;
        ///		
        ///		public ${ClassName}Transaction(Bundle bundle) {
        ///			_bundle = bundle;
        ///		}
        ///
        ///		public bool Save() {
        ///			var request = new Content(_bundle) {
        ///                Function = Constants.Save,
        ///                Table = _bundle.TableId,
        ///            };
        ///            Transaction.Dispatch(request, Constants.HttpPost, Constants.HttpContentApplication);
        ///            return false;
        ///		}
        ///
        ///
        ///		public static List&lt;int&gt; SelectArray() {
        ///
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Sync {
            get {
                return ResourceManager.GetString("Sync", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///		private Bundle MakePackage() {
        ///            var bundle = new Bundle {
        ///                TableId  = UniqueIdentifier,
        ///                ServerId = ServerId, 
        ///                Revision = Revision,
        ///				Key      = Key,
        ///            };
        ///            var obj = new JsonObject();
        ///			obj.AddValue(&quot;Key&quot;, Key);
        ///			${AddMembersToJObject}
        ///			bundle.Package = obj.AsCompactPrint();
        ///			return bundle;
        ///		}
        ///
        ///		public ${ClassName}Transaction CreateTransaction() {
        ///			return new ${ClassName}Transaction(MakePackage());        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Transaction {
            get {
                return ResourceManager.GetString("Transaction", resourceCulture);
            }
        }
    }
}
