local utils = require "utils"
local drawableSprite = require "structs.drawable_sprite"

local keyberry = {}

keyberry.name = "vitellary/keyberry"
keyberry.depth = -100

keyberry.nodeLineRenderType = "fan"
keyberry.nodeLimits = {0, -1}

keyberry.placements = {
    {
        name = "normal",
        data = {
            winged = false,
            collectTime = 0.15,
            persistent = true,
        }
    },
    {
        name = "winged",
        data = {
            winged = true,
            collectTime = 0.15,
            persistent = true,
        }
    }
}

function keyberry.sprite(room, entity)
    local winged = entity.winged

    local texture = winged and "collectables/keyberry/wings00" or "collectables/keyberry/normal03"
    local sprite = drawableSprite.fromTexture(texture, entity)
    if winged then
        sprite:addPosition(0, 1)
    end

    return sprite
end

keyberry.nodeTexture = "collectables/keyberry/seed00"

function keyberry.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local nodes = entity.nodes or {}

    local rects = {}
    for _, node in ipairs(nodes) do
        table.insert(rects, utils.rectangle(node.x - 5, node.y - 4, 9, 10))
    end

    return utils.rectangle(x - 8, y - 8, 16, 16), rects
end

return keyberry
