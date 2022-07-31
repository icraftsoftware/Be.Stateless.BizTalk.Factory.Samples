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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Moq;
using Org.Anization.BizTalk.Environment.Settings.Convention.Extensions;
using Xunit;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;
using static FluentAssertions.FluentActions;

namespace Org.Anization.BizTalk.Environment.Settings.Convention
{
	[Collection("TargetEnvironment")]
	public class NetworkZoneBoundHostResolutionPolicyFixture
	{
		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT, NetworkZones.B2B)]
		[InlineData(TargetEnvironment.DEVELOPMENT, NetworkZones.Intranet)]
		[InlineData(TargetEnvironment.BUILD, NetworkZones.B2B)]
		[InlineData(TargetEnvironment.BUILD, NetworkZones.Intranet)]
		[InlineData(TargetEnvironment.INTEGRATION, NetworkZones.Intranet)]
		[InlineData(TargetEnvironment.ACCEPTANCE, NetworkZones.Intranet)]
		[InlineData(TargetEnvironment.PREPRODUCTION, NetworkZones.Intranet)]
		[InlineData(TargetEnvironment.PRODUCTION, NetworkZones.Intranet)]
		public void ResolveHostForOrchestration(string targetEnvironment, NetworkZones networkZones)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var orchestrationMock = new Mock<OrchestrationBindingBase<BTXService>> {
					CallBase = true,
					Object = {
						Host = new NetworkZoneBoundHostResolutionPolicy(networkZones)
					}
				};
				var nameResolver = orchestrationMock.Object;

				nameResolver.ResolveHost().Should().Be(targetEnvironment.IsDevelopmentOrBuild() ? "BizTalkServerApplication" : "PxHost");
			}
		}

		[Theory]
		[InlineData(TargetEnvironment.INTEGRATION, NetworkZones.B2B)]
		[InlineData(TargetEnvironment.ACCEPTANCE, NetworkZones.B2B)]
		[InlineData(TargetEnvironment.PREPRODUCTION, NetworkZones.B2B)]
		[InlineData(TargetEnvironment.PRODUCTION, NetworkZones.B2B)]
		public void ResolveHostForOrchestrationThrows(string targetEnvironment, NetworkZones networkZones)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: targetEnvironment))
			{
				var orchestrationMock = new Mock<OrchestrationBindingBase<BTXService>> {
					CallBase = true,
					Object = {
						Host = new NetworkZoneBoundHostResolutionPolicy(networkZones)
					}
				};
				var nameResolver = orchestrationMock.Object;

				Invoking(() => nameResolver.ResolveHost()).Should().Throw<NotSupportedException>().WithMessage("Hosting orchestrations is not supported in B2B.");
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.INTEGRATION)]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.ACCEPTANCE)]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.PREPRODUCTION)]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocation(TestData testData)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
					(Action<IReceiveLocation<string>>) (rl => {
						rl.Transport.Adapter = testData.InboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = receiveLocationMock.Object.Transport;

				nameResolver.ResolveHost().Should().Be(testData.HostName);
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.DEVELOPMENT)]
		[MemberData(nameof(GetTestDataForSupportedInboundAdapter), TargetEnvironment.BUILD)]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.DEVELOPMENT)]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.BUILD)]
		public void ResolveHostForReceiveLocationForDevelopmentOrBuild(TestData testData)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
					(Action<IReceiveLocation<string>>) (rl => {
						rl.Transport.Adapter = testData.InboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = receiveLocationMock.Object.Transport;

				nameResolver.ResolveHost()
					.Should().Be(testData.InboundAdapter.ProtocolType.RequiresIsolatedHostForReceiveHandler() ? "BizTalkServerIsolatedHost" : "BizTalkServerApplication");
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.INTEGRATION)]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.ACCEPTANCE)]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.PREPRODUCTION)]
		[MemberData(nameof(GetTestDataForUnsupportedInboundAdapter), TargetEnvironment.PRODUCTION)]
		public void ResolveHostForReceiveLocationThrows(TestData testData)
		{
			// sanity check
			testData.HostName.Should().BeNullOrEmpty();

			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
					(Action<IReceiveLocation<string>>) (rl => {
						rl.Transport.Adapter = testData.InboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = receiveLocationMock.Object.Transport;

				Invoking(() => nameResolver.ResolveHost()).Should().Throw<NotSupportedException>();
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.INTEGRATION)]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.ACCEPTANCE)]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.PREPRODUCTION)]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPort(TestData testData)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>(
					(Action<ISendPort<string>>) (rl => {
						rl.Transport.Adapter = testData.OutboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = sendPortMock.Object.Transport;

				nameResolver.ResolveHost().Should().Be(testData.HostName);
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.DEVELOPMENT)]
		[MemberData(nameof(GetTestDataForSupportedOutboundAdapter), TargetEnvironment.BUILD)]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.DEVELOPMENT)]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.BUILD)]
		public void ResolveHostForSendPortForDevelopmentOrBuild(TestData testData)
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>(
					(Action<ISendPort<string>>) (rl => {
						rl.Transport.Adapter = testData.OutboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = sendPortMock.Object.Transport;

				nameResolver.ResolveHost().Should().Be("BizTalkServerApplication");
			}
		}

		[Theory]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.INTEGRATION)]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.ACCEPTANCE)]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.PREPRODUCTION)]
		[MemberData(nameof(GetTestDataForUnsupportedOutboundAdapter), TargetEnvironment.PRODUCTION)]
		public void ResolveHostForSendPortThrows(TestData testData)
		{
			// sanity check
			testData.HostName.Should().BeNullOrEmpty();

			using (new DeploymentContextInjectionScope(targetEnvironment: testData.TargetEnvironment))
			{
				var sendPortMock = new Mock<SendPortBase<string>>(
					(Action<ISendPort<string>>) (rl => {
						rl.Transport.Adapter = testData.OutboundAdapter;
						rl.Transport.Host = new NetworkZoneBoundHostResolutionPolicy(testData.NetworkZone);
					})) {
					CallBase = true
				};
				sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());
				var nameResolver = sendPortMock.Object.Transport;

				Invoking(() => nameResolver.ResolveHost()).Should().Throw<NotSupportedException>();
			}
		}

		private static IEnumerable<object[]> GetTestDataForSupportedInboundAdapter(string targetEnvironment)
		{
			return GetInboundAdapterTestData()
				.Where(td => td.InboundAdapter.IsSupportedFor(td.NetworkZone))
				.Select(
					td => {
						td.TargetEnvironment = targetEnvironment;
						return new object[] { td };
					});
		}

		private static IEnumerable<object[]> GetTestDataForSupportedOutboundAdapter(string targetEnvironment)
		{
			return GetOutboundAdapterTestData()
				.Where(td => td.OutboundAdapter.IsSupportedFor(td.NetworkZone))
				.Select(
					td => {
						td.TargetEnvironment = targetEnvironment;
						return new object[] { td };
					});
		}

		private static IEnumerable<object[]> GetTestDataForUnsupportedInboundAdapter(string targetEnvironment)
		{
			return GetInboundAdapterTestData()
				.Where(td => !td.InboundAdapter.IsSupportedFor(td.NetworkZone))
				.Select(
					td => {
						td.TargetEnvironment = targetEnvironment;
						return new object[] { td };
					});
		}

		private static IEnumerable<object[]> GetTestDataForUnsupportedOutboundAdapter(string targetEnvironment)
		{
			return GetOutboundAdapterTestData()
				.Where(td => !td.OutboundAdapter.IsSupportedFor(td.NetworkZone))
				.Select(
					td => {
						td.TargetEnvironment = targetEnvironment;
						return new object[] { td };
					});
		}

		private static IEnumerable<TestData> GetInboundAdapterTestData()
		{
			// @formatter:off
			yield return new TestData { Adapter = new FileAdapter.Inbound(),                                       NetworkZone = NetworkZones.Intranet, HostName = "RxHost_File" };
			yield return new TestData { Adapter = new FileAdapter.Inbound(),                                       NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new FtpAdapter.Inbound(),                                        NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new FtpAdapter.Inbound(),                                        NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new HttpAdapter.Inbound(),                                       NetworkZone = NetworkZones.Intranet, HostName = "LxHost_Http" };
			yield return new TestData { Adapter = new HttpAdapter.Inbound(),                                       NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new Pop3Adapter.Inbound(),                                       NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new Pop3Adapter.Inbound(),                                       NetworkZone = NetworkZones.B2B,      HostName = "RxHost_B2B_Pop3_32_Single" };
			yield return new TestData { Adapter = new SBMessagingAdapter.Inbound(),                                NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new SBMessagingAdapter.Inbound(),                                NetworkZone = NetworkZones.B2B,      HostName = "RxHost_B2B_SBMessaging" };
			yield return new TestData { Adapter = new SftpAdapter.Inbound(),                                       NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new SftpAdapter.Inbound(),                                       NetworkZone = NetworkZones.B2B,      HostName = "RxHost_B2B_Sftp_Single" };
			yield return new TestData { Adapter = new WcfBasicHttpAdapter.Inbound(),                               NetworkZone = NetworkZones.Intranet, HostName = "LxHost_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfBasicHttpAdapter.Inbound(),                               NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<BasicHttpBindingElement>(),         NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<BasicHttpBindingElement>(),         NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<BasicHttpBindingElement>(), NetworkZone = NetworkZones.Intranet, HostName = "LxHost_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<BasicHttpBindingElement>(), NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfBasicHttpRelayAdapter.Inbound(),                          NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfBasicHttpRelayAdapter.Inbound(),                          NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetMsmqAdapter.Inbound(),                                 NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfNetMsmq" };
			yield return new TestData { Adapter = new WcfNetMsmqAdapter.Inbound(),                                 NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(),           NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfNetMsmq" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(),           NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetNamedPipeAdapter.Inbound(),                            NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfNetNamedPipeAdapter.Inbound(),                            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetNamedPipeBindingElement>(),      NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetNamedPipeBindingElement>(),      NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetTcpAdapter.Inbound(),                                  NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfNetTcp" };
			yield return new TestData { Adapter = new WcfNetTcpAdapter.Inbound(),                                  NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetTcpBindingElement>(),            NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfNetTcp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<NetTcpBindingElement>(),            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetTcpRelayAdapter.Inbound(),                             NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfNetTcpRelayAdapter.Inbound(),                             NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfOracleAdapter.Inbound(),                                  NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfOracle" };
			yield return new TestData { Adapter = new WcfOracleAdapter.Inbound(),                                  NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<OracleBindingElement>(),            NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfOracle" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<OracleBindingElement>(),            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfSapAdapter.Inbound(),                                     NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfSap" };
			yield return new TestData { Adapter = new WcfSapAdapter.Inbound(),                                     NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<SapBindingElement>(),               NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfSap" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<SapBindingElement>(),               NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfSqlAdapter.Inbound(),                                     NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfSql" };
			yield return new TestData { Adapter = new WcfSqlAdapter.Inbound(),                                     NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<SqlBindingElement>(),               NetworkZone = NetworkZones.Intranet, HostName = "RxHost_WcfSql" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<SqlBindingElement>(),               NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfWebHttpAdapter.Inbound(),                                 NetworkZone = NetworkZones.Intranet, HostName = "LxHost_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfWebHttpAdapter.Inbound(),                                 NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<WebHttpBindingElement>(),           NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<WebHttpBindingElement>(),           NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WebHttpBindingElement>(),   NetworkZone = NetworkZones.Intranet, HostName = "LxHost_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WebHttpBindingElement>(),   NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfWSHttpAdapter.Inbound(),                                  NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfWSHttpAdapter.Inbound(),                                  NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfWSHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<WSHttpBindingElement>(),            NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomAdapter.Inbound<WSHttpBindingElement>(),            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WSHttpBindingElement>(),    NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WSHttpBindingElement>(),    NetworkZone = NetworkZones.B2B,      HostName = "LxHost_B2B_WcfWSHttp" };
			// @formatter:on
		}

		private static IEnumerable<TestData> GetOutboundAdapterTestData()
		{
			// @formatter:off
			yield return new TestData { Adapter = new FileAdapter.Outbound(),                                       NetworkZone = NetworkZones.Intranet, HostName = "TxHost_File" };
			yield return new TestData { Adapter = new FileAdapter.Outbound(),                                       NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new FtpAdapter.Outbound(),                                        NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new FtpAdapter.Outbound(),                                        NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new HttpAdapter.Outbound(),                                       NetworkZone = NetworkZones.Intranet, HostName = "TxHost_Http" };
			yield return new TestData { Adapter = new HttpAdapter.Outbound(),                                       NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_Http" };
			yield return new TestData { Adapter = new SBMessagingAdapter.Outbound(),                                NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new SBMessagingAdapter.Outbound(),                                NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_SBMessaging" };
			yield return new TestData { Adapter = new SftpAdapter.Outbound(),                                       NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new SftpAdapter.Outbound(),                                       NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_Sftp" };
			yield return new TestData { Adapter = new WcfBasicHttpAdapter.Outbound(),                               NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfBasicHttpAdapter.Outbound(),                               NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>(),         NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>(),         NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfBasicHttp" };
			yield return new TestData { Adapter = new WcfBasicHttpRelayAdapter.Outbound(),                          NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfBasicHttpRelayAdapter.Outbound(),                          NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetMsmqAdapter.Outbound(),                                 NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetMsmq" };
			yield return new TestData { Adapter = new WcfNetMsmqAdapter.Outbound(),                                 NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetMsmqBindingElement>(),           NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetMsmq" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetMsmqBindingElement>(),           NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetNamedPipeAdapter.Outbound(),                            NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetNamedPipe"};
			yield return new TestData { Adapter = new WcfNetNamedPipeAdapter.Outbound(),                            NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfNetNamedPipe" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetNamedPipeBindingElement>(),      NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetNamedPipe" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetNamedPipeBindingElement>(),      NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfNetNamedPipe" };
			yield return new TestData { Adapter = new WcfNetTcpAdapter.Outbound(),                                  NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetTcp" };
			yield return new TestData { Adapter = new WcfNetTcpAdapter.Outbound(),                                  NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(),            NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfNetTcp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(),            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfNetTcpRelayAdapter.Outbound(),                             NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfNetTcpRelayAdapter.Outbound(),                             NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfOracleAdapter.Outbound(),                                  NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfOracle" };
			yield return new TestData { Adapter = new WcfOracleAdapter.Outbound(),                                  NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<OracleBindingElement>(),            NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfOracle" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<OracleBindingElement>(),            NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfSapAdapter.Outbound(),                                     NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfSap" };
			yield return new TestData { Adapter = new WcfSapAdapter.Outbound(),                                     NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<SapBindingElement>(),               NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfSap" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<SapBindingElement>(),               NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfSqlAdapter.Outbound(),                                     NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfSql" };
			yield return new TestData { Adapter = new WcfSqlAdapter.Outbound(),                                     NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<SqlBindingElement>(),               NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfSql" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<SqlBindingElement>(),               NetworkZone = NetworkZones.B2B };
			yield return new TestData { Adapter = new WcfWebHttpAdapter.Outbound(),                                 NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfWebHttpAdapter.Outbound(),                                 NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<WebHttpBindingElement>(),           NetworkZone = NetworkZones.Intranet, HostName = "TxHost_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<WebHttpBindingElement>(),           NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfWebHttp" };
			yield return new TestData { Adapter = new WcfWSHttpAdapter.Outbound(),                                  NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfWSHttpAdapter.Outbound(),                                  NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfWSHttp" };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<WSHttpBindingElement>(),            NetworkZone = NetworkZones.Intranet };
			yield return new TestData { Adapter = new WcfCustomAdapter.Outbound<WSHttpBindingElement>(),            NetworkZone = NetworkZones.B2B,      HostName = "TxHost_B2B_WcfWSHttp" };
			// @formatter:on
		}

		public class TestData
		{
			#region Base Class Member Overrides

			public override string ToString()
			{
				return Adapter.GetType().ToString();
			}

			#endregion

			public IAdapter Adapter { get; set; }

			public string HostName { get; set; }

			public IInboundAdapter InboundAdapter => (IInboundAdapter) Adapter;

			public NetworkZones NetworkZone { get; set; }

			public IOutboundAdapter OutboundAdapter => (IOutboundAdapter) Adapter;

			public string TargetEnvironment { get; set; }
		}
	}
}
