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
using CustomNetMsmqBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.NetMsmqBindingElement;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	internal static class InboundAdapterExtensions
	{
		internal static bool IsSupportedFor(this IInboundAdapter inboundAdapter, NetworkZones networkZones)
		{
			return inboundAdapter.GetSupportedNetworkZones().Match(networkZones);
		}

		private static NetworkZones GetSupportedNetworkZones(this IInboundAdapter inboundAdapter)
		{
			return inboundAdapter switch {
				var adapter when adapter.IsWcfAdapter() => adapter.GetWcfAdapterBindingElement() switch {
					// only support BasicHttp on isolated host, thus not WcfCustomAdapter.Inbound<BasicHttp> which has to be hosted in process
					BasicHttpBindingElement => inboundAdapter.IsWcfCustomAdapter() ? NetworkZones.None : NetworkZones.All,
					BasicHttpRelayBindingElement => NetworkZones.None,
					NetMsmqBindingElement or CustomNetMsmqBindingElement => inboundAdapter.IsWcfCustomIsolatedAdapter() ? NetworkZones.None : NetworkZones.Intranet,
					NetNamedPipeBindingElement => NetworkZones.None,
					NetTcpBindingElement => NetworkZones.Intranet,
					NetTcpRelayBindingElement => NetworkZones.None,
					OracleBindingElement => inboundAdapter.IsWcfCustomIsolatedAdapter() ? NetworkZones.None : NetworkZones.Intranet,
					SapBindingElement => inboundAdapter.IsWcfCustomIsolatedAdapter() ? NetworkZones.None : NetworkZones.Intranet,
					SqlBindingElement => inboundAdapter.IsWcfCustomIsolatedAdapter() ? NetworkZones.None : NetworkZones.Intranet,
					// only support WebHttp on isolated host, thus not WcfCustomAdapter.Inbound<WebHttp> which has to be hosted in process
					WebHttpBindingElement => inboundAdapter.IsWcfCustomAdapter() ? NetworkZones.None : NetworkZones.All,
					// only support WSHttp on isolated host, thus not WcfCustomAdapter.Inbound<WSHttp> which has to be hosted in process
					WSHttpBindingElement => inboundAdapter.IsWcfCustomAdapter() ? NetworkZones.None : NetworkZones.B2B,
					_ => throw new NotSupportedException($"Inbound WCF adapter '{adapter.GetQualifiedAdapterName()}' is not supported in any {nameof(NetworkZones)}.")
				},
				FileAdapter.Inbound => NetworkZones.Intranet,
				FtpAdapter.Inbound => NetworkZones.None,
				HttpAdapter.Inbound => NetworkZones.Intranet,
				Office365EmailAdapter.Inbound => NetworkZones.B2B,
				Pop3Adapter.Inbound => NetworkZones.B2B,
				SBMessagingAdapter.Inbound => NetworkZones.B2B,
				SftpAdapter.Inbound => NetworkZones.B2B,
				_ => throw new NotSupportedException($"Inbound adapter '{inboundAdapter.GetQualifiedAdapterName()}' is not supported in any {nameof(NetworkZones)}.")
			};
		}
	}
}
