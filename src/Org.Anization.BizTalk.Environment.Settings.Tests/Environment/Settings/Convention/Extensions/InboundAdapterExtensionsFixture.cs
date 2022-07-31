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

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using FluentAssertions;
using Xunit;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;
using CustomNetMsmqBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.NetMsmqBindingElement;

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	public class InboundAdapterExtensionsFixture
	{
		[Theory]
		[MemberData(nameof(GetTestData))]
		public void IsSupportedForNetworkZoneAll(TestData testData)
		{
			testData.Adapter.IsSupportedFor(NetworkZones.All).Should().Be(testData.IsSupportedForB2B && testData.IsSupportedForIntranet);
		}

		[Theory]
		[MemberData(nameof(GetTestData))]
		public void IsSupportedForNetworkZoneB2B(TestData testData)
		{
			testData.Adapter.IsSupportedFor(NetworkZones.B2B).Should().Be(testData.IsSupportedForB2B);
		}

		[Theory]
		[MemberData(nameof(GetTestData))]
		public void IsSupportedForNetworkZoneIntranet(TestData testData)
		{
			testData.Adapter.IsSupportedFor(NetworkZones.Intranet).Should().Be(testData.IsSupportedForIntranet);
		}

		[Theory]
		[MemberData(nameof(GetTestData))]
		public void IsSupportedForNetworkZoneNone(TestData testData)
		{
			testData.Adapter.IsSupportedFor(NetworkZones.None).Should().Be(false);
		}

		private static IEnumerable<object[]> GetTestData()
		{
			var testData = new[] {
				// @formatter:off
				new TestData { Adapter = new FileAdapter.Inbound(),                                           IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new FtpAdapter.Inbound(),                                            IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new HttpAdapter.Inbound(),                                           IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new Office365EmailAdapter.Inbound(),                                 IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new Pop3Adapter.Inbound(),                                           IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new SBMessagingAdapter.Inbound(),                                    IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new SftpAdapter.Inbound(),                                           IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfBasicHttpAdapter.Inbound(),                                   IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<BasicHttpBindingElement>(),             IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<BasicHttpBindingElement>(),     IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfBasicHttpRelayAdapter.Inbound(),                              IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfNetMsmqAdapter.Inbound(),                                     IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(),               IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<NetMsmqBindingElement>(),       IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<CustomNetMsmqBindingElement>(),         IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<CustomNetMsmqBindingElement>(), IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfNetNamedPipeAdapter.Inbound(),                                IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<NetNamedPipeBindingElement>(),          IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<NetNamedPipeBindingElement>(),  IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfNetTcpAdapter.Inbound(),                                      IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<NetTcpBindingElement>(),                IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<NetTcpBindingElement>(),        IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfNetTcpRelayAdapter.Inbound(),                                 IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfOracleAdapter.Inbound(),                                      IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<OracleBindingElement>(),                IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<OracleBindingElement>(),        IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfSapAdapter.Inbound(),                                         IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<SapBindingElement>(),                   IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<SapBindingElement>(),           IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfSqlAdapter.Inbound(),                                         IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<SqlBindingElement>(),                   IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<SqlBindingElement>(),           IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfWebHttpAdapter.Inbound(),                                     IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<WebHttpBindingElement>(),               IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WebHttpBindingElement>(),       IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfWSHttpAdapter.Inbound(),                                      IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Inbound<WSHttpBindingElement>(),                IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomIsolatedAdapter.Inbound<WSHttpBindingElement>(),        IsSupportedForB2B = true,  IsSupportedForIntranet = false }
				// @formatter:on
			};
			return testData.Select(td => new object[] { td });
		}

		public class TestData
		{
			#region Base Class Member Overrides

			public override string ToString()
			{
				return Adapter.GetType().ToString();
			}

			#endregion

			public IInboundAdapter Adapter { get; set; }

			public bool IsSupportedForB2B { get; set; }

			public bool IsSupportedForIntranet { get; set; }
		}
	}
}
