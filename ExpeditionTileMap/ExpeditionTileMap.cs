using Shared;

namespace ExpeditionTileMap;

public class ExpeditionTileMap
{
    public Dictionary<V2I, ExpeditionTile> TilesByPosition { get; } = new();
    public Dictionary<V2I, ExpeditionRelay> RelaysByPosition { get; } = new();
    public Dictionary<V2I, ExpeditionEncounter> EncountersByPosition { get; } = new();
    public Dictionary<V2I, ExpeditionWire> WiresByPosition { get; } = new();
    public HashSet<V2I> DirtyPositions { get; } = new();
    public List<V2I> ProposedPath { get; } = new();
    public HashSet<V2I> ProposedRelays { get; } = new();
    public List<V2I> RelayOrder { get; } = new();

    public IReadOnlyCollection<ExpeditionTile> Tiles => TilesByPosition.Values;
    public IReadOnlyCollection<ExpeditionRelay> Relays => RelaysByPosition.Values;
    public IReadOnlyCollection<ExpeditionEncounter> Encounters => EncountersByPosition.Values;
    public IReadOnlyCollection<ExpeditionWire> Wires => WiresByPosition.Values;
    public V2I? CurrentPosition { get; set; }
    public int CompletedProposedPathIndex { get; set; } = -1;
    public int PowerUsage { get; set; }
    public int PowerLimit { get; set; } = 10;
    public long Version { get; set; }
    public long StructureVersion { get; set; }
    public long TileVersion { get; set; }
    public long RelayVersion { get; set; }
    public long RelayOrderVersion { get; set; }
    public long EncounterVersion { get; set; }
    public long ProposedPathVersion { get; set; }
    public long ProposedRelaysVersion { get; set; }
    public long ProposedPathProgressVersion { get; set; }
    public long PowerVersion { get; set; }
    public long WireVersion { get; set; }
}

