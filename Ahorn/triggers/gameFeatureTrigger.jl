module HeartWarsGameFeatureTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/GameFeatureTrigger" GameFeatureTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Game Feature Trigger (HeartWars)" => Ahorn.EntityPlacement(
        GameFeatureTrigger,
        "rectangle",
    )
)

end