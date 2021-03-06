﻿using System.Threading.Tasks;
using StackExchange.Redis;

namespace DotNetWorkQueue.Transport.Redis.Basic.Lua
{
    /// <inheritdoc />
    /// <summary>
    /// Enqueues a message that is delayed
    /// </summary>
    internal class EnqueueDelayedLua : BaseLua
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="EnqueueDelayedLua"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="redisNames">The redis names.</param>
        public EnqueueDelayedLua(IRedisConnection connection, RedisNames redisNames)
            : base(connection, redisNames)
        {
            Script = @" local id = @field
                    
                     if id == '' then
                        id = redis.call('INCR', @IDKey)
                     end
                     redis.call('hset', @key, id, @value) 
                     redis.call('hset', @headerskey, id, @headers) 
                     redis.call('zadd', @delaykey, @timestamp, id) 
                     redis.call('hset', @metakey, id, @metavalue) 
                     redis.call('hset', @StatusKey, id, '0') 

                     if @Route ~= '' then
                         redis.call('hset', @RouteIDKey, id, @Route)
                     end
                     return id";
        }
        /// <summary>
        /// Enqueues a message that is delayed
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="unixTime">The delay time in unix format.</param>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        public string Execute(string messageId, byte[] message, byte[] headers, byte[] metaData, long unixTime, string route)
        {
            if (Connection.IsDisposed)
                return null;

            var db = Connection.Connection.GetDatabase();
            return (string)db.ScriptEvaluate(LoadedLuaScript, GetParameters(messageId, message, headers, metaData, unixTime, route));
        }
        /// <summary>
        /// Enqueues a message that is delayed
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="unixTime">The delay time in unix format.</param>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        public async Task<string> ExecuteAsync(string messageId, byte[] message, byte[] headers, byte[] metaData, long unixTime, string route)
        {
            var db = Connection.Connection.GetDatabase();
            return (string) await db.ScriptEvaluateAsync(LoadedLuaScript, GetParameters(messageId, message, headers, metaData, unixTime, route)).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="unixTime">The unix time.</param>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        private object GetParameters(string messageId, byte[] message, byte[] headers, byte[] metaData, long unixTime, string route)
        {
            var realRoute = string.IsNullOrEmpty(route) ? string.Empty : route;
            return
             new
             {
                 key = (RedisKey)RedisNames.Values,
                 field = messageId,
                 value = message,
                 headers,
                 Route = realRoute,
                 headerskey = (RedisKey)RedisNames.Headers,
                 delaykey = (RedisKey)RedisNames.Delayed,
                 channel = RedisNames.Notification,
                 metakey = (RedisKey)RedisNames.MetaData,
                 metavalue = metaData,
                 timestamp = unixTime,
                 IDKey = (RedisKey)RedisNames.Id,
                 StatusKey = (RedisKey)RedisNames.Status,
                 RouteIDKey = (RedisKey)RedisNames.Route
             };
        }
    }
}
