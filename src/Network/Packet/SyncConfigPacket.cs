using ProtoBuf;

namespace PlayerList.Network.Packet;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class SyncConfigPacket : Packet {
    public int[]? Thresholds;
}
