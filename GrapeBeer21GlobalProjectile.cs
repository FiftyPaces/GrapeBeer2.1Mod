using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod
{
    public class GrapeBeer21GlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool grapeBeer21;
        public float homingRange;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            void Apply()
            {
                grapeBeer21 = true;
                homingRange = 600;
                if (projectile.timeLeft > 300 * projectile.MaxUpdates)
                    projectile.timeLeft = 300 * projectile.MaxUpdates;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = -1;
            }

            if (source is EntitySource_ItemUse_WithAmmo { Item: Item item })
            {
                if (source is EntitySource_Parent { Entity: Player player })
                {
                    if (player.GetModPlayer<GrapeBeer21Player>().grapeBeer21
                        && (item.useAmmo == AmmoID.Bullet
                         || item.useAmmo == AmmoID.Arrow
                         || item.useAmmo == AmmoID.Dart
                         || item.useAmmo == AmmoID.Rocket))
                    {
                        if (player.heldProj != projectile.whoAmI
                            && projectile.aiStyle != ProjAIStyleID.HeldProjectile
                            && projectile.damage > 0
                            && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                            Apply();
                        else
                            grapeBeer21 = true;
                    }
                }
            }
            else if (source is EntitySource_Parent { Entity: Projectile parent })
            {
                if (parent.TryGetGlobalProjectile(out GrapeBeer21GlobalProjectile pg) && pg.grapeBeer21)
                {
                    if (Main.player[projectile.owner].heldProj != projectile.whoAmI
                        && projectile.aiStyle != ProjAIStyleID.HeldProjectile
                        && projectile.damage > 0
                        && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                        Apply();
                    else
                        grapeBeer21 = true;
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (homingRange > 0f
                && Main.player[projectile.owner].heldProj != projectile.whoAmI
                && projectile.aiStyle != ProjAIStyleID.HeldProjectile)
            {
                HomeInOnNPC(projectile, !projectile.tileCollide, homingRange, 12f, 20f);
            }
        }

        /// <summary>
        /// 独立追踪实现，匹配灾厄2.1.2的 HomeInOnNPC 行为（不含extraUpdates操作）
        /// </summary>
        private static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia)
        {
            if (!projectile.friendly)
                return;

            Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

            float npcDistCompare = 25000f;
            int targetIndex = -1;
            foreach (NPC n in Main.ActiveNPCs)
            {
                float extraDistance = (n.width / 2) + (n.height / 2);
                if (!n.CanBeChasedBy(projectile, false) || !projectile.WithinRange(n.Center, maxDistance + extraDistance))
                    continue;

                float currentNPCDist = Vector2.Distance(n.Center, projectile.Center);
                if (currentNPCDist < npcDistCompare && (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1)))
                {
                    npcDistCompare = currentNPCDist;
                    targetIndex = n.whoAmI;
                }
            }

            if (targetIndex != -1)
            {
                destination = Main.npc[targetIndex].Center;
                locatedTarget = true;
            }

            if (locatedTarget)
            {
                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * inertia + homeDirection * homingVelocity) / (inertia + 1f);
            }
        }
    }
}
