module HeartWarsDefaultSpawnPoint

using ..Ahorn, Maple

@mapdef Entity "HeartWars/DefaultSpawnPoint" DefaultSpawnPoint(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Default SpawnPoint (HeartWars)" => Ahorn.EntityPlacement(
        DefaultSpawnPoint,
        "rectangle",
    )
)

end