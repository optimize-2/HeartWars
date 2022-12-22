module HeartWarsRedTeamTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/RedTeamTrigger" RedTeamTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Red Team Trigger (HeartWars)" => Ahorn.EntityPlacement(
        RedTeamTrigger,
        "rectangle",
    )
)

end