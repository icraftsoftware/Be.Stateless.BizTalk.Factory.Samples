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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Factory.Convention;
using Be.Stateless.BizTalk.Install;
using Org.Anization.BizTalk.Environment.Settings.Convention.Extensions;

namespace Org.Anization.BizTalk.Environment.Settings.Convention
{
	public class AnyNetworkZoneHostResolutionPolicy : HostResolutionPolicy
	{
		#region Base Class Member Overrides

		public override string ResolveHost(IOrchestrationBinding orchestration)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild()) return base.ResolveHost(orchestration);
			var isResolvedForIntranet = _intranetPolicy.TryResolveHost(orchestration, out var internHostName);
			var isResolvedForB2B = _b2BPolicy.TryResolveHost(orchestration, out var b2BHostName);
			return ResolveHostName(
				"orchestrations",
				$"{orchestration.ResolveName()}'s host",
				isResolvedForB2B,
				isResolvedForIntranet,
				b2BHostName,
				internHostName);
		}

		public override string ResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild()) return base.ResolveHost(transport);
			var isResolvedForIntranet = _intranetPolicy.TryResolveHost(transport, out var internHostName);
			var isResolvedForB2B = _b2BPolicy.TryResolveHost(transport, out var b2BHostName);
			return ResolveHostName(
				$"inbound {transport.Adapter.GetQualifiedAdapterName()} adapter",
				$"{transport.ReceiveLocation.Name}'s transport host",
				isResolvedForB2B,
				isResolvedForIntranet,
				b2BHostName,
				internHostName);
		}

		public override string ResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild()) return base.ResolveHost(transport);
			var isResolvedForIntranet = _intranetPolicy.TryResolveHost(transport, out var internHostName);
			var isResolvedForB2B = _b2BPolicy.TryResolveHost(transport, out var b2BHostName);
			return ResolveHostName(
				$"outbound {transport.Adapter.GetQualifiedAdapterName()} adapter",
				$"{transport.SendPort.Name}'s transport host",
				isResolvedForB2B,
				isResolvedForIntranet,
				b2BHostName,
				internHostName);
		}

		#endregion

		internal HostResolutionPolicy B2B => _b2BPolicy;

		internal HostResolutionPolicy Intranet => _intranetPolicy;

		private string ResolveHostName(
			string artifactType,
			string artifactHostPropertyDescriptor,
			bool isResolvedForB2B,
			bool isResolvedForIntranet,
			string b2BHostName,
			string internHostName)
		{
			if (isResolvedForB2B && isResolvedForIntranet)
				throw new InvalidOperationException(
					$"{nameof(HostResolutionPolicy)} cannot unambiguously resolve host for {artifactType} among the {nameof(NetworkZones)}. "
					+ $"Either {nameof(HostResolutionPolicy)}.{nameof(B2B)} or {nameof(HostResolutionPolicy)}.{nameof(Intranet)} must be explicitly assigned to {artifactHostPropertyDescriptor} property.");
			if (isResolvedForIntranet) return internHostName;
			if (isResolvedForB2B) return b2BHostName;
			throw new NotSupportedException($"Hosting {artifactType} is not supported for any '{nameof(NetworkZones)}' in '{DeploymentContext.TargetEnvironment}'.");
		}

		private readonly NetworkZoneBoundHostResolutionPolicy _b2BPolicy = new(NetworkZones.B2B);
		private readonly NetworkZoneBoundHostResolutionPolicy _intranetPolicy = new(NetworkZones.Intranet);
	}
}
