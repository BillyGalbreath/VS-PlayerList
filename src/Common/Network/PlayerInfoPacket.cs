using ProtoBuf;
using System.Collections.Generic;

namespace PlayerList.Common.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class PlayerInfoPacket {
    public Dictionary<string, float> pings = new();

    public override string ToString() {
        return $"PlayerInfoPacket[{(pings == null ? "null" : string.Join(", ", pings))}]";
    }
}
