local activationTypes = {
    "",
    "Seeds",
    "Strawberry",
    "Keyberry",
    "Locked Door",
    "Refill",
    "Jellyfish",
    "Theo",
    "Core Mode (Hot)",
    "Core Mode (Cold)",
    "Death"
}

local customWindTrigger = {}

customWindTrigger.name = "vitellary/customwindtrigger"

customWindTrigger.fieldInformation = {
    activationType = {
        editable = false,
        options = activationTypes
    }
}

customWindTrigger.placements = {
    {
        name = "custom_wind_trigger",
        data = {
            speedX = "0",
            speedY = "0",
            alternationSpeed = "0",
            catchupSpeed = 1.0,
            activationType = "",
            loop = true,
            persist = false,
            oneUse = false,
            onRoomEnter = false,
            fixUpdateDepth = true
        }
    }
}

return customWindTrigger
