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
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Org.Anization.BizTalk.Environment.Settings.Convention
{
	[Collection("TargetEnvironment")]
	public class AnyNetworkZoneHostResolutionPolicyFixture
	{
		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForOrchestration(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var orchestrationMock = new Mock<IOrchestrationBinding>();
				orchestrationMock.Setup(m => m.ResolveName()).Returns("myProcess");

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				sut.ResolveHost(orchestrationMock.Object).Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "PxHost");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocation(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
				receiveLocationMock.Object.Transport.Adapter = new FileAdapter.Inbound();
				receiveLocationMock.Object.Name = "myReceiveLocation";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				sut.ResolveHost(receiveLocationMock.Object.Transport).Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "RxHost_File");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocationThrowsAmbiguousInvalidOperationException(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
					(Action<IReceiveLocation<string>>) (rl => { rl.Transport.Adapter = new WcfBasicHttpAdapter.Inbound(); })
				);
				receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				receiveLocationMock.Object.Name = "myReceiveLocation";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				Invoking(() => sut.ResolveHost(receiveLocationMock.Object.Transport))
					.Should().Throw<InvalidOperationException>()
					.WithMessage(
						"HostResolutionPolicy cannot unambiguously resolve host for inbound WcfBasicHttp adapter among the NetworkZones. "
						+ "Either HostResolutionPolicy.B2B or HostResolutionPolicy.Intranet must be explicitly assigned to myReceiveLocation's transport host property.");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocationThrowsNotSupportedException(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
					(Action<IReceiveLocation<string>>) (rl => { rl.Transport.Adapter = new FtpAdapter.Inbound(); })
				);
				receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				receiveLocationMock.Object.Name = "myReceiveLocation";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				Invoking(() => sut.ResolveHost(receiveLocationMock.Object.Transport))
					.Should().Throw<NotSupportedException>()
					.WithMessage($"Hosting inbound Ftp adapter is not supported for any 'NetworkZones' in '{targetEnvironment}'.");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocationWithExplicitB2BPolicy(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
				receiveLocationMock.Object.Transport.Adapter = new WcfBasicHttpAdapter.Inbound();
				receiveLocationMock.Object.Name = "myReceiveLocation";

				var sut = new B2BHostResolutionPolicySpy();

				sut.ResolveHost(receiveLocationMock.Object.Transport)
					.Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerIsolatedHost" : "LxHost_B2B_WcfBasicHttp");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocationWithExplicitIntranetPolicy(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
				receiveLocationMock.Object.Transport.Adapter = new WcfBasicHttpAdapter.Inbound();
				receiveLocationMock.Object.Name = "myReceiveLocation";

				var sut = new IntranetHostResolutionPolicySpy();

				sut.ResolveHost(receiveLocationMock.Object.Transport)
					.Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerIsolatedHost" : "LxHost_WcfBasicHttp");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPort(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>();
				sendPortMock.Object.Transport.Adapter = new FileAdapter.Outbound();
				sendPortMock.Object.Name = "mySendPort";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				sut.ResolveHost(sendPortMock.Object.Transport).Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "TxHost_File");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPortThrowsAmbiguousInvalidOperationException(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>(
					(Action<ISendPort<string>>) (rl => { rl.Transport.Adapter = new WcfBasicHttpAdapter.Outbound(); })
				);
				sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				sendPortMock.Object.Name = "mySendPort";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				Invoking(() => sut.ResolveHost(sendPortMock.Object.Transport))
					.Should().Throw<InvalidOperationException>()
					.WithMessage(
						"HostResolutionPolicy cannot unambiguously resolve host for outbound WcfBasicHttp adapter among the NetworkZones. "
						+ "Either HostResolutionPolicy.B2B or HostResolutionPolicy.Intranet must be explicitly assigned to mySendPort's transport host property.");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPortThrowsNotSupportedException(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>(
					(Action<ISendPort<string>>) (rl => { rl.Transport.Adapter = new FtpAdapter.Outbound(); })
				);
				sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				sendPortMock.Object.Name = "mySendPort";

				var sut = new AnyNetworkZoneHostResolutionPolicySpy();

				Invoking(() => sut.ResolveHost(sendPortMock.Object.Transport))
					.Should().Throw<NotSupportedException>()
					.WithMessage($"Hosting outbound Ftp adapter is not supported for any 'NetworkZones' in '{targetEnvironment}'.");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPortWithExplicitB2BPolicy(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>();
				sendPortMock.Object.Transport.Adapter = new WcfBasicHttpAdapter.Outbound();
				sendPortMock.Object.Name = "mySendPort";

				var sut = new B2BHostResolutionPolicySpy();

				sut.ResolveHost(sendPortMock.Object.Transport).Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "TxHost_B2B_WcfBasicHttp");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.INTEGRATION)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPortWithExplicitIntranetPolicy(string targetEnvironment)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>();
				sendPortMock.Object.Transport.Adapter = new WcfBasicHttpAdapter.Outbound();
				sendPortMock.Object.Name = "mySendPort";

				var sut = new IntranetHostResolutionPolicySpy();

				sut.ResolveHost(sendPortMock.Object.Transport).Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "TxHost_WcfBasicHttp");
			}
		}

		private class AnyNetworkZoneHostResolutionPolicySpy : AnyNetworkZoneHostResolutionPolicy
		{
			internal new string ResolveHost(IOrchestrationBinding orchestration)
			{
				return base.ResolveHost(orchestration);
			}

			internal new string ResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}

			internal new string ResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}
		}

		private class B2BHostResolutionPolicySpy : NetworkZoneBoundHostResolutionPolicy
		{
			internal B2BHostResolutionPolicySpy() : base(NetworkZones.B2B) { }

			internal new string ResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}

			internal new string ResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}
		}

		private class IntranetHostResolutionPolicySpy : NetworkZoneBoundHostResolutionPolicy
		{
			internal IntranetHostResolutionPolicySpy() : base(NetworkZones.Intranet) { }

			internal new string ResolveHost<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}

			internal new string ResolveHost<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHost(transport);
			}
		}
	}
}
