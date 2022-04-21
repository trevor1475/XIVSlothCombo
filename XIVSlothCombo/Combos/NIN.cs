using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Logging;

namespace XIVSlothComboPlugin.Combos
{
    internal static class NIN
    {
        public const byte ClassID = 18;
        public const byte JobID = 30;

        public const uint
            SpinningEdge = 2240,
            GustSlash = 2242,
            Hide = 2245,
            Assassinate = 2246,
            ThrowingDaggers = 2247,
            Mug = 2248,
            DeathBlossom = 2254,
            AeolianEdge = 2255,
            TrickAttack = 2258,
            Kassatsu = 2264,
            ArmorCrush = 3563,
            DreamWithinADream = 3566,
            TenChiJin = 7403,
            Bhavacakra = 7402,
            HakkeMujinsatsu = 16488,
            Meisui = 16489,
            Bunshin = 16493,
            Huraijin = 25876,
            PhantomKamaitachi = 25774,
            ForkedRaiju = 25777,
            FleetingRaiju = 25778,
            Hellfrog = 7401,

            //Mudras
            Ninjutsu = 2260,
            Rabbit = 2272,

            //-- initial state mudras (the ones with charges)
            Ten = 2259,
            Chi = 2261,
            Jin = 2263,

            //-- mudras used for combos (the one used while you have the mudra buff)
            TenCombo = 18805,
            ChiCombo = 18806,
            JinCombo = 18807,

            //Ninjutsu
            FumaShuriken = 2265,
            Hyoton = 2268,
            Doton = 2270,
            Katon = 2266,
            Suiton = 2271,
            Raiton = 2267,
            Huton = 2269,
            GokaMekkyaku = 16491,
            HyoshoRanryu = 16492;

        public static class Buffs
        {
            public const ushort
                Mudra = 496,
                Kassatsu = 497,
                Suiton = 507,
                Hidden = 614,
                TenChiJin = 1186,
                AssassinateReady = 1955,
                RaijuReady = 2690,
                PhantomReady = 2723,
                Doton = 501;
        }

        public static class Debuffs
        {
            public const ushort
            TrickAttack = 1054;
        }

        public static class Levels
        {
            public const byte
                SpinningEdge = 1,
                GustSlash = 4,
                Mug = 15,
                AeolianEdge = 26,
                Ten = 30,
                Chi = 35,
                Jin = 45,
                Doton = 45,
                Assassinate = 40,
                Kassatsu = 50,
                HakkeMujinsatsu = 52,
                ArmorCrush = 54,
                Huraijin = 60,
                Bhavacakra = 68,
                Meisui = 72,
                EnhancedKassatsu = 76,
                Bunshin = 80,
                PhantomKamaitachi = 82,
                ForkedRaiju = 90;
        }

        public static class TraitLevels
        {
            public const byte
                Shukiho = 66;
        }
        public static class Config
        {
            public const string
                TrickCooldownRemaining = "TrickCooldownRemaining",
                HutonRemainingTimer = "HutonRemainingTimer",
                HutonRemainingArmorCrush = "HutonRemainingArmorCrush",
                MugNinkiGauge = "MugNinkiGauge";
        }
    }

