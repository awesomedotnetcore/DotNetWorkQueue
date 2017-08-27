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
using System;
using System.Data;
using DotNetWorkQueue.Transport.RelationalDatabase.Basic.Command;
using DotNetWorkQueue.Validation;

namespace DotNetWorkQueue.Transport.RelationalDatabase.Basic.CommandPrepareHandler
{
    public class SetStatusTableStatusCommandPrepareHandler : IPrepareCommandHandler<SetStatusTableStatusCommand>
    {
        private readonly CommandStringCache _commandCache;

        public SetStatusTableStatusCommandPrepareHandler(CommandStringCache commandCache)
        {
            Guard.NotNull(() => commandCache, commandCache);
            _commandCache = commandCache;
        }
        public void Handle(SetStatusTableStatusCommand command, IDbCommand dbCommand, CommandStringTypes commandType)
        {
            dbCommand.CommandText = _commandCache.GetCommand(commandType);

            var queueId = dbCommand.CreateParameter();
            queueId.ParameterName = "@QueueID";
            queueId.DbType = DbType.Int64;
            queueId.Value = command.QueueId;
            dbCommand.Parameters.Add(queueId);

            var status = dbCommand.CreateParameter();
            status.ParameterName = "@Status";
            status.DbType = DbType.Int16;
            status.Value = Convert.ToInt16(command.Status);
            dbCommand.Parameters.Add(status);
        }
    }
}