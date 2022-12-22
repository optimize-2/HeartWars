module HeartWarsRedTeamSpawnPoint

using ..Ahorn, Maple

@mapdef Entity "HeartWars/RedTeamSpawnPoint" RedTeamSpawnPoint(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Red Team SpawnPoint (HeartWars)" => Ahorn.EntityPlacement(
        RedTeamSpawnPoint,
        "rectangle",
    )
)

end