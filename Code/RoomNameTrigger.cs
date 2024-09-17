﻿using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Celeste.Mod.Code.Entities
{
    [CustomEntity("vitellary/roomnametrigger")]
    [Tracked]
    public class RoomNameTrigger : Trigger
    {
        public string Name;
        private string bgColor;
        private string textColor;
        private float timer;
        private bool oneUse;
        private bool instant;

        public RoomNameTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Name = data.Attr("roomName");
            bgColor = data.Attr("backgroundColor", "000000FF");
            textColor = data.Attr("textColor", "FFFFFFFF");
            timer = data.Float("disappearTimer", -1f);
            oneUse = data.Bool("oneUse", false);
            instant = data.Bool("instant", false);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            var display = RoomNameDisplay.GetDisplay(player.Scene);
            display.SetName(Name);
            display.SetColor(Calc.HexToColorWithAlpha(textColor), Calc.HexToColorWithAlpha(bgColor));
            display.SetTimer(Math.Max(timer, 0f));
            if (instant)
            {
                display.SetInstant();
            }
            if (oneUse)
            {
                RemoveSelf();
            }
        }
    }
}
