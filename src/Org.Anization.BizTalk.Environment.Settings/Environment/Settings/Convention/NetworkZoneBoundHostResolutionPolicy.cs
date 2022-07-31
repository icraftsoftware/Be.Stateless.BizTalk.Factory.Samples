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
using System.ComponentModel;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Factory.Convention;
using Be.Stateless.BizTalk.Install;
using Org.Anization.BizTalk.Environment.Settings.Convention.Extensions;

namespace Org.Anization.BizTalk.Environment.Settings.Convention
{
	internal class NetworkZoneBoundHostResolutionPolicy : HostResolutionPolicy
	{
		internal NetworkZoneBoundHostResolutionPolicy(NetworkZones networkZones)
		{
			if (!Enum.IsDefined(typeof(NetworkZones), networkZones)) throw new InvalidEnumArgumentException(nameof(networkZones), (int) networkZones, typeof(NetworkZones));
			if (!networkZones.Single())
				throw new InvalidEnumArgumentException(
					$"A {nameof(NetworkZoneBoundHostResolutionPolicy)} can only be bound to one single NetworkZone and not to the following multiple ones: '{_networkZones}'.");
			_networkZones = networkZones;
		}

		#region Base Class Member Overrides

		public override string ResolveHost(IOrchestrationBinding orchestration)
		{
			return TryResolveHost(orchestration, out var hostName)
				? hostName
				: throw new NotSupportedException($"Hosting orchestrations is not supported in {_networkZones}.");
		}

		public override string ResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
		{
			return TryResolveHost(transport, out var hostName)
				? hostName
				: throw new NotSupportedException($"Hosting inbound adapter '{transport.Adapter.GetQualifiedAdapterName()}' is not supported in {_networkZones}.");
		}

		public override string ResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
		{
			return TryResolveHost(transport, out var hostName)
				? hostName
				: throw new NotSupportedException($"Hosting outbound adapter '{transport.Adapter.GetQualifiedAdapterName()}' is not supported in {_networkZones}.");
		}

		#endregion

		internal bool TryResolveHost(IOrchestrationBinding orchestration, out string hostName)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild())
			{
				hostName = base.ResolveHost(orchestration);
				return true;
			}
			if (_networkZones.Match(NetworkZones.Intranet))
			{
				hostName = "PxHost";
				return true;
			}
			hostName = null;
			return false;
		}

		internal bool TryResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport, out string hostName)
			where TNamingConvention : class
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild())
			{
				hostName = base.ResolveHost(transport);
				return true;
			}
			if (transport.Adapter.IsSupportedFor(_networkZones))
			{
				// [L|R]xHost_([NetworkZone]_)?[Adapter](_[bitness])?(_Single)?
				hostName = (transport.Adapter.ProtocolType.RequiresIsolatedHostForReceiveHandler() ? "L" : "R") + "xHost_";
				if (!_networkZones.Match(NetworkZones.Intranet)) hostName += $"{_networkZones}_";
				hostName += transport.Adapter.GetAdapterName();
				if (transport.Adapter.ProtocolType.Support32BitOnly()) hostName += "_32";
				if (transport.Adapter is Pop3Adapter.Inbound or SftpAdapter.Inbound) hostName += "_Single";
				return true;
			}
			hostName = null;
			return false;
		}

		internal bool TryResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport, out string hostName)
			where TNamingConvention : class
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild())
			{
				hostName = base.ResolveHost(transport);
				return true;
			}
			if (transport.Adapter.IsSupportedFor(_networkZones))
			{
				// TxHost_([NetworkZone]_)?[Adapter](_[bitness])?
				hostName = "TxHost_";
				if (!_networkZones.Match(NetworkZones.Intranet)) hostName += $"{_networkZones}_";
				hostName += transport.Adapter.GetAdapterName();
				if (transport.Adapter.ProtocolType.Support32BitOnly()) hostName += "_32";
				return true;
			}
			hostName = null;
			return false;
		}

		private readonly NetworkZones _networkZones;
	}
}
