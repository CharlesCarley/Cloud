/*
-------------------------------------------------------------------------------
    Copyright (c) Charles Carley.

  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
-------------------------------------------------------------------------------
*/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cloud.Common;
using Cloud.GeneratorApi;
using Cloud.ReflectionApi;

namespace Cloud.Generator.ClientSQLite
{
    internal static class Parameters {
        public const string HeaderUsing               = "${Using}";
        public const string HeaderProjectNamespace    = "${ProjectNamespace}";
        public const string HeaderContent             = "${Content}";
        public const string ContentRegisterTypes      = "${RegisterTypes}";
        public const string ContentClearTypes         = "${ClearTypes}";
        public const string ClassClassName            = "${ClassName}";
        public const string ClassInterfaceName        = "${InterfaceName}";
        public const string ClassUniqueId             = "${UniqueId}";
        public const string ClassPropertyDeclarations = "${InterfaceDeclarations}";
        public const string ClassGetters              = "${Getters}";
        public const string ClassSetters              = "${Setters}";
        public const string ClassTransactionImpl      = "${TransactionImpl}";
        public const string TransAddMembersToJObject  = "${AddMembersToJObject}";
        public const string ClassSyncImpl             = "${SyncImpl}";
        public const string EOL                       = "\r\n";
    }

    internal static class Builders {
        public static StringBuilder Output { get; }
        public static StringBuilder Content { get; }
        public static StringBuilder Registration { get; }
        public static StringBuilder Classes { get; }
        public static StringBuilder Decelerations { get; }

        static Builders()
        {
            Output        = new StringBuilder();
            Content       = new StringBuilder();
            Registration  = new StringBuilder();
            Classes       = new StringBuilder();
            Decelerations = new StringBuilder();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ClientSQLiteGenerator : IStoreGenerator {
        private StoreItemManager Manager { get; set; }
        private string           Output { get; set; }

        private bool GenerateImpl()
        {
            Builders.Output.Clear();

            var headerString = Templates.Header;
            var needsExtra   = Manager.Items.Any(item => item.CanSynchronize);
            Builders.Content.Clear();

            if (needsExtra) {
                Builders.Content.Append("using Cloud.Transaction;");
                Builders.Content.Append(Parameters.EOL);
                Builders.Content.Append("using Cloud.Common;");
                Builders.Content.Append(Parameters.EOL);
            }

            // make sure that the defining API is visible
            foreach (var ns in Manager.UsedNamespaces) {
                // exclude it if its the same as the destination.
                if (Manager.Namespace.Equals(ns)) continue;

                Builders.Content.Append($"using {ns};");
                Builders.Content.Append(Parameters.EOL);
            }

            headerString = headerString.Replace(Parameters.HeaderUsing, Builders.Content.ToString());
            headerString = headerString.Replace(Parameters.HeaderProjectNamespace, Manager.Namespace);

            Builders.Content.Clear();
            GenerateContent();
            headerString = headerString.Replace(Parameters.HeaderContent, Builders.Content.ToString());

            Builders.Output.Append(headerString);
            File.WriteAllText(Output, Builders.Output.ToString());
            return true;
        }

        private void GenerateClasses()
        {
            foreach (var item in Manager.Items) {
                var classTemplate = Templates.Class;

                Builders.Decelerations.Clear();

                classTemplate = classTemplate.Replace(Parameters.ClassClassName, item.UserName);
                classTemplate = classTemplate.Replace(Parameters.ClassInterfaceName, item.InterfaceName);
                classTemplate = classTemplate.Replace(Parameters.ClassGetters, string.Empty);
                classTemplate = classTemplate.Replace(Parameters.ClassSetters, string.Empty);
                classTemplate = classTemplate.Replace(Parameters.ClassUniqueId, item.UniqueID.ToString(CultureInfo.InvariantCulture));

                classTemplate = classTemplate.Replace(Parameters.ClassPropertyDeclarations, Builders.Decelerations.ToString());

                // 
                Builders.Decelerations.Clear();
                foreach (var property in item.Properties)
                    BuildJsonObjectSetter(Builders.Decelerations, property);

                classTemplate = classTemplate.Replace(Parameters.TransAddMembersToJObject, Builders.Decelerations.ToString());

                if (!item.CanSynchronize) {
                    classTemplate = classTemplate.Replace(Parameters.ClassTransactionImpl, "");
                    classTemplate = classTemplate.Replace(Parameters.ClassSyncImpl, "");
                } else {
                    var transactionCode = Templates.Transaction;
                    transactionCode     = transactionCode.Replace(Parameters.ClassClassName, item.UserName);

                    Builders.Decelerations.Clear();
                    foreach (var property in item.Properties)
                        BuildJsonObjectSetter(Builders.Decelerations, property);

                    transactionCode = transactionCode.Replace(Parameters.TransAddMembersToJObject, Builders.Decelerations.ToString());
                    classTemplate   = classTemplate.Replace(Parameters.ClassTransactionImpl, transactionCode);

                    var sync = Templates.Sync;
                    sync     = sync.Replace(Parameters.ClassClassName, item.UserName);

                    classTemplate = classTemplate.Replace(Parameters.ClassSyncImpl, sync);
                }
                Builders.Classes.Append(classTemplate);
            }
        }

        private static void BuildJsonObjectSetter(StringBuilder decelerations, StoreItemProperty property)
        {
            decelerations.Append(Parameters.EOL);
            decelerations.Append(' ', 12);
            
            if (property.Property.Options == ItemPropertyOptions.MultiLineString)
                decelerations.Append($"obj.AddValue(\"{property.Name}\", StringUtils.ToBase64({property.Name}));");
            else
                decelerations.Append($"obj.AddValue(\"{property.Name}\", {property.Name});");
        }

        private void GenerateContent()
        {
            var connection = Templates.Connection;

            Builders.Registration.Clear();
            Builders.Classes.Clear();

            // Replace the RegisterTypes variable
            Builders.Registration.Append(Parameters.EOL);
            foreach (var item in Manager.Items) {
                Builders.Registration.Append(' ', 16);
                Builders.Registration.Append($"{item.UserName}.Register();");
                Builders.Registration.Append(Parameters.EOL);
            }
            connection = connection.Replace(Parameters.ContentRegisterTypes, Builders.Registration.ToString());

            // Replace the ClearTypes variable
            Builders.Registration.Clear();
            Builders.Registration.Append(Parameters.EOL);
            foreach (var item in Manager.Items) {
                Builders.Registration.Append(' ', 12);
                Builders.Registration.Append($"{item.UserName}.Clear();");
                Builders.Registration.Append(Parameters.EOL);
            }
            connection = connection.Replace(Parameters.ContentClearTypes, Builders.Registration.ToString());

            GenerateClasses();
            connection = connection.Replace(Parameters.HeaderContent, Builders.Classes.ToString());

            Builders.Content.Append(connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Generate(StoreItemManager items, string path)
        {
            try {
                Manager = items;
                Output  = path;
                return GenerateImpl();
            } catch (Exception ex) {
                LogUtils.Log(ex.Message);
            }

            return false;
        }
    }
}
