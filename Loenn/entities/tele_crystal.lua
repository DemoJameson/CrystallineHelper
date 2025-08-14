local directions = {
    "Up",
    "Down",
    "Left",
    "Right"
}

local teleCrystal = {}

teleCrystal.name = "vitellary/goodtelecrystal"
teleCrystal.depth = -100

teleCrystal.fieldInformation = {
    respawnTime = {
        minimumValue = 0.0
    },
    direction = {
        editable = false,
        options = directions
    }
}

teleCrystal.placements = {}
for i, dir in ipairs(directions) do
    teleCrystal.placements[i] = {
        name = string.lower(dir),
        data = {
            direction = dir,
            oneUse = false,
            preventCrash = true,
            respawnTime = 0.2,
            resetSpeed = true,
        }
    }
end

local crystalTextures = {
    ["Right"] = "objects/crystals/tele/right/idle00",
    ["Down"] = "objects/crystals/tele/down/idle00",
    ["Left"] = "objects/crystals/tele/left/idle00",
    ["Up"] = "objects/crystals/tele/up/idle00"
}

function teleCrystal.texture(room, entity)
    return crystalTextures[entity.direction] or crystalTextures["Right"]
end

return teleCrystal
