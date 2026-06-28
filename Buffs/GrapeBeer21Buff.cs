using CalamityMod;
using GrapeBeer21Mod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod.Buffs
{
    public class GrapeBeer21Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GrapeBeer21Player>().grapeBeer21 = true;
            // 灾厄2.2已移除CritLoss机制，改为距离伤害系统
            // critDamage默认为0，-0.75会导致暴击伤害减半，故移除
        }
    }
}
