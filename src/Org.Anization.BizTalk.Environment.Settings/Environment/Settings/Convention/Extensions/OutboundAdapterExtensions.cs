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
using Microsoft.ServiceBus.Configuration;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	internal static class OutboundAdapterExtensions
	{
		internal static bool IsSupportedFor(this IOutboundAdapter outboundAdapter, NetworkZones networkZones)
		{
			return outboundAdapter.GetSupportedNetworkZones().Match(networkZones);
		}

		private static NetworkZones GetSupportedNetworkZones(this IOutboundAdapter outboundAdapter)
		{
			return outboundAdapter switch {
				var adapter when adapter.IsWcfAdapter() => adapter.GetWcfAdapterBindingElement() switch {
					BasicHttpBindingElement => NetworkZones.All,
					BasicHttpRelayBindingElement => NetworkZones.None,
					NetMsmqBindingElement => NetworkZones.Intranet,
					NetNamedPipeBindingElement => NetworkZones.All,
					NetTcpBindingElement => NetworkZones.Intranet,
					NetTcpRelayBindingElement => NetworkZones.None,
					OracleBindingElement => NetworkZones.Intranet,
					SapBindingElement => NetworkZones.Intranet,
					SqlBindingElement => NetworkZones.Intranet,
					WebHttpBindingElement => NetworkZones.All,
					WSHttpBindingElement => NetworkZones.B2B,
					_ => throw new NotSupportedException($"Outbound WCF adapter '{adapter.GetQualifiedAdapterName()}' is not supported in any {nameof(NetworkZones)}.")
				},
				FileAdapter.Outbound => NetworkZones.Intranet,
				FtpAdapter.Outbound => NetworkZones.None,
				HttpAdapter.Outbound => NetworkZones.All,
				SBMessagingAdapter.Outbound => NetworkZones.B2B,
				SftpAdapter.Outbound => NetworkZones.B2B,
				_ => throw new NotSupportedException($"Outbound adapter '{outboundAdapter.GetQualifiedAdapterName()}' is not supported in any {nameof(NetworkZones)}.")
			};
		}
	}
}
