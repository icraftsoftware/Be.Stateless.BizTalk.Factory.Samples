#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.Extensions;
using Org.Anization.BizTalk.Extensions;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	internal static class AdapterExtensions
	{
		internal static string GetAdapterName(this IAdapter adapter)
		{
			return adapter.IsWcfAdapter()
				? adapter.GetWcfAdapterName()
				: adapter.GetType().BaseType!.Name.SubstringBefore("Adapter");
		}

		internal static string GetQualifiedAdapterName(this IAdapter adapter)
		{
			return adapter.IsWcfAdapter()
				? adapter.GetQualifiedWcfAdapterName()
				: adapter.GetAdapterName();
		}

		internal static StandardBindingElement GetWcfAdapterBindingElement(this IAdapter adapter)
		{
			if (!adapter.IsWcfAdapter()) throw new ArgumentException($"'{adapter.GetType()}' does not implements '{typeof(IAdapterConfigBinding<>)}' interface.", nameof(adapter));
			var adapterConfigBindingInterface = adapter.GetType().GetInterface(typeof(IAdapterConfigBinding<>).Name);
			if (adapterConfigBindingInterface == null) throw new InvalidOperationException($"Cannot get '{typeof(IAdapterConfigBinding<>)}' interface from '{adapter.GetType()}'.");
			var getMethod = adapterConfigBindingInterface.GetProperty(nameof(IAdapterConfigBinding<StandardBindingElement>.Binding))!.GetMethod;
			return (StandardBindingElement) getMethod.Invoke(adapter, null);
		}

		private static string GetWcfAdapterName(this IAdapter adapter)
		{
			return "Wcf" + adapter.GetWcfAdapterBindingElement() switch {
				OracleBindingElement => "Oracle",
				SapBindingElement => "Sap",
				SqlBindingElement => "Sql",
				var binding => binding.GetType().Name.SubstringBefore("BindingElement")
			};
		}

		private static string GetQualifiedWcfAdapterName(this IAdapter adapter)
		{
			var name = adapter.GetWcfAdapterName();
			if (adapter.IsWcfCustomAdapter()) name += "Custom";
			if (adapter.IsWcfCustomIsolatedAdapter()) name += "CustomIsolated";
			return name;
		}

		internal static bool IsWcfAdapter(this IAdapter adapter)
		{
			if (adapter == null) throw new ArgumentNullException(nameof(adapter));
			return adapter.GetType().IsSubclassOfGenericType(typeof(IAdapterConfigBinding<>));
		}

		internal static bool IsWcfCustomAdapter(this IAdapter adapter)
		{
			if (adapter == null) throw new ArgumentNullException(nameof(adapter));
			return adapter.GetType().IsSubclassOfGenericType(typeof(WcfCustomAdapter<,>));
		}

		internal static bool IsWcfCustomIsolatedAdapter(this IAdapter adapter)
		{
			if (adapter == null) throw new ArgumentNullException(nameof(adapter));
			return adapter.GetType().IsSubclassOfGenericType(typeof(WcfCustomIsolatedAdapter<,>));
		}
	}
}
