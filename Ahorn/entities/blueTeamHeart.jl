module HeartWarsBlueTeamHeart
using ..Ahorn, Maple

@mapdef Entity "HeartWars/BlueTeamHeart" BlueTeamHeart(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Blue Team Heart (HeartWars)" => Ahorn.EntityPlacement(
        BlueTeamHeart
    )
)

const sprite = "collectables/heartGem/0/00.png"

function Ahorn.selection(entity::BlueTeamHeart)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BlueTeamHeart, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end