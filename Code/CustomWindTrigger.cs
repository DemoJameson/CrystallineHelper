using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    [CustomEntity("vitellary/customwindtrigger")]
    public class CustomWindTrigger : Trigger
    {
        public CustomWindTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            speedX = new List<float>();
            string[] speedXStrings = data.Attr("speedX", "0").Split(',');
            foreach (string sx in speedXStrings)
            {
                speedX.Add(float.Parse(sx));
            }
            speedY = new List<float>();
            string[] speedYStrings = data.Attr("speedY", "0").Split(',');
            foreach (string sy in speedYStrings)
            {
                speedY.Add(float.Parse(sy));
            }
            alternateSpeed = new List<float>();
            string[] alternateSpeedStrings = data.Attr("alternationSpeed", "0").Split(',');
            foreach (string sa in alternateSpeedStrings)
            {
                alternateSpeed.Add(float.Parse(sa));
            }
            catchupSpeed = data.Float("catchupSpeed", 1f);
            activateType = data.Attr("activationType", "");
            loop = data.Bool("loop", true);
            persist = data.Bool("persist", false);
            oneUse = data.Bool("oneUse", false);
            ID = data.ID;
            onRoomEnter = data.Bool("onRoomEnter", false);
            fixUpdateDepth = data.Bool("fixUpdateDepth", false);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (onRoomEnter)
            {
                OnEnter(scene.Tracker.GetEntity<Player>());
            }
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            WindController wind = Scene.Entities.FindFirst<WindController>();
            if (wind != null)
            {
                wind.RemoveSelf();
            }
            CustomWindController customWind = Scene.Entities.FindFirst<CustomWindController>();
            if (customWind != null)
            {
                customWind.RemoveSelf();
            }
            customWind = new CustomWindController(speedX, speedY, alternateSpeed, catchupSpeed, activateType, loop, persist, fixUpdateDepth);
            Scene.Add(customWind);
            if (oneUse)
            {
                RemoveSelf();
                if (persist)
                {
                    Session session = SceneAs<Level>().Session;
                    session.DoNotLoad.Add(new EntityID(session.Level, ID));
                }
            }
        }

        private List<float> speedX;

        private List<float> speedY;

        private List<float> alternateSpeed;

        private float catchupSpeed;

        private string activateType;

        private bool loop;

        private bool persist;

        private bool oneUse;

        private int ID;

        private bool onRoomEnter;

        private bool fixUpdateDepth;
    }
}
