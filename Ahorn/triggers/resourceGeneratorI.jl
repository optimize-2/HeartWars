module HeartWarsResourceGeneratorI

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/ResourceGeneratorI" ResourceGeneratorI(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Resource Generator Level I (HeartWars)" => Ahorn.EntityPlacement(
        ResourceGeneratorI,
        "rectangle",
    )
)

end