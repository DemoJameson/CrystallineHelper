using Celeste;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using Celeste.Mod;
using Mono.Cecil.Cil;

namespace vitmod {
    [CustomEntity("vitellary/coyotebounce")]
    [Tracked]
    public class CoyoteBounceTrigger : Trigger {
        public static void Load() {
            On.Celeste.Actor.OnGround_int += Actor_OnGround_int;
			IL.Celeste.Player.OnCollideH += Player_OnCollideHV;
            IL.Celeste.Player.OnCollideV += Player_OnCollideHV;
		}

        private static bool Actor_OnGround_int(On.Celeste.Actor.orig_OnGround_int orig, Actor self, int downCheck) {
            var result = orig(self, downCheck);
            if (self is Player && CoyoteBounceTrigger.GroundedOverride) {
                result = true;
            }
            return result;
        }

		private static void Player_OnCollideHV(ILContext il)
		{
			var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<DashCollision>("Invoke")))
                return;

            var index = cursor.Index;

            int dataArg = -1;

            if (!cursor.TryGotoPrev(MoveType.After,
                instr => instr.MatchLdarg(out dataArg),
                instr => instr.MatchLdfld<CollisionData>("Direction")))
                return;

            cursor.Index = index;

			Logger.Log("CrystallineHelper/CoyoteBounceTrigger", "Adding Player.OnCollideH/V hook");

            cursor.Emit(OpCodes.Dup);
            cursor.Emit(OpCodes.Ldarg_0);
			cursor.Emit(OpCodes.Ldarg, dataArg);
			cursor.Emit<CollisionData>(OpCodes.Ldfld, "Direction");
            cursor.EmitDelegate<Action<DashCollisionResults, Player, Vector2>>(HookDashCollision);
        }

        private static void HookDashCollision(DashCollisionResults result, Player player, Vector2 direction)
        {
			CoyoteBounceTrigger bouncer = CoyoteBounceTrigger.CoyoteTriggerInside;

            if (bouncer == null)
                return;

            if (!CoyoteBounceTrigger.MatchDashType(result, bouncer.types))
                return;

            if (CoyoteBounceTrigger.MatchDirection(direction, bouncer.directions)) {
                player.jumpGraceTimer = bouncer.time;
                if (bouncer.setGrounded) {
                    CoyoteBounceTrigger.GroundedOverride = true;
                }
            }

			if (CoyoteBounceTrigger.MatchDirection(direction, bouncer.refill))
			{
                if (bouncer.dashes)
                {
                    player.RefillDash();
                }
                if (bouncer.stamina)
                {
                    player.RefillStamina();
                }
			}
		}

        public static void Unload() {
            On.Celeste.Actor.OnGround_int -= Actor_OnGround_int;
			IL.Celeste.Player.OnCollideH -= Player_OnCollideHV;
			IL.Celeste.Player.OnCollideV -= Player_OnCollideHV;
		}

		public BounceDirections directions;
        public DashTypes types;
		public float time;
        public BounceDirections refill;
        public bool setGrounded;
        public bool dashes;
        public bool stamina;

        public static CoyoteBounceTrigger CoyoteTriggerInside;
        public static bool GroundedOverride;

        public CoyoteBounceTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            directions = data.Enum("directions", BounceDirections.Top);
            types = data.Enum("dashTypes", DashTypes.All);
            time = data.Float("time", 0.1f);
            if (data.Has("refill") || data.Attr("refillDirections", "Top") == "MatchCoyote") {
                refill = directions;
            } else {
                refill = data.Enum("refillDirections", BounceDirections.Top);
            }
            setGrounded = data.Bool("setGrounded", false);
            dashes = data.Bool("refillDashes", true);
            stamina = data.Bool("refillStamina", true);
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            CoyoteTriggerInside = this;
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            if (CoyoteTriggerInside == this) {
                CoyoteTriggerInside = null;
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
			if (CoyoteTriggerInside == this) {
				CoyoteTriggerInside = null;
			}
		}

        public enum BounceDirections {
            None,
            Top,
            TopAndSides,
            AllDirections
        }

        public enum DashTypes {
            All,
            Rebound,
            Bounce,
            Ignore,
            Normal,
        }

        public static bool MatchDirection(Vector2 direction, BounceDirections bounceDirections) {
            if (bounceDirections == BounceDirections.None)
                return false;

            return !((direction.Y < 0f && bounceDirections != BounceDirections.AllDirections) || (direction.X != 0f && bounceDirections == BounceDirections.Top));
        }

        public static bool MatchDashType(DashCollisionResults result, DashTypes dashTypes) {
            if (dashTypes == DashTypes.All)
                return true;

            if (result.ToString() == dashTypes.ToString())
                return true;

            if (dashTypes == DashTypes.Normal && (result == DashCollisionResults.NormalCollision || result == DashCollisionResults.NormalOverride))
                return true;

            return false;
        }
    }
}
