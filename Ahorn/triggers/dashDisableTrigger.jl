module HeartWarsDashDisableTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/DashDisableTrigger" DashDisableTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Dash Enable Trigger (HeartWars)" => Ahorn.EntityPlacement(
        DashDisableTrigger,
        "rectangle",
    )
)

end