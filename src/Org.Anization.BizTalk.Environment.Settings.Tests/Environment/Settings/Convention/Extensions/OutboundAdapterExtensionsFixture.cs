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
using Microsoft.ServiceBus.Configuration;
using Xunit;
using OracleBindingElement = Microsoft.Adapters.OracleDB.OracleDBBindingConfigurationElement;
using SapBindingElement = Microsoft.Adapters.SAP.SAPAdapterBindingConfigurationElement;
using SqlBindingElement = Microsoft.Adapters.Sql.SqlAdapterBindingConfigurationElement;

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	public class OutboundAdapterExtensionsFixture
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
				new TestData { Adapter = new FileAdapter.Outbound(),                                    IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new FtpAdapter.Outbound(),                                     IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new HttpAdapter.Outbound(),                                    IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new SBMessagingAdapter.Outbound(),                             IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new SftpAdapter.Outbound(),                                    IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfBasicHttpAdapter.Outbound(),                            IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>(),      IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfBasicHttpRelayAdapter.Outbound(),                       IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<BasicHttpRelayBindingElement>(), IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfNetMsmqAdapter.Outbound(),                              IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<NetMsmqBindingElement>(),        IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfNetNamedPipeAdapter.Outbound(),                         IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<NetNamedPipeBindingElement>(),   IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfNetTcpAdapter.Outbound(),                               IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(),         IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfNetTcpRelayAdapter.Outbound(),                          IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<NetTcpRelayBindingElement>(),    IsSupportedForB2B = false, IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfOracleAdapter.Outbound(),                               IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<OracleBindingElement>(),         IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfSapAdapter.Outbound(),                                  IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<SapBindingElement>(),            IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfSqlAdapter.Outbound(),                                  IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<SqlBindingElement>(),            IsSupportedForB2B = false, IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfWebHttpAdapter.Outbound(),                              IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<WebHttpBindingElement>(),        IsSupportedForB2B = true,  IsSupportedForIntranet = true },
				new TestData { Adapter = new WcfWSHttpAdapter.Outbound(),                               IsSupportedForB2B = true,  IsSupportedForIntranet = false },
				new TestData { Adapter = new WcfCustomAdapter.Outbound<WSHttpBindingElement>(),         IsSupportedForB2B = true,  IsSupportedForIntranet = false }
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

			public IOutboundAdapter Adapter { get; set; }

			public bool IsSupportedForB2B { get; set; }

			public bool IsSupportedForIntranet { get; set; }
		}
	}
}
