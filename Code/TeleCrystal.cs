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
    [CustomEntity("vitellary/telecrystal", "vitellary/goodtelecrystal")]
    public class TeleCrystal : Entity
    {
        public TeleCrystal(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            string dirstr = "";
            if (data.Name == "vitellary/telecrystal")
            {
                switch (data.Int("direction"))
                {
                    case 0:
                        dirstr = "right"; break;
                    case 1:
                        dirstr = "down"; break;
                    case 2:
                        dirstr = "left"; break;
                    case 3:
                        dirstr = "up"; break;
                }
            }
            else
            {
                dirstr = data.Attr("direction").ToLower();
            }
            switch (dirstr)
            {
                case "right":
                    dirVector = Vector2.UnitX; break;
                case "down":
                    dirVector = Vector2.UnitY; break;
                case "left":
                    dirVector = -Vector2.UnitX; break;
                case "up":
                    dirVector = -Vector2.UnitY; break;
            }
            oneUse = data.Bool("oneUse", false);
            preventCrash = data.Bool("preventCrash", true);
            useTime = data.Float("respawnTime", 0.2f);
            resetSpeed = data.Bool("resetSpeed", true);

            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
            if (!oneUse)
            {
                Add(outline = new Image(GFX.Game["objects/crystals/tele/" + dirstr + "/outline"]));
                outline.CenterOrigin();
                outline.Visible = false;
            }
            string spritetype = oneUse ? "idlenr" : "idle";
            Add(sprite = new Sprite(GFX.Game, "objects/crystals/tele/" + dirstr + "/" + spritetype));
            sprite.AddLoop(spritetype, "", 0.1f);
            sprite.Play(spritetype, false, false);
            sprite.CenterOrigin();
            Add(flash = new Sprite(GFX.Game, "objects/crystals/tele/" + dirstr + "/flash"));
            flash.Add("flash", "", 0.05f);
            flash.OnFinish = delegate (string anim)
            {
                flash.Visible = false;
            };
            flash.CenterOrigin();
            Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v)
            {
                sprite.Scale = (flash.Scale = Vector2.One * (1f + v * 0.2f));
            }, false, false));
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f));
            sine.Randomize();
            UpdateY();
            Depth = -100;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            else if (Scene.OnInterval(0.1f))
            {
                level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
            }
            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            if (Scene.OnInterval(2f) && sprite.Visible)
            {
                flash.Play("flash", true, false);
                flash.Visible = true;
            }
        }

        private void Respawn()
        {
            if (!Collidable)
            {
                Collidable = true;
                sprite.Visible = true;
                outline.Visible = false;
                Depth = -100;
                wiggler.Start();
                Audio.Play("event:/game/general/diamond_return", Position);
                level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateY()
        {
            flash.Y = (sprite.Y = (bloom.Y = sine.Value * 2f));
        }

        public override void Render()
        {
            if (sprite.Visible)
            {
                sprite.DrawOutline(1);
            }
            base.Render();
        }

        private void OnPlayer(Player player)
        {
            player.MoveHExact((int)dirVector.X * (preventCrash ? SceneAs<Level>().Bounds.Width : int.MaxValue));
            player.MoveVExact((int)dirVector.Y * (preventCrash ? SceneAs<Level>().Bounds.Height : int.MaxValue));
            if (dirVector == Vector2.UnitY)
            {
                player.MoveVExact(-2);
            }
            if (resetSpeed)
            {
                if (dirVector.X != 0)
                {
                    player.Speed.X = 0;
                }
                else
                {
                    player.Speed.Y = 0;
                }
            }
            player.StateMachine.State = 0;
            Audio.Play("event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player), true));
            if (oneUse)
            {
                RemoveSelf();
            }
            else
            {
                respawnTimer = useTime;
            }
        }

        private IEnumerator RefillRoutine(Player player)
        {
            Celeste.Celeste.Freeze(0.05f);
            yield return null;
            level.Shake(0.3f);
            sprite.Visible = (flash.Visible = false);
            if (!oneUse)
            {
                outline.Visible = true;
            }
            Depth = 8999;
            yield return 0.05f;
            float num = player.Speed.Angle();
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num - 1.57079637f);
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num + 1.57079637f);
            SlashFx.Burst(Position, num);
            yield break;
        }

        private bool oneUse;

        private bool preventCrash;

        private float useTime;

        private bool resetSpeed;

        private Vector2 dirVector;

        private float respawnTimer;

        public static ParticleType P_Shatter;

        public static ParticleType P_Regen;

        public static ParticleType P_Glow;

        private Sprite sprite;

        private Sprite flash;

        private Image outline;

        private Wiggler wiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private Level level;

        private SineWave sine;
    }
}
