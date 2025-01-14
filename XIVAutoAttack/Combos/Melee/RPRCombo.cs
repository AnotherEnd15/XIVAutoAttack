using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;

namespace XIVAutoAttack.Combos.Melee;

internal class RPRCombo : CustomComboJob<RPRGauge>
{
    internal override uint JobID => 39;
    protected override bool ShouldSayout => !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded);
    internal struct Actions
    {
        public static readonly BaseAction
            //死亡之影
            ShadowofDeath = new BaseAction(24378)
            {
                TargetStatus = new ushort[] { ObjectStatus.DeathsDesign },
            },

            //切割
            Slice = new BaseAction(24373),

            //增盈切割
            WaxingSlice = new BaseAction(24374),

            //地狱切割
            InfernalSlice = new BaseAction(24375),

            //隐匿挥割
            BloodStalk = new BaseAction(24389),

            //勾刃
            Harpe = new BaseAction(24386) { BuffsProvide = new ushort[] { ObjectStatus.SoulReaver } },

            //绞决
            Gibbet = new BaseAction(24382) { EnermyLocation = EnemyLocation.Side },

            //缢杀
            Gallows = new BaseAction(24383) { EnermyLocation = EnemyLocation.Back },

            //灵魂切割
            SoulSlice = new BaseAction(24380),

            //死亡之涡
            WhorlofDeath = new BaseAction(24379)
            {
                TargetStatus = new ushort[] { ObjectStatus.DeathsDesign },
            },

            //旋转钐割
            SpinningScythe = new BaseAction(24376),

            //噩梦钐割
            NightmareScythe = new BaseAction(24377),

            //束缚挥割
            GrimSwathe = new BaseAction(24392),

            //暴食
            Gluttony = new BaseAction(24393),

            //断首
            Guillotine = new BaseAction(24384),

            //灵魂钐割
            SoulScythe = new BaseAction(24381),

            //夜游魂衣 变身！
            Enshroud = new BaseAction(24394),

            //团契
            Communio = new BaseAction(24398),

            //神秘纹 加盾
            ArcaneCrest = new BaseAction(24404, true),

            //神秘环 加Buff
            ArcaneCircle = new BaseAction(24405, true),

            //播魂种
            Soulsow = new BaseAction(24387)
            {
                BuffsProvide = new ushort[] {ObjectStatus.Soulsow},
            },

            //收获月
            HarvestMoon = new BaseAction(24388)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Soulsow },
            },

            //地狱入境
            HellsIngress = new BaseAction(24401),

            //大丰收
            PlentifulHarvest = new BaseAction(24385);
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.单体防御, $"{Actions.ArcaneCrest.Action.Name}"},
        {DescType.移动, $"{Actions.HellsIngress.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(!TargetHelper.InBattle && Actions.Soulsow.ShouldUseAction(out act)) return true;

        //上Debuff
        if (Actions.WhorlofDeath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.ShadowofDeath.ShouldUseAction(out act, lastComboActionID)) return true;

        //处于变身状态。
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.LemureShroud == 1)
            {
                if (Actions.Communio.ShouldUseAction(out act, mustUse: true)) return true;
            }

            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }
        //处于补蓝状态，赶紧补蓝条。
        else if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EnhancedGibbet))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }

        if (JobGauge.Shroud <= 50 && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice)
            && BaseAction.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice) &&
             Actions.PlentifulHarvest.ShouldUseAction(out act, mustUse: true)) return true;


        //获得灵魂 50.
        if (JobGauge.Soul <= 50)
        {
            if (Actions.SoulScythe.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.SoulSlice.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }


        //群体二连
        if (Actions.NightmareScythe.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SpinningScythe.ShouldUseAction(out act, lastComboActionID)) return true;


        //单体三连
        if (Actions.InfernalSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.WaxingSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Slice.ShouldUseAction(out act, lastComboActionID)) return true;

        //够不着了
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;

        if (Actions.HarvestMoon.ShouldUseAction(out act, mustUse:true)) return true;
        if (Actions.Harpe.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //变身用能力
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.VoidShroud > 1)
            {
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }

        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            //蓝条够了，变身！
            if (JobGauge.Shroud >= 50 && Actions.Enshroud.ShouldUseAction(out act)) return true;

            //灵魂够了，拿蓝条状态。
            if (JobGauge.Soul >= 50)
            {
                if (Actions.Gluttony.ShouldUseAction(out act, mustUse: true)) return true;
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //究极团辅
        if (Actions.ArcaneCircle.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //地狱入境
        if (Actions.HellsIngress.ShouldUseAction(out act) && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (Actions.ArcaneCrest.ShouldUseAction(out act)) return true;
        return false;
    }


}