    internal class NinjaAeolianEdgeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAeolianEdgeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.AeolianEdge)
            {
                if (OriginalHook(NIN.Ninjutsu) is NIN.Rabbit) return OriginalHook(NIN.Ninjutsu);

                if (IsEnabled(CustomComboPreset.NinjaRangedUptimeFeature) && !HasEffect(NIN.Buffs.Mudra))
                {
                    if (!InMeleeRange(true))
                        return NIN.ThrowingDaggers;
                }
                if (IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && (HasEffect(NIN.Buffs.Mudra) || HasEffect(NIN.Buffs.Kassatsu)))
                {
                    return OriginalHook(NIN.Ninjutsu);
                }
                if (IsEnabled(CustomComboPreset.NinjaFleetingRaijuFeature))
                {
                    if (HasEffect(NIN.Buffs.RaijuReady))
                        return NIN.FleetingRaiju;
                }
                if (IsEnabled(CustomComboPreset.NinjaHuraijinFeature) && level >= NIN.Levels.Huraijin)
                {
                    var gauge = GetJobGauge<NINGauge>();
                    var timer = Service.Configuration.GetCustomIntValue(NIN.Config.HutonRemainingTimer);
                    if (gauge.HutonTimer <= timer)
                        return NIN.Huraijin;
                }

                if (IsEnabled(CustomComboPreset.NinjaBunshinFeature) && level >= NIN.Levels.Bunshin)
                {
                    var canWeave = CanWeave(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var bunshinCD = GetCooldown(NIN.Bunshin);
                    if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && canWeave)
                        return NIN.Bunshin;
                    if (HasEffect(NIN.Buffs.PhantomReady) && canWeave && level >= NIN.Levels.PhantomKamaitachi)
                        return NIN.PhantomKamaitachi;
                }
                if (IsEnabled(CustomComboPreset.NinjaBhavacakraFeature) && level >= NIN.Levels.Bhavacakra)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var bunshinCD = GetCooldown(NIN.Bunshin);
                    if (gauge.Ninki >= 50 && actionIDCD.IsCooldown)
                        return NIN.Bhavacakra;
                }

                if (IsEnabled(CustomComboPreset.NinAeolianAssassinateFeature) && level >= NIN.Levels.Assassinate)
                {
                    var actionIDCD = GetCooldown(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var assasinateCD = GetCooldown(NIN.Assassinate);
                    if (actionIDCD.IsCooldown && !assasinateCD.IsCooldown)
                        return OriginalHook(NIN.Assassinate);
                }
                if (IsEnabled(CustomComboPreset.NinAeolianMugFeature) && level >= NIN.Levels.Mug)
                {
                    var canWeave = CanWeave(actionID);
                    var gauge = GetJobGauge<NINGauge>();
                    var mugCD = GetCooldown(NIN.Mug);
                    var mugNinkiValue = Service.Configuration.GetCustomIntValue(NIN.Config.MugNinkiGauge);
                    if (!mugCD.IsCooldown && gauge.Ninki <= mugNinkiValue && canWeave && level >= NIN.TraitLevels.Shukiho)
                        return OriginalHook(NIN.Mug);
                    if (!mugCD.IsCooldown && canWeave && level < NIN.TraitLevels.Shukiho)
                        return OriginalHook(NIN.Mug);
                }

                if (comboTime > 0f)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= NIN.Levels.GustSlash)
                    {
                        return NIN.GustSlash;
                    }

                    var huton = GetJobGauge<NINGauge>();
                    var armorcrushTimer = Service.Configuration.GetCustomIntValue(NIN.Config.HutonRemainingArmorCrush);

                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.ArmorCrush && huton.HutonTimer < armorcrushTimer * 1000 && IsEnabled(CustomComboPreset.NinjaArmorCrushOnMainCombo))
                    {
                        return NIN.ArmorCrush;
                    }

                    if (lastComboMove == NIN.GustSlash && level >= NIN.Levels.AeolianEdge)
                    {
                        return NIN.AeolianEdge;
                    }
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }

    internal class SimpleNinjaSingleTarget : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinSimpleSingleTarget;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is NIN.ThrowingDaggers or NIN.SpinningEdge)
            {
                var gauge = GetJobGauge<NINGauge>();
                var mudraCD = GetCooldown(NIN.Jin);
                var trickCD = GetCooldown(NIN.TrickAttack);
                var trickCDThreshold = Service.Configuration.GetCustomIntValue(NIN.Config.TrickCooldownRemaining);

                if (actionID == NIN.ThrowingDaggers)
                {
                    // 17s threshold for ranged since once back in melee you'll gcd first
                    if (ShouldSuiton(level, trickCD, trickCDThreshold: 17))
                        return SuitonCombo(level);
                    if (level >= NIN.Levels.Jin && (HasEffect(NIN.Buffs.Mudra) || mudraCD.RemainingCharges == 2))
                        return MudraCombo(level);
                    if (level >= NIN.Levels.PhantomKamaitachi && HasEffect(NIN.Buffs.PhantomReady))
                        return NIN.PhantomKamaitachi;
                    if (level >= NIN.Levels.Jin && mudraCD.RemainingCharges == 1)
                        return MudraCombo(level);

                    return actionID;
                }

                if (actionID == NIN.SpinningEdge)
                {
                    if (CanWeave(actionID) && !HasEffect(NIN.Buffs.Mudra))
                    {
                        if (HasEffect(NIN.Buffs.Suiton) && !trickCD.IsCooldown)
                            return NIN.TrickAttack;
                        if (level >= 50 && !GetCooldown(NIN.Kassatsu).IsCooldown)
                            return NIN.Kassatsu;
                        if (level >= 15 && gauge.Ninki <= 60 && !GetCooldown(NIN.Mug).IsCooldown)
                            return OriginalHook(NIN.Mug);
                        if (level >= NIN.Levels.Bunshin && gauge.Ninki >= 50 && !GetCooldown(NIN.Bunshin).IsCooldown)
                            return NIN.Bunshin;
                        if (level >= 40 && !GetCooldown(OriginalHook(NIN.Assassinate)).IsCooldown && trickCD.IsCooldown)
                            return OriginalHook(NIN.Assassinate);
                        if (level >= 68 && ((gauge.Ninki >= 95 && !HasEffect(NIN.Buffs.TenChiJin) && GetCooldown(NIN.TenChiJin).IsCooldown) || (gauge.Ninki >= 50 /*&& HasEffect(NIN.Buffs.Meisui)*/)))
                            return NIN.Bhavacakra;
                    }

                    // Should only happen in scuffed opener or after death
                    if (level >= 60 && gauge.HutonTimer == 0 && !HasEffect(NIN.Buffs.Mudra) && InMeleeRange(true))
                        return NIN.Huraijin;

                    if (OriginalHook(NIN.Ninjutsu) is NIN.Rabbit)
                        return OriginalHook(NIN.Ninjutsu);                    

                    if (ShouldSuiton(level, trickCD, trickCDThreshold))
                        return SuitonCombo(level);

                    if (level >= NIN.Levels.PhantomKamaitachi && HasEffect(NIN.Buffs.PhantomReady) && !InMeleeRange(true))
                        return NIN.PhantomKamaitachi;

                    // Burn mudra if almost or at 2 charges or out of melee range
                    if (level >= NIN.Levels.Jin &&
                            (HasEffect(NIN.Buffs.Mudra) || mudraCD.RemainingCharges == 2) || (mudraCD.RemainingCharges == 1 && (!InMeleeRange(true) || mudraCD.ChargeCooldownRemaining < 4)))
                        return MudraCombo(level);

                    if (!InMeleeRange(true))
                    {
                        return NIN.ThrowingDaggers;
                    }

                    if (HasEffect(NIN.Buffs.RaijuReady))
                        return NIN.FleetingRaiju;

                    if (level >= NIN.Levels.PhantomKamaitachi && (FindEffect(NIN.Buffs.PhantomReady)?.RemainingTime < 5 || (trickCD.CooldownRemaining < 3 && lastComboMove != NIN.GustSlash)))
                        return NIN.PhantomKamaitachi;

                    if (comboTime > 0f)
                    {
                        if (lastComboMove == NIN.SpinningEdge && level >= 4)
                            return NIN.GustSlash;
                        if (lastComboMove == NIN.GustSlash && level >= 20 && gauge.HutonTimer < 15000 && level >= 54)
                            return NIN.ArmorCrush;
                        if (lastComboMove == NIN.GustSlash && level >= 26)
                            return NIN.AeolianEdge;
                    }

                    return NIN.SpinningEdge;
                }
            }

            return actionID;
        }

        private static uint MudraCombo(byte level)
        {
            if (level >= 76)
            {
                if (!HasEffect(NIN.Buffs.Kassatsu))
                {
                    if (OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                        return OriginalHook(NIN.Ninjutsu);
                    if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                        return OriginalHook(NIN.ChiCombo);
                    if (HasEffect(NIN.Buffs.Kassatsu))
                        return NIN.JinCombo;
                    if (GetCooldown(NIN.Jin).RemainingCharges > 0)
                        return NIN.Jin;
                }
                else
                {
                    if (OriginalHook(NIN.Ninjutsu) is NIN.HyoshoRanryu or NIN.Raiton)
                        return OriginalHook(NIN.Ninjutsu);
                    if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                        return OriginalHook(NIN.JinCombo);

                    return OriginalHook(NIN.Ten);
                }
            }
            else
            {
                if (OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                    return OriginalHook(NIN.Ninjutsu);
                if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                {
                    if (level < NIN.Levels.Chi)
                        return OriginalHook(NIN.Ninjutsu);

                    return OriginalHook(NIN.ChiCombo);
                }
                if (HasEffect(NIN.Buffs.Kassatsu))
                    return NIN.JinCombo;
            }

            return NIN.Jin;
        }

        private static bool ShouldSuiton(byte level, CooldownData trickCD, int trickCDThreshold)
        {
            return (!trickCD.IsCooldown || trickCD.CooldownRemaining <= trickCDThreshold)
                    && (!HasEffect(NIN.Buffs.Kassatsu) || (HasEffect(NIN.Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleTrickKassatsuFeature)))
                    && level >= 45 && IsEnabled(CustomComboPreset.NinSimpleTrickFeature);
        }

        private static uint SuitonCombo(byte level)
        {
            if (!HasEffect(NIN.Buffs.Mudra) && !HasEffect(NIN.Buffs.Suiton)
                       && (GetCooldown(NIN.Chi).RemainingCharges > 0 || (HasEffect(NIN.Buffs.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleTrickKassatsuFeature))))
                return OriginalHook(NIN.Chi);

            if (level >= NIN.Levels.Ten && !HasEffect(NIN.Buffs.Suiton) && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                return OriginalHook(NIN.TenCombo);

            if (level >= NIN.Levels.Jin && !HasEffect(NIN.Buffs.Suiton) && (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku))
                return OriginalHook(NIN.JinCombo);

            if (OriginalHook(NIN.Ninjutsu) is NIN.Suiton && !HasEffect(NIN.Buffs.Suiton))
                return OriginalHook(NIN.Ninjutsu);

            PluginLog.Error("How did I get here NIN:SuitonCombo?");
            return OriginalHook(NIN.Ninjutsu);
        }
    }


    internal class SimpleNinjaAoE : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinSimpleAoE;

        private static uint lastUsedJutsu { get; set; }

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.DeathBlossom)
            {

                var dotonBuff = FindEffect(NIN.Buffs.Doton);
                var jutsuCooldown = GetCooldown(NIN.Ten);
                var jutsuCharges = jutsuCooldown.RemainingCharges;
                var jutsuFullCooldown = jutsuCooldown.CooldownRemaining;
                lastUsedJutsu = OriginalHook(NIN.Ninjutsu) != NIN.Ninjutsu ? OriginalHook(NIN.Ninjutsu) : lastUsedJutsu;


                var gauge = GetJobGauge<NINGauge>();
                if (gauge.HutonTimer == 0 && !HasEffect(NIN.Buffs.Mudra) && !HasEffect(NIN.Buffs.Kassatsu) && level >= 60)
                    return NIN.Huraijin;

                //Doton is really annoying. It takes a hot moment for the buff to apply. This is just logic to try and deal with it so it doesn't clip with the rest of the feature.
                if (OriginalHook(NIN.Ninjutsu) == NIN.Doton)
                {
                    return OriginalHook(NIN.Ninjutsu);
                }

                if ((!HasEffect(NIN.Buffs.Doton) || (dotonBuff != null && dotonBuff?.RemainingTime <= 5)) && (jutsuCharges > 0 || HasEffect(NIN.Buffs.Mudra)) && level >= NIN.Levels.Doton && lastUsedJutsu != NIN.Doton && IsEnabled(CustomComboPreset.NinSimpleAoeMudras))
                {
                    if (OriginalHook(NIN.Ninjutsu) == NIN.Doton)
                    {
                        return NIN.Doton;
                    }
                    if (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku)
                    {
                        return NIN.ChiCombo;
                    }
                    if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                    {
                        return NIN.TenCombo;
                    }

                    if (HasEffect(NIN.Buffs.Kassatsu)) return NIN.JinCombo;

                    if (!HasEffect(NIN.Buffs.Mudra))
                        return NIN.Jin;

                }



                if ((jutsuCharges > 1 || HasEffect(NIN.Buffs.Mudra) || HasEffect(NIN.Buffs.Kassatsu) || (!GetCooldown(NIN.Kassatsu).IsCooldown) && level >= NIN.Levels.Kassatsu) && IsEnabled(CustomComboPreset.NinSimpleAoeMudras))
                {
                    if (!GetCooldown(NIN.Kassatsu).IsCooldown && !HasEffect(NIN.Buffs.Mudra) && level >= NIN.Levels.Kassatsu)
                    {
                        return NIN.Kassatsu;
                    }

                    if (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku) return OriginalHook(NIN.Ninjutsu);
                    if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken) return NIN.TenCombo;

                    if (HasEffect(NIN.Buffs.Kassatsu)) return NIN.JinCombo;

                    return NIN.Jin;

                }

                if (!HasEffect(NIN.Buffs.Mudra))
                {
                    var actionIDCD = GetCooldown(actionID);
                    var bunshinCD = GetCooldown(NIN.Bunshin);

                    if (gauge.Ninki >= 50 && !bunshinCD.IsCooldown && actionIDCD.IsCooldown && level >= NIN.Levels.Bunshin && IsEnabled(CustomComboPreset.NinSimpleAoeBunshin))
                        return NIN.Bunshin;
                    if (HasEffect(NIN.Buffs.PhantomReady) && level >= NIN.Levels.PhantomKamaitachi && IsEnabled(CustomComboPreset.NinSimpleAoeBunshin))
                        return NIN.PhantomKamaitachi;
                    if (gauge.Ninki >= 50 && actionIDCD.IsCooldown && IsEnabled(CustomComboPreset.NinSimpleHellfrogFeature))
                        return NIN.Hellfrog;

                    if (comboTime > 0f && lastComboMove == NIN.DeathBlossom && level >= 52)
                    {
                        return NIN.HakkeMujinsatsu;
                    }


                    return NIN.DeathBlossom;
                }


            }

            return actionID;
        }
    }

    internal class NinjaArmorCrushCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaArmorCrushCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.ArmorCrush)
            {
                if (CustomCombo.IsEnabled(CustomComboPreset.NinjaGCDNinjutsuFeature) && CustomCombo.OriginalHook(NIN.Jin) == CustomCombo.OriginalHook(NIN.JinCombo))
                {
                    return CustomCombo.OriginalHook(NIN.Ninjutsu);
                }

                if (comboTime > 0f)
                {
                    if (lastComboMove == NIN.SpinningEdge && level >= 4)
                    {
                        return NIN.GustSlash;
                    }

                    if (lastComboMove == NIN.GustSlash && level >= 54)
                    {
                        return NIN.ArmorCrush;
                    }
                }

                return NIN.SpinningEdge;
            }

            return actionID;
        }
    }


    internal class NinjaHideMugFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Hide)
            {
                if (CustomCombo.HasEffect(NIN.Buffs.Suiton) || CustomCombo.HasEffect(NIN.Buffs.Hidden))
                {
                    return NIN.TrickAttack;
                }

                if (CustomCombo.HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                {
                    return NIN.Mug;
                }
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuChiJinFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuChiJinFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Chi && level >= 76 && CustomCombo.HasEffect(NIN.Buffs.Kassatsu))
            {
                return NIN.Jin;
            }

            return actionID;
        }
    }

    internal class NinjaKassatsuTrickFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaKassatsuTrickFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Kassatsu)
            {
                if (CustomCombo.HasEffect(NIN.Buffs.Suiton) || CustomCombo.HasEffect(NIN.Buffs.Hidden))
                {
                    return NIN.TrickAttack;
                }

                return NIN.Kassatsu;
            }

            return actionID;
        }
    }

    internal class NinjaTCJMeisuiFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset => CustomComboPreset.NinjaTCJMeisuiFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.TenChiJin)
            {

                if (HasEffect(NIN.Buffs.Suiton))
                    return NIN.Meisui;

                if (HasEffect(NIN.Buffs.TenChiJin) && IsEnabled(CustomComboPreset.NinTCJFeature))
                {
                    var tcjTimer = FindEffectAny(NIN.Buffs.TenChiJin).RemainingTime;

                    if (tcjTimer > 5)
                        return OriginalHook(NIN.Ten);
                    if (tcjTimer > 4)
                        return OriginalHook(NIN.Chi);
                    if (tcjTimer > 3)
                        return OriginalHook(NIN.Jin);
                }
            }

            return actionID;
        }
    }

    internal class NinjaHuraijinRaijuFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHuraijinRaijuFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == NIN.Huraijin)
            {
                if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature1) && level >= NIN.Levels.ForkedRaiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.FleetingRaiju;

                if (IsEnabled(CustomComboPreset.NinjaHuraijinRaijuFeature2) && level >= NIN.Levels.ForkedRaiju && HasEffect(NIN.Buffs.RaijuReady))
                    return NIN.ForkedRaiju;
            }

            return actionID;
        }
    }

    internal class NinjaSimpleMudras : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaSimpleMudras;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is NIN.Ten or NIN.Chi or NIN.Jin)
            {
                var mudrapath = Service.Configuration.MudraPathSelection;

                if (HasEffect(NIN.Buffs.Mudra))
                {
                    if (mudrapath == 0)
                    {
                        if (level >= NIN.Levels.Ten && actionID == NIN.Ten)
                        {
                            if (level >= NIN.Levels.Chi && (OriginalHook(NIN.Ninjutsu) is NIN.Hyoton or NIN.HyoshoRanryu))
                            {
                                return OriginalHook(NIN.ChiCombo);
                            }
                            if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                if (level >= NIN.Levels.Jin) return OriginalHook(NIN.JinCombo);
                                else if (level >= NIN.Levels.Chi) return OriginalHook(NIN.ChiCombo);
                            }
                        }

                        if (level >= NIN.Levels.Chi && actionID == NIN.Chi)
                        {
                            if (level >= NIN.Levels.Jin && (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku))
                            {
                                return OriginalHook(NIN.JinCombo);
                            }
                            if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                return OriginalHook(NIN.TenCombo);
                            }
                        }

                        if (level >= NIN.Levels.Jin && actionID == NIN.Jin)
                        {
                            if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.Raiton)
                            {
                                return OriginalHook(NIN.TenCombo);
                            }
                            if (level >= NIN.Levels.Chi && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                return OriginalHook(NIN.ChiCombo);
                            }
                        }

                        return OriginalHook(NIN.Ninjutsu);
                    }


                    if (mudrapath == 1)
                    {
                        if (level >= NIN.Levels.Ten && actionID == NIN.Ten)
                        {
                            if (level >= NIN.Levels.Jin && (OriginalHook(NIN.Ninjutsu) is NIN.Raiton))
                            {
                                return OriginalHook(NIN.JinCombo);
                            }
                            if (level >= NIN.Levels.Chi && (OriginalHook(NIN.Ninjutsu) is NIN.HyoshoRanryu))
                            {
                                return OriginalHook(NIN.ChiCombo);
                            }
                            if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                if (HasEffect(NIN.Buffs.Kassatsu) && level >= NIN.Levels.EnhancedKassatsu) return NIN.JinCombo;
                                if (level >= NIN.Levels.Chi) return OriginalHook(NIN.ChiCombo);
                                else if (level >= NIN.Levels.Jin) return OriginalHook(NIN.JinCombo);
                            }
                        }

                        if (level >= NIN.Levels.Chi && actionID == NIN.Chi)
                        {
                            if (level >= NIN.Levels.Ten && (OriginalHook(NIN.Ninjutsu) is NIN.Hyoton))
                            {
                                return OriginalHook(NIN.TenCombo);
                            }
                            if (level >= NIN.Levels.Jin && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                return OriginalHook(NIN.JinCombo);
                            }
                        }

                        if (level >= NIN.Levels.Jin && actionID == NIN.Jin)
                        {
                            if (level >= NIN.Levels.Chi && OriginalHook(NIN.Ninjutsu) is NIN.GokaMekkyaku or NIN.Katon)
                            {
                                return OriginalHook(NIN.ChiCombo);
                            }
                            if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                return OriginalHook(NIN.TenCombo);
                            }
                        }

                        return OriginalHook(NIN.Ninjutsu);
                    }

                    if (mudrapath == 2)
                    {
                        if (level >= NIN.Levels.Ten && actionID == NIN.Ten)
                        {
                            if (level >= NIN.Levels.Chi && (OriginalHook(NIN.Ninjutsu) is NIN.Hyoton or NIN.HyoshoRanryu))
                            {
                                return OriginalHook(NIN.Chi);
                            }

                            if (OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                if (level >= NIN.Levels.Jin) return OriginalHook(NIN.JinCombo);
                                else if (level >= NIN.Levels.Chi) return OriginalHook(NIN.ChiCombo);
                            }
                        }

                        if (level >= NIN.Levels.Chi && actionID == NIN.Chi)
                        {
                            if (level >= NIN.Levels.Jin && (OriginalHook(NIN.Ninjutsu) is NIN.Katon or NIN.GokaMekkyaku))
                            {
                                return OriginalHook(NIN.Jin);
                            }
                            if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                return OriginalHook(NIN.Ten);
                            }
                        }

                        if (level >= NIN.Levels.Jin && actionID == NIN.Jin)
                        {
                            if (level >= NIN.Levels.Ten && OriginalHook(NIN.Ninjutsu) is NIN.Raiton)
                            {
                                return OriginalHook(NIN.Ten);
                            }
                            if (level >= NIN.Levels.Chi && OriginalHook(NIN.Ninjutsu) == NIN.GokaMekkyaku)
                            {
                                return OriginalHook(NIN.Chi);
                            }
                            if (level >= NIN.Levels.Chi && OriginalHook(NIN.Ninjutsu) == NIN.FumaShuriken)
                            {
                                if (HasEffect(NIN.Buffs.Kassatsu) && level >= NIN.Levels.EnhancedKassatsu) return OriginalHook(NIN.Ten);
                                return OriginalHook(NIN.Chi);
                            }
                        }

                        return OriginalHook(NIN.Ninjutsu);
                    }


                }
            }

            return actionID;
        }
    }
}
