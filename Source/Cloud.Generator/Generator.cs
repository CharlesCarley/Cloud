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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloud.Common;
using Cloud.Generator.ClientSQLite;
using Cloud.Generator.ServerSQLite;
using Cloud.GeneratorApi;
using Cloud.ReflectionApi;

namespace Cloud.Generator
{
    public class GeneratorImpl {
        public string            Namespace { get; set; }
        public string            Input { get; set; }
        public bool              Verbose { get; set; }
        public string            Output { get; set; }
        public string            Type { get; set; }
        private StoreItemManager Manager { get; }
        private IStoreGenerator  Generator { get; set; }

        public GeneratorImpl()
        {
            Manager = new StoreItemManager();
        }

        public void Run()
        {
            try {
                RunImpl();
                Environment.ExitCode = 0;
            } catch (Exception ex) {
                LogUtils.Log(ex.Message);
                Environment.ExitCode = -1;
            }
        }

        private void RunImpl()
        {
            CreateGeneratorBackend();

            var assembly = Assembly.LoadFile(Input);

            var definedTypes = assembly.DefinedTypes;
            foreach (var definedType in definedTypes) {
                if (!Manager.UsedNamespaces.Contains(definedType.Namespace))
                    Manager.UsedNamespaces.Add(definedType.Namespace);
            }

            if (Namespace == null) {
                // Note: This assumes that the input assembly classes
                // are all defined in the same namespace...
                var first = assembly.DefinedTypes.First();
                if (first != null)
                    Manager.Namespace = first.Namespace;
            } else {
                Manager.Namespace = Namespace;
            }

            if (Verbose)
                LogUtils.Log(Resources.VerboseNamespace, Manager.Namespace);

            if (string.IsNullOrEmpty(Manager.Namespace)) {
                LogUtils.Log(Resources.MissingNamespace);
                Environment.Exit(1);
            }

            foreach (var type in assembly.DefinedTypes)
                BuildType(type);

            Generator.Generate(Manager, Output);
        }

        private void BuildType(Type type)
        {
            var classAttributes = type.GetCustomAttributes();

            ItemAttribute topLevelItem = null;
            foreach (var clsAttr in classAttributes) {
                if (clsAttr is ItemAttribute attr)
                    topLevelItem = attr;
            }

            if (topLevelItem is null) {
                // skip this type
                if (Verbose)
                    LogUtils.Log(Resources.SkipClass, type.Name);
                return;
            }

            var storeItem = new StoreItem {
                UserName      = topLevelItem.Name,
                InterfaceName = type.Name,
                // TODO: add a filter to determine if the supplied code is unique within the content of this API.
                UniqueID       = topLevelItem.Code,
                CanSynchronize = topLevelItem.CanSynchronize,
                Attribute      = topLevelItem,
            };

            if (Verbose)
                LogUtils.Log(Resources.VerboseTypeName, storeItem.UserName);

            Manager.Items.Add(storeItem);

            var properties = type.GetProperties();
            foreach (var prop in properties) {
                if (!prop.CanRead) // filter private
                    continue;

                var attributes = prop.GetCustomAttributes();
                foreach (var attribute in attributes) {
                    if (attribute is ItemPropertyAttribute attr)
                        BuildAttributeForType(storeItem, attr, prop);
                }
            }
        }

        private void BuildAttributeForType(StoreItem             storeItem,
                                           ItemPropertyAttribute attribute,
                                           PropertyInfo          propertyInfo)
        {
            var property = new StoreItemProperty {
                Name     = propertyInfo.Name,
                Property = attribute,
            };

            var type = propertyInfo.PropertyType;
            if (type == typeof(int))
                property.Type = PropertyType.Integer;
            else if (type == typeof(float))
                property.Type = PropertyType.Real;
            else if (type == typeof(string))
                property.Type = PropertyType.String;
            else
                property.Type = PropertyType.String;

            property.IsAutoProperty = attribute.IsAutoProperty;
            property.MaximumSize    = attribute.MaximumSize;
            property.MinimumSize    = attribute.MinimumSize;
            property.Default        = attribute.Default;

            if (Verbose) {
                LogUtils.LogF(Resources.VerboseType1, property.Name);
                LogUtils.LogF(Resources.VerboseType2, property.Type);
                LogUtils.LogF(Resources.VerboseType3, property.IsAutoProperty);
                LogUtils.LogF(Resources.VerboseType4, property.MaximumSize);
                LogUtils.LogF(Resources.VerboseType5, property.MinimumSize);
                LogUtils.LogF(Resources.VerboseType6, property.Default);
            }

            storeItem.Properties.Add(property);
        }

        private void CreateGeneratorBackend()
        {
            switch (Type) {
            case "ClientSQLite":
                Generator = new ClientSQLiteGenerator();
                break;
            case "ServerSQLite":
                Generator = new ServerSQLiteGenerator();
                break;
            default:
                throw new NotImplementedException();
            }
        }
    }

}
