module HeartWarsDashEnableTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/DashEnableTrigger" DashEnableTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Dash Enable Trigger (HeartWars)" => Ahorn.EntityPlacement(
        DashEnableTrigger,
        "rectangle",
    )
)

end