local roomNameTrigger = {}

roomNameTrigger.name = "vitellary/roomnametrigger"

roomNameTrigger.placements = {
    {
        name = "room_name_trigger",
        data = {
            roomName = "",
            backgroundColor = "000000FF",
            textColor = "FFFFFFFF",
            disappearTimer = -1,
            outlineColor = "000000FF",
            outlineThickness = 0,
            scale = 1,
            oneUse = false,
            instant = false,
            offset = 0,
        }
    }
}
roomNameTrigger.fieldInformation = {
    backgroundColor = {
        fieldType = "color"
    },
    textColor = {
        fieldType = "color"
    },
    outlineColor = {
        fieldType = "color"
    },
    scale = {
        minimumValue = 0.01,
    },
}

return roomNameTrigger
