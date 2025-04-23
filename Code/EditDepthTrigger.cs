using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace vitmod
{
    [CustomEntity("vitellary/editdepthtrigger")]
    public class EditDepthTrigger : Trigger
    {
        public EditDepthTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            newDepth = data.Int("depth", 0);
            affectedTypes = TypeHelper.ParseTypeList(data.Attr("entitiesToAffect", ""));
            debug = data.Bool("debug", false);
            update = data.Bool("updateOnEntry", false);
            if (update && data.Bool("cacheValidEntities", false)) // Should default to 'true' in editors
                validEntitiesCache = new();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            foreach (Entity entity in scene.Entities)
            {
                HandleEntity(entity, fillCache: true);

                if (debug && entity.CollideCheck(this)) {
                    Logger.Info("CrystallineHelper/EditDepthTrigger", $"{entity.GetType().FullName}: {entity.Depth}");
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (!update) {
                return;
            }

            if (validEntitiesCache is { } cache) {
                foreach (Entity entity in cache) {
                    HandleEntity(entity, fillCache: false);
                }
            } else {
                foreach (Entity entity in Scene.Entities) {
                    HandleEntity(entity, fillCache: false);
                }
            }
        }

        private void HandleEntity(Entity entity, bool fillCache) {
            if (affectedTypes.Contains(entity.GetType())) {
                if (fillCache) {
                    validEntitiesCache?.Add(entity);
                }

                if (entity.CollideCheck(this)) {
                    entity.Depth = newDepth;
                }
            }
        }

        private HashSet<Type> affectedTypes;
        private List<Entity> validEntitiesCache;

        private int newDepth;
        private bool debug;
        private bool update;
    }
}