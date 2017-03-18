﻿// ---------------------------------------------------------------------
//This file is part of DotNetWorkQueue
//Copyright © 2017 Brian Lehnen
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// ---------------------------------------------------------------------
using DotNetWorkQueue.Configuration;
using DotNetWorkQueue.Logging;
using DotNetWorkQueue.Validation;

namespace DotNetWorkQueue.Queue
{
    /// <summary>
    /// Contains information for user code execution
    /// </summary>
    public class WorkerNotification : IWorkerNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerNotification" /> class.
        /// </summary>
        /// <param name="headerNames">The header names.</param>
        /// <param name="cancelWork">The cancel work.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="log">The log.</param>
        /// <param name="metrics">The metrics factory.</param>
        public WorkerNotification(IHeaders headerNames,
            IQueueCancelWork cancelWork,
            TransportConfigurationReceive configuration,
            ILogFactory log,
            IMetrics metrics)
        {
            Guard.NotNull(() => headerNames, headerNames);
            Guard.NotNull(() => cancelWork, cancelWork);
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => log, log);
            Guard.NotNull(() => metrics, metrics);

            HeaderNames = headerNames;
            WorkerStopping = cancelWork;
            TransportSupportsRollback = configuration.MessageRollbackSupported;
            Log = log.Create();
            Metrics = metrics;
        }

        /// <summary>
        /// Provides status on if the queue is shutting down.
        /// </summary>
        /// <value>
        /// The worker stopping.
        /// </value>
        public ICancelWork WorkerStopping { get; set; }

        /// <summary>
        /// The heart beat status.
        /// </summary>
        /// <value>
        /// The heart beat.
        /// </value>
        public IWorkerHeartBeatNotification HeartBeat { get; set; }

        /// <summary>
        /// Gets the header names.
        /// </summary>
        /// <value>
        /// The header names.
        /// </value>
        public IHeaders HeaderNames { get; set; }

        /// <summary>
        /// If true, the transport being used supports rolling back the de-queue operation.
        /// </summary>
        /// <value>
        /// <c>true</c> if [transport supports rollback]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If this value is false, canceling an in progress operation may result in data loss.
        /// If using a transport not configured for rollback, re-queuing or saving state is up to user code.
        /// </remarks>
        public bool TransportSupportsRollback { get; }

        /// <summary>
        /// An instance of the logging class
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public ILog Log { get; }

        /// <summary>
        /// Allows logging of metrics
        /// </summary>
        /// <value>
        /// The metrics.
        /// </value>
        public IMetrics Metrics { get; }
    }
}
