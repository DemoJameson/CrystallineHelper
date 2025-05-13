using Celeste;
using Celeste.Mod;
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
    [Tracked]
    public class CustomWindController : Entity
    {
        public static void Load()
        {
            On.Celeste.WindController.Update += WindController_Update;
            On.Celeste.Refill.OnPlayer += Refill_OnPlayer;
            On.Celeste.Glider.OnPickup += Glider_OnPickup;
            On.Celeste.TheoCrystal.OnPickup += TheoCrystal_OnPickup;
        }

        public static void Unload()
        {
            On.Celeste.WindController.Update -= WindController_Update;
            On.Celeste.Refill.OnPlayer -= Refill_OnPlayer;
            On.Celeste.Glider.OnPickup -= Glider_OnPickup;
            On.Celeste.TheoCrystal.OnPickup -= TheoCrystal_OnPickup;
        }

        public CustomWindController(List<float> speedX, List<float> speedY, List<float> alternateSpeed, float catchupSpeed, string activateType, bool loop, bool persist)
        {
            active = false;
            Tag = Tags.TransitionUpdate;
            this.speedX = speedX;
            this.speedY = speedY;
            this.alternateSpeed = alternateSpeed;
            this.catchupSpeed = catchupSpeed;
            this.activateType = activateType;
            this.loop = loop;
            if (persist)
            {
                Tag |= Tags.Global | Tags.Persistent;
            }
        }

        public override void Update()
        {
            base.Update();
            CheckActivation();
            Level level = SceneAs<Level>();
            level.Wind = Calc.Approach(level.Wind, targetSpeed, catchupSpeed * 1000f * Engine.DeltaTime);
            if (level.Wind != Vector2.Zero && !level.Transitioning)
            {
                foreach (Component component in Scene.Tracker.GetComponents<WindMover>())
                {
                    WindMover windMover = component as WindMover;
                    windMover.Move(level.Wind * 0.1f * Engine.DeltaTime);
                }
            }
        }

        private void CheckActivation()
        {
            switch (activateType)
            {
                case "Seeds":
                    bool activate1 = false;
                    foreach (StrawberrySeed seed in Scene.Tracker.GetEntities<StrawberrySeed>())
                    {
                        if (seed.Collected)
                        {
                            activate1 = true;
                            break;
                        }
                    }
                    foreach (KeyBerrySeed seed in Scene.Tracker.GetEntities<KeyBerrySeed>())
                    {
                        if (seed.collected)
                        {
                            activate1 = true;
                            break;
                        }
                    }
                    if (activate1)
                    {
                        ActivateWind();
                    }
                    else
                    {
                        DeactivateWind();
                    }
                    break;
                case "Strawberry":
                    Player player = Scene.Tracker.GetEntity<Player>();
                    bool activate2 = false;
                    foreach (Follower follower in player.Leader.Followers)
                    {
                        if (follower.Entity is Strawberry)
                        {
                            Strawberry berry = follower.Entity as Strawberry;
                            if (!berry.Golden)
                            {
                                activate2 = true;
                                break;
                            }
                        }
                    }
                    if (activate2)
                    {
                        ActivateWind();
                    }
                    else
                    {
                        DeactivateWind();
                    }
                    break;
                case "Keyberry":
                    Player player2 = Scene.Tracker.GetEntity<Player>();
                    bool activate3 = false;
                    foreach (Follower follower in player2.Leader.Followers)
                    {
                        if (follower.Entity is KeyBerry)
                        {
                            activate3 = true;
                            break;
                        }
                    }
                    if (activate3)
                    {
                        ActivateWind();
                    }
                    else
                    {
                        DeactivateWind();
                    }
                    break;
                case "Locked Door":
                case "Refill":
                case "Jellyfish":
                case "Theo":
                    //handled inside hooks
                    break;
                case "Core Mode (Hot)":
                    if (SceneAs<Level>().CoreMode == Session.CoreModes.Hot)
                    {
                        ActivateWind();
                    }
                    else
                    {
                        DeactivateWind();
                    }
                    break;
                case "Core Mode (Cold)":
                    if (SceneAs<Level>().CoreMode == Session.CoreModes.Cold)
                    {
                        ActivateWind();
                    }
                    else
                    {
                        DeactivateWind();
                    }
                    break;
                case "Death":
                    Player player3 = Scene.Tracker.GetEntity<Player>();
                    if (player3 == null || player3.Dead)
                    {
                        ActivateWind();
                    }
                    break;
                default:
                    ActivateWind(true);
                    break;
            }
        }

        public static void Refill_OnPlayer(On.Celeste.Refill.orig_OnPlayer orig, Refill self, Player player)
        {
            orig(self, player);
            if (!self.Collidable)
            {
                CustomWindController wind = self.Scene.Tracker.GetEntity<CustomWindController>();
                if (wind != null && wind.activateType == "Refill")
                {
                    wind.ActivateWind();
                }
            }
        }

        public static void Glider_OnPickup(On.Celeste.Glider.orig_OnPickup orig, Glider self)
        {
            orig(self);
            CustomWindController wind = self.Scene.Tracker.GetEntity<CustomWindController>();
            if (wind != null && wind.activateType == "Jellyfish")
            {
                wind.ActivateWind();
            }
        }

        public static void TheoCrystal_OnPickup(On.Celeste.TheoCrystal.orig_OnPickup orig, TheoCrystal self)
        {
            orig(self);
            CustomWindController wind = self.Scene.Tracker.GetEntity<CustomWindController>();
            if (wind != null && wind.activateType == "Theo")
            {
                wind.ActivateWind();
            }
        }

        public static void WindController_Update(On.Celeste.WindController.orig_Update orig, WindController self)
        {
            if (self.Scene.Tracker.GetEntities<CustomWindController>().Count == 0)
            {
                orig(self);
            }
        }

        public void ActivateWind(bool debug = false)
        {
            if (active) return;
            active = true;
            if (debug && !string.IsNullOrEmpty(activateType))
            {
                Logger.Info("CrystallineHelper/CustomWindController", $"Custom wind doesn't have a activation case for: {activateType}");
            }
            if ((speedX.Count > 1 || speedY.Count > 1) && !alternateSpeed.Contains(0))
            {
                Add(coroutine = new Coroutine(WindRoutine()));
            }
            else
            {
                targetSpeed.X = speedX[0] * 100f;
                targetSpeed.Y = speedY[0] * 100f;
                SetAmbience();
            }
        }

        public void DeactivateWind()
        {
            if (!active) { return; }
            active = false;
            if (coroutine != null)
            {
                coroutine.RemoveSelf();
            }
            targetSpeed = Vector2.Zero;
            SetAmbience();
        }

        public void SnapWind()
        {
            if (coroutine != null && coroutine.Active)
            {
                coroutine.Update();
            }
            SceneAs<Level>().Wind = targetSpeed;
            SetAmbience();
            windIndex = 0;
        }

        private void SetAmbience()
        {
            if (targetSpeed != Vector2.Zero)
            {
                float angle = (float)Math.Atan2(targetSpeed.Y, targetSpeed.X);
                angle += (float)Math.PI / 4;
                angle %= (float)Math.PI * 2;
                if (angle < Math.PI)
                {
                    Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "wind_direction", 1f);
                }
                else
                {
                    Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "wind_direction", -1f);
                }
            }
            else
            {
                Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "wind_direction", 0f);
            }
            Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "strong_wind", targetSpeed.LengthSquared() > 640000f ? 1f : 0f);
        }

        private IEnumerator WindRoutine()
        {
            while (loop || windIndex < Math.Max(speedX.Count, speedY.Count))
            {
                targetSpeed.X = speedX[windIndex % speedX.Count] * 100f;
                targetSpeed.Y = speedY[windIndex % speedY.Count] * 100f;
                SetAmbience();
                yield return alternateSpeed[windIndex % alternateSpeed.Count];
                windIndex++;
            }
            RemoveSelf();
            yield break;
        }

        private bool active;

        private List<float> speedX;

        private List<float> speedY;

        private List<float> alternateSpeed;

        private float catchupSpeed;

        public string activateType;

        private bool loop;

        private int windIndex = 0;

        private Vector2 targetSpeed;

        private Coroutine coroutine;
    }
}