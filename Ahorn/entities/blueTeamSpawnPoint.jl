module HeartWarsBlueTeamSpawnPoint

using ..Ahorn, Maple

@mapdef Entity "HeartWars/BlueTeamSpawnPoint" BlueTeamSpawnPoint(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Blue Team SpawnPoint (HeartWars)" => Ahorn.EntityPlacement(
        BlueTeamSpawnPoint,
        "rectangle",
    )
)

end