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

namespace Org.Anization.BizTalk.Environment.Settings.Convention.Extensions
{
	public class AdapterExtensionsFixture
	{
		[Theory]
		[MemberData(nameof(GetTestData))]
		public void GetName(TestData testData)
		{
			testData.Adapter.GetAdapterName().Should().Be(testData.Name);
		}

		private static IEnumerable<object[]> GetTestData()
		{
			var testData = new[] {
				// @formatter:off
				new TestData { Name = "File",              Adapter = new FileAdapter.Inbound() },
				new TestData { Name = "File",              Adapter = new FileAdapter.Outbound() },
				new TestData { Name = "Ftp",               Adapter = new FtpAdapter.Inbound() },
				new TestData { Name = "Ftp",               Adapter = new FtpAdapter.Outbound() },
				new TestData { Name = "Http",              Adapter = new HttpAdapter.Inbound() },
				new TestData { Name = "Http",              Adapter = new HttpAdapter.Outbound() },
				new TestData { Name = "Office365Email",    Adapter = new Office365EmailAdapter.Inbound() },
				new TestData { Name = "Pop3",              Adapter = new Pop3Adapter.Inbound() },
				new TestData { Name = "SBMessaging",       Adapter = new SBMessagingAdapter.Inbound() },
				new TestData { Name = "SBMessaging",       Adapter = new SBMessagingAdapter.Outbound() },
				new TestData { Name = "Sftp",              Adapter = new SftpAdapter.Inbound() },
				new TestData { Name = "Sftp",              Adapter = new SftpAdapter.Outbound() },
				new TestData { Name = "WcfBasicHttp",      Adapter = new WcfBasicHttpAdapter.Inbound() },
				new TestData { Name = "WcfBasicHttp",      Adapter = new WcfBasicHttpAdapter.Outbound() },
				new TestData { Name = "WcfBasicHttp",      Adapter = new WcfCustomAdapter.Inbound<BasicHttpBindingElement>() },
				new TestData { Name = "WcfBasicHttp",      Adapter = new WcfCustomIsolatedAdapter.Inbound<BasicHttpBindingElement>() },
				new TestData { Name = "WcfBasicHttp",      Adapter = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>() },
				new TestData { Name = "WcfBasicHttpRelay", Adapter = new WcfBasicHttpRelayAdapter.Inbound() },
				new TestData { Name = "WcfBasicHttpRelay", Adapter = new WcfBasicHttpRelayAdapter.Outbound() },
				new TestData { Name = "WcfNetMsmq",        Adapter = new WcfNetMsmqAdapter.Inbound() },
				new TestData { Name = "WcfNetMsmq",        Adapter = new WcfNetMsmqAdapter.Outbound() },
				new TestData { Name = "WcfNetMsmq",        Adapter = new WcfCustomAdapter.Inbound<NetMsmqBindingElement>() },
				new TestData { Name = "WcfNetMsmq",        Adapter = new WcfCustomIsolatedAdapter.Inbound<NetMsmqBindingElement>() },
				new TestData { Name = "WcfNetMsmq",        Adapter = new WcfCustomAdapter.Outbound<NetMsmqBindingElement>() },
				new TestData { Name = "WcfNetNamedPipe",   Adapter = new WcfNetNamedPipeAdapter.Inbound() },
				new TestData { Name = "WcfNetNamedPipe",   Adapter = new WcfNetNamedPipeAdapter.Outbound() },
				new TestData { Name = "WcfNetNamedPipe",   Adapter = new WcfCustomAdapter.Inbound<NetNamedPipeBindingElement>() },
				new TestData { Name = "WcfNetNamedPipe",   Adapter = new WcfCustomIsolatedAdapter.Inbound<NetNamedPipeBindingElement>() },
				new TestData { Name = "WcfNetNamedPipe",   Adapter = new WcfCustomAdapter.Outbound<NetNamedPipeBindingElement>() },
				new TestData { Name = "WcfNetTcp",         Adapter = new WcfNetTcpAdapter.Inbound() },
				new TestData { Name = "WcfNetTcp",         Adapter = new WcfNetTcpAdapter.Outbound() },
				new TestData { Name = "WcfNetTcp",         Adapter = new WcfCustomAdapter.Inbound<NetTcpBindingElement>() },
				new TestData { Name = "WcfNetTcp",         Adapter = new WcfCustomIsolatedAdapter.Inbound<NetTcpBindingElement>() },
				new TestData { Name = "WcfNetTcp",         Adapter = new WcfCustomAdapter.Outbound<NetTcpBindingElement>() },
				new TestData { Name = "WcfNetTcpRelay",    Adapter = new WcfNetTcpRelayAdapter.Inbound() },
				new TestData { Name = "WcfNetTcpRelay",    Adapter = new WcfNetTcpRelayAdapter.Outbound() },
				new TestData { Name = "WcfOracle",         Adapter = new WcfOracleAdapter.Inbound() },
				new TestData { Name = "WcfOracle",         Adapter = new WcfOracleAdapter.Outbound() },
				new TestData { Name = "WcfOracle",         Adapter = new WcfCustomAdapter.Inbound<OracleBindingElement>() },
				new TestData { Name = "WcfOracle",         Adapter = new WcfCustomIsolatedAdapter.Inbound<OracleBindingElement>() },
				new TestData { Name = "WcfOracle",         Adapter = new WcfCustomAdapter.Outbound<OracleBindingElement>() },
				new TestData { Name = "WcfSap",            Adapter = new WcfSapAdapter.Inbound() },
				new TestData { Name = "WcfSap",            Adapter = new WcfSapAdapter.Outbound() },
				new TestData { Name = "WcfSap",            Adapter = new WcfCustomAdapter.Inbound<SapBindingElement>() },
				new TestData { Name = "WcfSap",            Adapter = new WcfCustomIsolatedAdapter.Inbound<SapBindingElement>() },
				new TestData { Name = "WcfSap",            Adapter = new WcfCustomAdapter.Outbound<SapBindingElement>() },
				new TestData { Name = "WcfSql",            Adapter = new WcfSqlAdapter.Inbound() },
				new TestData { Name = "WcfSql",            Adapter = new WcfSqlAdapter.Outbound() },
				new TestData { Name = "WcfSql",            Adapter = new WcfCustomAdapter.Inbound<SqlBindingElement>() },
				new TestData { Name = "WcfSql",            Adapter = new WcfCustomIsolatedAdapter.Inbound<SqlBindingElement>() },
				new TestData { Name = "WcfSql",            Adapter = new WcfCustomAdapter.Outbound<SqlBindingElement>() },
				new TestData { Name = "WcfWebHttp",        Adapter = new WcfWebHttpAdapter.Inbound() },
				new TestData { Name = "WcfWebHttp",        Adapter = new WcfWebHttpAdapter.Outbound() },
				new TestData { Name = "WcfWebHttp",        Adapter = new WcfCustomAdapter.Inbound<WebHttpBindingElement>() },
				new TestData { Name = "WcfWebHttp",        Adapter = new WcfCustomIsolatedAdapter.Inbound<WebHttpBindingElement>() },
				new TestData { Name = "WcfWebHttp",        Adapter = new WcfCustomAdapter.Outbound<WebHttpBindingElement>() },
				new TestData { Name = "WcfWSHttp",         Adapter = new WcfWSHttpAdapter.Inbound() },
				new TestData { Name = "WcfWSHttp",         Adapter = new WcfWSHttpAdapter.Outbound() },
				new TestData { Name = "WcfWSHttp",         Adapter = new WcfCustomAdapter.Inbound<WSHttpBindingElement>() },
				new TestData { Name = "WcfWSHttp",         Adapter = new WcfCustomIsolatedAdapter.Inbound<WSHttpBindingElement>() },
				new TestData { Name = "WcfWSHttp",         Adapter = new WcfCustomAdapter.Outbound<WSHttpBindingElement>() }
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

			public IAdapter Adapter { get; set; }

			public string Name { get; set; }
		}
	}
}
