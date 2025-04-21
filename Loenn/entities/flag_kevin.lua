local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local kevin = {}

kevin.name = "vitellary/flagkevin"
kevin.depth = 0
kevin.minimumSize = {24, 24}

kevin.placements = {
    {
        name = "normal",
        data = {
            width = 24,
            height = 24,
            axes = "both",
            flagDirection = "Right",
            flag = "",
            customPath = "crushblock",
            fillColor = "62222b",
            inverted = false,
            chillout = false,
            lavaSpeed = 1,
            crushSpeed = 240,
            returnSpeed = 60,
            crushAccel = 500,
            returnAccel = 160,
            playerCanHit = true,
            repeatWhileFlag = false,
            setFlagOnHit = false,
            useVanillaParticles = false,
        }
    },
    {
        name = "lava_aware",
        data = {
            width = 24,
            height = 24,
            axes = "both",
            flagDirection = "Right",
            flag = "",
            customPath = "crushblock",
            fillColor = "62222b",
            inverted = false,
            chillout = false,
            lavaSpeed = 0.5,
            crushSpeed = 240,
            returnSpeed = 60,
            crushAccel = 500,
            returnAccel = 160,
            playerCanHit = true,
            repeatWhileFlag = false,
            setFlagOnHit = false,
            useVanillaParticles = false,
        }
    },
}

local axesOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal"
}
local moveDirections = {
    "Up", "Down", "Left", "Right"
}
kevin.fieldInformation = {
    axes = {
        options = axesOptions,
        editable = false,
    },
    flagDirection = {
        options = moveDirections,
        editable = false,
    },
    fillColor = {
        fieldType = "color"
    }
}

local frameTextures = {
    none = "/block00",
    horizontal = "/block01",
    vertical = "/block02",
    both = "/block03"
}

local ninePatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local defaultPath = "objects/crushblock"
local defaultColor = {98 / 255, 34 / 255, 43 / 255}
local smallFaceTexture = "/idle_face"
local giantFaceTexture = "/giant_block00"

function kevin.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local axes = entity.axes or "both"
    local chillout = entity.chillout

    local path = (entity.customPath ~= nil and entity.customPath ~= "") and ("objects/" .. entity.customPath) or defaultPath
    local kevinColor = utils.getColor(entity.fillColor or defaultColor)

    local giant = height >= 48 and width >= 48 and chillout
    local faceTexture = path .. (giant and giantFaceTexture or smallFaceTexture)

    local frameTexture = path .. (frameTextures[axes] or frameTextures["both"])
    local ninePatch = drawableNinePatch.fromTexture(frameTexture, ninePatchOptions, x, y, width, height)

    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, kevinColor)
    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)

    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()

    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)

    return sprites
end

return kevin