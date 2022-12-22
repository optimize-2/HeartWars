module HeartWarsResourceGeneratorII

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/ResourceGeneratorII" ResourceGeneratorII(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Resource Generator Level II (HeartWars)" => Ahorn.EntityPlacement(
        ResourceGeneratorII,
        "rectangle",
    )
)

end