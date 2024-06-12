using ProtoBuf;

namespace playerlist.configuration;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Config {
    public int[]? Thresholds { get; set; } = { 65, 125, 500 };
}
