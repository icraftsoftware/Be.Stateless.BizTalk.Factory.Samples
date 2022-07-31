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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Schema;
using Microsoft.XLANGs.BaseTypes;
using BizTalkFactoryProperties = Be.Stateless.BizTalk.ContextProperties.Subscribable.BizTalkFactoryProperties;

namespace Org.Anization.Accounting
{
	public class TaxEvadingShellCompanySendPort : SendPort
	{
		public TaxEvadingShellCompanySendPort()
		{
			Name = "TaxEvadingShellCompany";
			SendPipeline = new SendPipeline<XmlTransmit>();
			OrderedDelivery = true;
			Priority = Priority.Normal;

			Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\tax-evasion"; });
			Transport.Host = "BizTalkServerApplication";
			Transport.RetryPolicy = new RetryPolicy {
				Count = 3,
				Interval = TimeSpan.FromHours(1)
			};
			Transport.ServiceWindow = new ServiceWindow { StartTime = new Time(11, 15), StopTime = new Time(23, 15) };

			BackupTransport.Value.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\tax-evasion-backup-plan"; });
			BackupTransport.Value.Host = "BizTalkServerApplication";
			BackupTransport.Value.RetryPolicy = RetryPolicy.Default;
			BackupTransport.Value.ServiceWindow = new ServiceWindow { StartTime = new Time(1, 0), StopTime = new Time(23, 0) };

			Filter = new Filter(
				() =>
					(BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType || BtsProperties.ReceivePortName == ApplicationBinding.ReceivePorts.Find<EvilBankReceivePort>().Name)
					&& BizTalkFactoryProperties.EnvironmentTag == TargetEnvironment.ACCEPTANCE
			);
		}
	}
}
