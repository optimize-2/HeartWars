module HeartWarsResourceGeneratorIII

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/ResourceGeneratorIII" ResourceGeneratorIII(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Resource Generator Level III (HeartWars)" => Ahorn.EntityPlacement(
        ResourceGeneratorIII,
        "rectangle",
    )
)

end