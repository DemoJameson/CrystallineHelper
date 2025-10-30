module CustomWindTrigger

using ..Ahorn, Maple

const actTypes = ["", "Seeds", "Strawberry", "Keyberry", "Locked Door", "Refill", "Jellyfish", "Theo", "Core Mode (Hot)", "Core Mode (Cold)", "Death"]

@mapdef Trigger "vitellary/customwindtrigger" Wind(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, speedX::String="0", speedY::String="0", alternationSpeed::String="0", catchupSpeed::Number=1.0, activationType::String="", loop::Bool=true, persist::Bool=false, oneUse::Bool=false, onRoomEnter::Bool=false, fixUpdateDepth::Bool=true)

const placements = Ahorn.PlacementDict(
    "Custom Wind Trigger (Crystalline)" => Ahorn.EntityPlacement(
        Wind,
        "rectangle"
    )
)

Ahorn.editingOptions(entity::Wind) = Dict{String, Any}(
    "activationType" => actTypes
)
Ahorn.editingOrder(entity::Wind) = String["x", "y", "width", "height", "speedX", "speedY", "alternationSpeed", "catchupSpeed", "activationType", "onRoomEnter", "persist", "loop", "oneUse", "fixUpdateDepth"]

end