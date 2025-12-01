local utils = require "utils"
local drawableSprite = require "structs.drawable_sprite"

local energyBooster = {}

energyBooster.name = "vitellary/energybooster"
energyBooster.placements = {
    {
        name = "normal",
        data = {
            behaveLikeDash = false,
            redirectSpeed = false,
            oneUse = false,
            setLastBooster = true,
        },
    },
    {
        name = "redirect",
        data = {
            behaveLikeDash = false,
            redirectSpeed = true,
            oneUse = false,
            setLastBooster = true,
        },
    },
}

energyBooster.fieldOrder = {
    "x", "y",
    "redirectSpeed", "behaveLikeDash", "oneUse", "setLastBooster",
}

function energyBooster.sprite(room, entity)
    return drawableSprite.fromTexture(entity.redirectSpeed and "objects/CrystallineHelper/FLCC/energyBoosterRedirect/booster00" or "objects/CrystallineHelper/FLCC/energyBooster/booster00", entity)
end

function energyBooster.selection(room, entity)
    return utils.rectangle(entity.x - 10, entity.y - 10, 20, 20)
end

return energyBooster