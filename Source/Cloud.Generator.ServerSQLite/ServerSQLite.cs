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
using System.Text;
using Cloud.Common;
using Cloud.GeneratorApi;

namespace Cloud.Generator.ServerSQLite
{
    static class Parameters
    {
        public const string HeaderUsing               = "${Using}";
        public const string HeaderProjectNamespace    = "${ProjectNamespace}";
        public const string HeaderContent             = "${Content}";
        public const string ContentRegisterTypes      = "${RegisterTypes}";
        public const string ContentClearTypes         = "${ClearTypes}";
        public const string ContentStatus             = "${Status}";
        public const string ClassClassName            = "${ClassName}";
        public const string ClassInterfaceName        = "${InterfaceName}";
        public const string ClassUniqueId             = "${UniqueId}";
        public const string ClassPropertyDeclarations = "${InterfaceDeclarations}";
        public const string EOL                       = "\r\n";

        // codes from utils..
        public const string RouteSelectArray = "${SelectArray}";
        public const string RouteSelectById  = "${SelectById}";
        public const string RouteSelectByKey = "${SelectByKey}";
        public const string RouteSave        = "${Save}";
        public const string RouteDrop        = "${Drop}";
        public const string RouteClear       = "${Clear}";
        public const string RouteContainsKey = "${ContainsKey}";
    }

    public static class Builders
    {
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

    public class ServerSQLiteGenerator : IStoreGenerator
    {
        private StoreItemManager Manager { get; set; }
        private string           Output { get; set; }

        private bool GenerateImpl()
        {
            Builders.Output.Clear();
            var headerString = Templates.Header;

            // make sure that the defining API is visible
            Builders.Content.Clear();
            foreach (var ns in Manager.UsedNamespaces)
            {
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
            foreach (var item in Manager.Items)
            {
                var classTemplate = Templates.Class;

                Builders.Decelerations.Clear();
                classTemplate = classTemplate.Replace(Parameters.ClassClassName, $"{item.UserName}");
                classTemplate = classTemplate.Replace(Parameters.ClassInterfaceName, item.InterfaceName);
                classTemplate = classTemplate.Replace(Parameters.ClassPropertyDeclarations, Builders.Decelerations.ToString());
                classTemplate = classTemplate.Replace(Parameters.ClassUniqueId, item.UniqueID.ToString(CultureInfo.InvariantCulture));

                // swap out route codes
                classTemplate = classTemplate.Replace(Parameters.RouteSave, Constants.Save.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteSelectArray, Constants.SelectArray.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteSelectById, Constants.SelectById.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteSelectByKey, Constants.SelectByKey.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteDrop, Constants.Drop.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteClear, Constants.Clear.ToString());
                classTemplate = classTemplate.Replace(Parameters.RouteContainsKey, Constants.Contains.ToString());

                Builders.Classes.Append(classTemplate);
            }
        }

        private void BuildPropertyDeclarationString(StringBuilder     propertyDeclarations,
                                                    StoreItemProperty property)
        {
            propertyDeclarations.Append(Parameters.EOL);
            propertyDeclarations.Append(' ', 8);
            propertyDeclarations.Append("public ");
            switch (property.Type)
            {
            case PropertyType.DateAndTime:
            case PropertyType.String:
                propertyDeclarations.Append("string ");
                break;
            case PropertyType.Integer:
                propertyDeclarations.Append("int ");
                break;
            case PropertyType.Real:
                propertyDeclarations.Append("float ");
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            propertyDeclarations.Append(property.Name);
            propertyDeclarations.Append(" { get; set; }");

            if (property.Default != null)
            {
                if (property.Type == PropertyType.String)
                    propertyDeclarations.Append($" = \"{property.Default}\";");
                else if (property.Type == PropertyType.Real)
                    propertyDeclarations.Append($" = {property.Default}f;");
                else
                    propertyDeclarations.Append($" = {property.Default};");
            }
            propertyDeclarations.Append(Parameters.EOL);
        }

        private void GenerateContent()
        {
            var connection = Templates.Connection;

            Builders.Registration.Clear();
            Builders.Classes.Clear();

            // Replace the RegisterTypes variable
            Builders.Registration.Append(Parameters.EOL);
            foreach (var item in Manager.Items)
            {
                Builders.Registration.Append(' ', 16);
                Builders.Registration.Append($"{item.UserName}.Register();");
                Builders.Registration.Append(Parameters.EOL);
            }
            connection = connection.Replace(Parameters.ContentRegisterTypes, Builders.Registration.ToString());
            connection = connection.Replace(Parameters.ContentStatus, Constants.Status.ToString());

            // Replace the ClearTypes variable
            Builders.Registration.Clear();
            Builders.Registration.Append(Parameters.EOL);

            foreach (var item in Manager.Items)
            {
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
            try
            {
                Manager = items;
                Output  = path;
                return GenerateImpl();
            }
            catch (Exception ex)
            {
                LogUtils.Log(ex.Message);
            }
            return false;
        }
    }
}
