﻿using Byond.TopicSender;
using Microsoft.Extensions.Logging;
using System;
using Tgstation.Server.Api.Models.Internal;
using Tgstation.Server.Host.Components.Chat;
using Tgstation.Server.Host.Components.Compiler;
using Tgstation.Server.Host.Core;

namespace Tgstation.Server.Host.Components.Watchdog
{
	/// <inheritdoc />
	sealed class WatchdogFactory : IWatchdogFactory
	{
		/// <summary>
		/// The <see cref="IServerControl"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly IServerControl serverControl;

		/// <summary>
		/// The <see cref="ILoggerFactory"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly ILoggerFactory loggerFactory;

		/// <summary>
		/// The <see cref="IDatabaseContextFactory"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly IDatabaseContextFactory databaseContextFactory;

		/// <summary>
		/// The <see cref="IByondTopicSender"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly IByondTopicSender byondTopicSender;

		/// <summary>
		/// The <see cref="IJobManager"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly IJobManager jobManager;

		/// <summary>
		/// The <see cref="IAsyncDelayer"/> for the <see cref="WatchdogFactory"/>
		/// </summary>
		readonly IAsyncDelayer asyncDelayer;

		/// <summary>
		/// Construct a <see cref="WatchdogFactory"/>
		/// </summary>
		/// <param name="serverControl">The value of <see cref="serverControl"/></param>
		/// <param name="loggerFactory">The value of <see cref="loggerFactory"/></param>
		/// <param name="databaseContextFactory">The value of <see cref="databaseContextFactory"/></param>
		/// <param name="byondTopicSender">The value of <see cref="byondTopicSender"/></param>
		/// <param name="jobManager">The value of <see cref="jobManager"/></param>
		/// <param name="asyncDelayer">The value of <see cref="asyncDelayer"/></param>
		public WatchdogFactory(IServerControl serverControl, ILoggerFactory loggerFactory, IDatabaseContextFactory databaseContextFactory, IByondTopicSender byondTopicSender, IJobManager jobManager, IAsyncDelayer asyncDelayer)
		{
			this.serverControl = serverControl ?? throw new ArgumentNullException(nameof(serverControl));
			this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			this.databaseContextFactory = databaseContextFactory ?? throw new ArgumentNullException(nameof(databaseContextFactory));
			this.byondTopicSender = byondTopicSender ?? throw new ArgumentNullException(nameof(byondTopicSender));
			this.jobManager = jobManager ?? throw new ArgumentNullException(nameof(jobManager));
			this.asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
		}

		/// <inheritdoc />
		public IWatchdog CreateWatchdog(IChat chat, IDmbFactory dmbFactory, IReattachInfoHandler reattachInfoHandler, IEventConsumer eventConsumer, ISessionControllerFactory sessionControllerFactory, Api.Models.Instance instance, DreamDaemonSettings settings) => new Watchdog(chat, sessionControllerFactory, dmbFactory, reattachInfoHandler, databaseContextFactory, byondTopicSender, eventConsumer, jobManager, serverControl, asyncDelayer, loggerFactory.CreateLogger<Watchdog>(), settings, instance, settings.AutoStart.Value);
	}
}
