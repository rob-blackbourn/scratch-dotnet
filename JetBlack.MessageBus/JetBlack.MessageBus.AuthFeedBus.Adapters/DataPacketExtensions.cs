using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public static class DataPacketExtensions
    {
        public static DataPacket[] ToDataPacketArray<TValue>(this IDictionary<Guid, TValue> data)
        {
            return data.Select(x => new DataPacket(x.Key, x.Value)).ToArray();
        }
    }
}
