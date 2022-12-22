module HeartWarsGameStartButton
using ..Ahorn, Maple

@mapdef Entity "HeartWars/GameStartButton" GameStartButton(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Game Start Button (HeartWars)" => Ahorn.EntityPlacement(
        GameStartButton,
        "rectangle"
    )
)

function Ahorn.selection(entity::GameStartButton)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x, y - 4, 16, 12)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::GameStartButton, room::Maple.Room)
    texture = "objects/temple/dashButtonMirror00.png"
    Ahorn.drawSprite(ctx, texture, 27, 7, rot=pi / 2)
end

end