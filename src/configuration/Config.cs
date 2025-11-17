using Newtonsoft.Json;
using ProtoBuf;

namespace playerlist.configuration;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Config {
    public int[]? Thresholds { get; set; } = [100, 250, 500];

    public string? Logo { get; set; } = "playerlist:textures/vs_logo.png";

    public string? Header { get; set; }

    public string? Footer { get; set; }

    public int? MaxNameLength { get; set; } = 20;

    [JsonIgnore] public int? MaxPlayers { get; set; }
}
