module HeartWarsRedTeamHeart
using ..Ahorn, Maple

@mapdef Entity "HeartWars/RedTeamHeart" RedTeamHeart(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Red Team Heart (HeartWars)" => Ahorn.EntityPlacement(
        RedTeamHeart
    )
)

const sprite = "collectables/heartGem/1/00.png"

function Ahorn.selection(entity::RedTeamHeart)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::RedTeamHeart, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end