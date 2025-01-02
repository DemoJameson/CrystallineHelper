local coyoteBounceTrigger = {}

coyoteBounceTrigger.name = "vitellary/coyotebounce"

coyoteBounceTrigger.fieldInformation = {
    time = {
        minimumValue = 0.0
    },
    directions = {
        options = {"None", "Top", "TopAndSides", "AllDirections"},
        editable = false,
    },
    dashTypes = {
        options = {"All", "Rebound", "Bounce", "Normal", "Ignore"},
        editable = false,
    },
    refillDirections = {
        options = {"MatchCoyote", "None", "Top", "TopAndSides", "AllDirections"},
        editable = false,
    },
}

coyoteBounceTrigger.placements = {
    {
        name = "coyote_bounce",
        data = {
            directions = "Top",
            time = 0.1,
            refillDirections = "MatchCoyote",
            dashTypes = "Rebound",
            setGrounded = false,
            refillDashes = true,
            refillStamina = true,
        }
    }
}

return coyoteBounceTrigger
