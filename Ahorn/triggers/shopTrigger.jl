module HeartWarsShopTrigger

using ..Ahorn, Maple

@mapdef Trigger "HeartWars/ShopTrigger" ShopTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight
)

const placements = Ahorn.PlacementDict(
    "Shop Trigger (HeartWars)" => Ahorn.EntityPlacement(
        ShopTrigger,
        "rectangle",
    )
)

end