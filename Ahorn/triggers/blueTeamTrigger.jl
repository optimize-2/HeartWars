module HeartWarsBlueTeamTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/BlueTeamTrigger" BlueTeamTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Blue Team Trigger (HeartWars)" => Ahorn.EntityPlacement(
        BlueTeamTrigger,
        "rectangle",
    )
)

end