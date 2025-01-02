local roomName = {}

roomName.name = "vitellary/roomname"
roomName.depth = -100

roomName.placements = {
    {
        name = "room_name",
        data = {
            roomName = "",
            backgroundColor = "000000FF",
            textColor = "FFFFFFFF",
            disappearTimer = -1,
            outlineColor = "000000FF",
            outlineThickness = 0,
            scale = 1,
            offset = 0,
        }
    }
}
roomName.fieldInformation = {
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

roomName.texture = "ahorn_roomname"

return roomName
