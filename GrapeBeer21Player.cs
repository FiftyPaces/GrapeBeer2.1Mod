using Terraria;
using Terraria.ModLoader;

namespace GrapeBeer21Mod
{
    public class GrapeBeer21Player : ModPlayer
    {
        public bool grapeBeer21;

        public override void ResetEffects()
        {
            grapeBeer21 = false;
        }

        // 独立实现暴击伤害降低，不依赖灾厄 critDamage
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (grapeBeer21)
            {
                // 降低75%暴击伤害：匹配灾厄2.1.2效果 (critDamage -= 0.75)
                modifiers.CritDamage += -0.75f;
            }
        }
    }
}
