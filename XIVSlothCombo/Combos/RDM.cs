using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Logging;

namespace XIVSlothComboPlugin.Combos
{
    internal static class RDM
    {
        public const byte JobID = 35;

        public const uint
            Verthunder = 7505,
            Veraero = 7507,
            Veraero2 = 16525,
            Veraero3 = 25856,
            Verthunder2 = 16524,
            Verthunder3 = 25855,
            Impact = 16526,
            Redoublement = 7516,
            EnchantedRedoublement = 7529,
            Zwerchhau = 7512,
            EnchantedZwerchhau = 7528,
            Riposte = 7504,
            EnchantedRiposte = 7527,
            Scatter = 7509,
            Verstone = 7511,
            Verfire = 7510,
            Jolt = 7503,
            Jolt2 = 7524,
            Verholy = 7526,
            Verflare = 7525,
            Fleche = 7517,
            ContreSixte = 7519,
            Engagement = 16527,
            Verraise = 7523,
            Scorch = 16530,
            Resolution = 25858,
            Moulinet = 7513,
            EnchantedMoulinet = 7530,
            Corpsacorps = 7506,
            Displacement = 7515,

            //Buffs
            Acceleration = 7518,
            Swiftcast = 7561,
            Manafication = 7521,
            Embolden = 7520,
            LucidDreaming = 7562;

        public static class Buffs
        {
            public const ushort
                Swiftcast = 167,
                VerfireReady = 1234,
                VerstoneReady = 1235,
                Dualcast = 1249,
                Chainspell = 2560,
                Acceleration = 1238,
                Embolden = 1239, 
                Manafication = 1971;
        }

        public static class Debuffs
        {
            // public const short placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Jolt = 2,
                Verthunder = 4,
                Veraero = 10,
                Verthunder2 = 18,
                Veraero2 = 22,
                LucidDreaming = 24,
                Verraise = 64,
                Zwerchhau = 35,
                Swiftcast = 18,
                Acceleration = 50,
                Redoublement = 50,
                Moulinet = 52,
                Vercure = 54,
                Embolden = 58,
                Manafication = 60,
                Jolt2 = 62,
                Impact = 66,
                Verflare = 68,
                Verholy = 70,
                Fleche = 45,
                ContreSixte = 56,
                Engagement = 40,
                Scorch = 80,
                Veraero3 = 82,
                Verthunder3 = 82,
                Resolution = 90;
        }

        public static class Config
        {
            public const string
                RdmLucidMpThreshold = "RdmLucidMpThreshold";
        }
    }

    internal class RedMageAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var canWeave = CanWeave(actionID);
           
            if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast))
            {
                if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                    return RDM.Fleche;
                if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                    return RDM.ContreSixte;
            }

            if (actionID == RDM.Veraero2)
            {
                if (HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell))
                    return OriginalHook(RDM.Impact);

                return RDM.Veraero2;
            }

            if (actionID == RDM.Verthunder2)
            {
                if (HasEffect(RDM.Buffs.Swiftcast) || HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell))
                    return OriginalHook(RDM.Impact);

                return RDM.Verthunder2;
            }

            return actionID;
        }
    }

    internal class RedMageMeleeCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMeleeCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RDM.Redoublement || actionID == RDM.Riposte || actionID == RDM.Zwerchhau)
            {
                var gauge = GetJobGauge<RDMGauge>();
                var corpsCharges = GetRemainingCharges(RDM.Corpsacorps);
                var engagementCharges = GetRemainingCharges(RDM.Engagement);
                var emboldenCD = GetCooldownRemainingTime(RDM.Embolden);
                var canWeave = CanWeave(OriginalHook(actionID));

                if (canWeave && !HasEffect(RDM.Buffs.Dualcast))
                {
                    if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) )
                    {
                        if (level >= RDM.Levels.Embolden && IsOffCooldown(RDM.Embolden))
                        {
                            if (lastComboMove == RDM.EnchantedRedoublement || HasEffect(RDM.Buffs.Manafication))
                                return RDM.Embolden;
                        }


                        if (level >= RDM.Levels.Manafication && IsOffCooldown(RDM.Manafication))
                        {
                            if (lastComboMove == RDM.EnchantedRedoublement
                                || (gauge.ManaStacks >= 1 && (gauge.BlackMana < 15 || gauge.WhiteMana < 15)))
                            {
                                return RDM.Manafication;
                            }
                        }
                        
                        if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                            return RDM.Fleche;
                        if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                            return RDM.ContreSixte;
                    }

                    if (IsEnabled(CustomComboPreset.RedMageEngagementFeature))
                    {
                        if (corpsCharges >= 1 && corpsCharges >= engagementCharges && InCorpsRange(true)
                             && level >= RDM.Levels.Engagement && (emboldenCD > GetCooldownRemainingTime(RDM.Corpsacorps) - 10))
                        {
                            return RDM.Corpsacorps;
                        }

                        if (engagementCharges >= 1 && corpsCharges <= engagementCharges && InMeleeRange(true)
                             && level >= RDM.Levels.Engagement && (emboldenCD > GetCooldownRemainingTime(RDM.Engagement) - 10))
                        {
                            return RDM.Engagement;
                        }
                    }
                }
                
                if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus) && gauge.ManaStacks == 3)
                {
                    if (gauge.BlackMana >= gauge.WhiteMana && level >= RDM.Levels.Verholy)
                    {
                        if ((gauge.BlackMana - gauge.WhiteMana <= 9) && gauge.BlackMana <= 81 && HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }
                    else if (level >= RDM.Levels.Verflare)
                    {
                        if ((gauge.WhiteMana - gauge.BlackMana <= 9) && gauge.WhiteMana <= 81 && !HasEffect(RDM.Buffs.VerstoneReady) && HasEffect(RDM.Buffs.VerfireReady)
                             && level >= RDM.Levels.Verholy)
                            return RDM.Verholy;

                        return RDM.Verflare;
                    }
                }

                if ((lastComboMove == RDM.Riposte || lastComboMove == RDM.EnchantedRiposte) && level >= RDM.Levels.Zwerchhau)
                    return OriginalHook(RDM.Zwerchhau);

                if (lastComboMove == RDM.Zwerchhau && level >= RDM.Levels.Redoublement)
                    return OriginalHook(RDM.Redoublement);

                if (IsEnabled(CustomComboPreset.RedMageMeleeComboPlus))
                {
                    if ((lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy) && level >= RDM.Levels.Scorch)
                        return RDM.Scorch;
                }

                if (IsEnabled(CustomComboPreset.RedmageResolutionFinisherMelee))
                {
                    if (lastComboMove == RDM.Scorch && level >= 90)
                        return RDM.Resolution;
                }

                return OriginalHook(RDM.Riposte);
            }

            return actionID;
        }
    }

    internal class RedMageVerprocCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageVerprocCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RDM.Verstone)
            {
                var canWeave = CanWeave(actionID);
                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast))
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }

                if (IsEnabled(CustomComboPreset.RedmageResolutionFinisher))
                {
                    if (lastComboMove == RDM.Scorch && level >= 90)
                        return RDM.Resolution;
                }

                if (level >= RDM.Levels.Scorch && (lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy))
                    return RDM.Scorch;

                if (lastComboMove == RDM.EnchantedRedoublement && level >= RDM.Levels.Verholy)
                    return RDM.Verholy;

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Veraero3)
                        return RDM.Veraero3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerstoneReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Veraero3)
                        return RDM.Veraero3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Veraero && level < RDM.Levels.Veraero3)
                        return RDM.Veraero;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerstoneReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Veraero && level < RDM.Levels.Veraero3)
                        return RDM.Veraero;
                }

                if (HasEffect(RDM.Buffs.VerstoneReady))
                    return RDM.Verstone;

                return OriginalHook(RDM.Jolt2);
            }

            if (actionID == RDM.Verfire)
            {
                if (level >= RDM.Levels.Scorch && (lastComboMove == RDM.Verflare || lastComboMove == RDM.Verholy))
                    return RDM.Scorch;

                if (lastComboMove == RDM.EnchantedRedoublement && level >= RDM.Levels.Verflare)
                    return RDM.Verflare;

                // Thunder 3
                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Verthunder3)
                        return RDM.Verthunder3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder3)
                        return RDM.Verthunder3;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocComboPlus))
                {
                    if ((HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Chainspell) || HasEffect(RDM.Buffs.Swiftcast)) && level >= RDM.Levels.Verthunder && level < RDM.Levels.Verthunder3)
                        return RDM.Verthunder;
                }

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder && level < RDM.Levels.Verthunder3)
                        return RDM.Verthunder;
                }

                if (HasEffect(RDM.Buffs.VerfireReady))
                    return RDM.Verfire;
                if (IsEnabled(CustomComboPreset.RedmageResolutionFinisher))
                {
                    if (lastComboMove == RDM.Scorch && level >= 90)
                        return RDM.Resolution;
                }

                return OriginalHook(RDM.Jolt2);
            }

            return actionID;
        }
    }

    internal class RedMageOgcdCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageOgcdCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (IsEnabled(CustomComboPreset.RedMageOgcdCombo))
            {
                if (actionID == RDM.ContreSixte || actionID == RDM.Fleche)
                {
                    if (level >= RDM.Levels.ContreSixte && level <= RDM.Levels.Fleche)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    if (level >= RDM.Levels.ContreSixte)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    if (level >= RDM.Levels.Fleche)
                        return CalcBestAction(actionID, RDM.ContreSixte, RDM.Fleche);

                    return RDM.Fleche;
                }

                return actionID;
            }

            return actionID;
        }
    }
    internal class RedMageSmartcastAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSmartcastAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RDM.Veraero2 or RDM.Verthunder2)
            {
                const int
                FINISHER_DELTA = 11,
                IMBALANCE_DIFF_MAX = 30;
                var accelBuff = HasEffect(RDM.Buffs.Acceleration);
                var dualcastBuff = HasEffect(RDM.Buffs.Dualcast);
                var swiftcastBuff = HasEffect(RDM.Buffs.Swiftcast);
                var gauge = GetJobGauge<RDMGauge>();
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                int blackThreshold = white + IMBALANCE_DIFF_MAX;
                int whiteThreshold = black + IMBALANCE_DIFF_MAX;
                var canWeave = CanWeave(actionID);

                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast))
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }
                if (lastComboMove == RDM.Scorch && level >= RDM.Levels.Resolution)
                    return RDM.Resolution;
                if (lastComboMove == RDM.Verholy && level >= RDM.Levels.Scorch || lastComboMove == RDM.Verflare && level >= RDM.Levels.Scorch)
                    return RDM.Scorch;
                if (gauge.ManaStacks == 3 && level >= RDM.Levels.Verflare)
                {
                    if (black >= white && level >= RDM.Levels.Verholy)
                    {
                        if (HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }
                    if (HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && level >= RDM.Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                        return RDM.Verholy;

                    return RDM.Verflare;
                }
                if (dualcastBuff || accelBuff || swiftcastBuff || HasEffect(RDM.Buffs.Chainspell) || level <= 18)
                    return OriginalHook(RDM.Impact);

                if (level <= 18)
                    return RDM.Verthunder2;

                if (gauge.BlackMana > gauge.WhiteMana)
                    return RDM.Veraero2;

                if (gauge.WhiteMana > gauge.BlackMana)
                    return RDM.Verthunder2;


            }

            return actionID;
        }
    }
    internal class RedMageSmartSingleTargetCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSmartSingleTargetCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {

            if (actionID is RDM.Veraero or RDM.Verthunder or RDM.Verstone or RDM.Verfire)
            {
                const int
                LONG_DELTA = 6,
                PROC_DELTA = 5,
                FINISHER_DELTA = 11,
                IMBALANCE_DIFF_MAX = 30;

                bool verfireUp = HasEffect(RDM.Buffs.VerfireReady);
                bool verstoneUp = HasEffect(RDM.Buffs.VerstoneReady);
                RDMGauge gauge = GetJobGauge<RDMGauge>();
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                var canWeave = CanWeave(OriginalHook(actionID));

                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerSmartCastFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder)
                        return OriginalHook(RDM.Verthunder);
                }
                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast))
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }
                if (actionID is RDM.Veraero or RDM.Verthunder)
                {

                    if (level < RDM.Levels.Verthunder)
                        return RDM.Jolt;

                    if (level is < RDM.Levels.Veraero and >= RDM.Levels.Verthunder)
                        return OriginalHook(RDM.Verthunder);

                    // This is for the long opener only, so we're not bothered about fast casting or finishers or anything like that
                    if (black < white)
                        return OriginalHook(RDM.Verthunder);

                    if (white < black)
                        return OriginalHook(RDM.Veraero);

                    return actionID;
                }

                if (actionID is RDM.Verstone or RDM.Verfire)
                {

                    bool fastCasting = HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast);
                    bool accelerated = HasEffect(RDM.Buffs.Acceleration);
                    bool isFinishing1 = gauge.ManaStacks == 3;
                    bool isFinishing2 = comboTime > 0 && lastComboMove is RDM.Verholy or RDM.Verflare;
                    bool isFinishing3 = comboTime > 0 && lastComboMove is RDM.Scorch;
                    bool canFinishWhite = level >= RDM.Levels.Verholy;
                    bool canFinishBlack = level >= RDM.Levels.Verflare;
                    int blackThreshold = white + IMBALANCE_DIFF_MAX;
                    int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                    // If we're ready to Scorch or Resolution, just do that. Nice and simple. Sadly, that's where the simple ends.
                    if (isFinishing3 && level >= 90)
                        return RDM.Resolution;
                    if (isFinishing2 && level >= RDM.Levels.Scorch)
                        return RDM.Scorch;

                    if (isFinishing1 && canFinishBlack)
                    {

                        if (black >= white && canFinishWhite)
                        {

                            // If we can already Verstone, but we can't Verfire, and Verflare WON'T imbalance us, use Verflare
                            if (verstoneUp && !verfireUp && (black + FINISHER_DELTA <= blackThreshold))
                                return RDM.Verflare;

                            return RDM.Verholy;
                        }

                        // If we can already Verfire, but we can't Verstone, and we can use Verholy, and it WON'T imbalance us, use Verholy
                        if (verfireUp && !verstoneUp && canFinishWhite && (white + FINISHER_DELTA <= whiteThreshold))
                            return RDM.Verholy;

                        return RDM.Verflare;
                    }

                    if (fastCasting || accelerated)
                    {

                        if (level is < RDM.Levels.Veraero and >= RDM.Levels.Verthunder)
                            return RDM.Verthunder;

                        if (verfireUp == verstoneUp)
                        {

                            // Either both procs are already up or neither is - use whatever gives us the mana we need
                            if (black < white)
                                return OriginalHook(RDM.Verthunder);

                            if (white < black)
                                return OriginalHook(RDM.Veraero);

                            // If mana levels are equal, prioritise the colour that the original button was
                            return actionID is RDM.Verstone
                                ? OriginalHook(RDM.Veraero)
                                : OriginalHook(RDM.Verthunder);
                        }

                        if (verfireUp)
                        {

                            // If Veraero is feasible, use it
                            if (white + LONG_DELTA <= whiteThreshold)
                                return OriginalHook(RDM.Veraero);

                            return OriginalHook(RDM.Verthunder);
                        }

                        if (verstoneUp)
                        {

                            // If Verthunder is feasible, use it
                            if (black + LONG_DELTA <= blackThreshold)
                                return OriginalHook(RDM.Verthunder);

                            return OriginalHook(RDM.Veraero);
                        }
                    }

                    if (verfireUp && verstoneUp)
                    {

                        // Decide by mana levels
                        if (black < white)
                            return RDM.Verfire;

                        if (white < black)
                            return RDM.Verstone;

                        // If mana levels are equal, prioritise the original button
                        return actionID;
                    }

                    // Only use Verfire if it won't imbalance us
                    if (verfireUp && black + PROC_DELTA <= blackThreshold)
                        return RDM.Verfire;

                    // Only use Verstone if it won't imbalance us
                    if (verstoneUp && white + PROC_DELTA <= whiteThreshold)
                        return RDM.Verstone;

                    // If neither's up or the one that is would imbalance us, just use Jolt
                    return OriginalHook(RDM.Jolt2);
                }
            }
            return actionID;
        }
    }
    internal class RedMageMeleeAoECombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMeleeAoECombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {

            if (actionID == RDM.EnchantedMoulinet || actionID == RDM.Moulinet)
            {
                const int
                FINISHER_DELTA = 11,
                IMBALANCE_DIFF_MAX = 30;
                var gauge = GetJobGauge<RDMGauge>();
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                int blackThreshold = white + IMBALANCE_DIFF_MAX;
                int whiteThreshold = black + IMBALANCE_DIFF_MAX;
                var engagementCD = GetCooldown(RDM.Engagement);
                var canWeave = CanWeave(OriginalHook(actionID));
                
                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast))
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }
                if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && engagementCD.CooldownRemaining < 35 && InMeleeRange(true))
                {
                    return RDM.Engagement;
                }
                if (lastComboMove == RDM.Scorch && level >= RDM.Levels.Resolution)
                    return RDM.Resolution;
                if (lastComboMove == RDM.Verholy && level >= RDM.Levels.Scorch || lastComboMove == RDM.Verflare && level >= RDM.Levels.Scorch)
                    return RDM.Scorch;
                if (gauge.ManaStacks == 3 && level >= RDM.Levels.Verflare)
                {
                    if (black >= white && level >= RDM.Levels.Verholy)
                    {
                        if (HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }
                    if (HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && level >= RDM.Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                        return RDM.Verholy;

                    return RDM.Verflare;
                }

            }
            return actionID;
        }
    }
    internal class SimpleRedMage : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleRedMage;

        internal static bool inOpener = false;
        internal static bool openerFinished = false;
        internal static byte step = 0;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)

        {
            if (actionID is RDM.Verthunder3)
            {
                var inCombat = HasCondition(ConditionFlag.InCombat);

                if (lastComboMove == RDM.Verthunder3)
                {
                    if (inCombat && HasEffect(RDM.Buffs.Dualcast) && !inOpener)
                    {
                        inOpener = true;
                    }

                    return RDM.Veraero3;
                }

                if (!inCombat && (inOpener || openerFinished))
                {
                    inOpener = false;
                    openerFinished = false;
                    step = 0;

                    return RDM.Verthunder3;
                }
            }

            if (actionID is RDM.Jolt or RDM.Verstone or RDM.Verfire)
            {
                const int
                LONG_DELTA = 6,
                PROC_DELTA = 5,
                FINISHER_DELTA = 11,
                IMBALANCE_DIFF_MAX = 30;

                bool verfireUp = HasEffect(RDM.Buffs.VerfireReady);
                bool verstoneUp = HasEffect(RDM.Buffs.VerstoneReady);
                RDMGauge gauge = GetJobGauge<RDMGauge>();
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                var corpsCharges = GetRemainingCharges(RDM.Corpsacorps);
                var engagementCharges = GetRemainingCharges(RDM.Engagement);
                var emboldenCD = GetCooldownRemainingTime(RDM.Embolden);
                var canWeave = CanWeave(actionID);

                if (IsEnabled(CustomComboPreset.SimpleRedMageOpener) && level >= 90)
                {
                    if (inOpener && !openerFinished)
                    {
                        //veraero
                        //swiftcast
                        //accel
                        //verthunder
                        //verthunder
                        //embolden
                        //manafication
                        //Riposte
                        //Fleche
                        //Zwercchau
                        //Contre-sixte
                        //Redoublement
                        //Corps-a-corps
                        //Engagement
                        //Verholy
                        //Corps-a-corps
                        //Engagement
                        //Scorch
                        //Resolution

                        //we do it in steps to be able to control it
                        if (step == 0)
                        {
                            if ( lastComboMove == RDM.Veraero3 ) step++;
                            else return RDM.Veraero3;
                        }

                        if (step == 1)
                        {
                            if (IsOnCooldown(RDM.Swiftcast)) step++;
                            else return RDM.Swiftcast;
                        }

                        if (step == 2)
                        {
                            if (GetRemainingCharges(RDM.Acceleration) < 2) step++;
                            else return RDM.Acceleration;
                        }

                        if (step == 3)
                        {
                            if (lastComboMove == RDM.Verthunder3) step++;
                            else return RDM.Verthunder3;
                        }

                        if (step == 4)
                        {
                            if (lastComboMove == RDM.Veraero3) step++;
                            else return RDM.Veraero3;
                        }

                        if (step == 5)
                        {
                            if (IsOnCooldown(RDM.Embolden)) step++;
                            else return RDM.Embolden;
                        }

                        if (step == 6)
                        {
                            if (IsOnCooldown(RDM.Manafication)) step++;
                            else return RDM.Manafication;
                        }

                        if (step == 7)
                        {
                            if (lastComboMove == RDM.Riposte) step++;
                            else return RDM.EnchantedRiposte;
                        }

                        if (step == 8)
                        {
                            if (IsOnCooldown(RDM.Fleche)) step++;
                            else return RDM.Fleche;
                        }

                        if (step == 9)
                        {
                            if (lastComboMove == RDM.Zwerchhau) step++;
                            else return RDM.EnchantedZwerchhau;
                        }

                        if (step == 10)
                        {
                            if (IsOnCooldown(RDM.ContreSixte)) step++;
                            else return RDM.ContreSixte;
                        }

                        if (step == 11)
                        {
                            if (lastComboMove == RDM.Redoublement || gauge.ManaStacks == 3) step++;
                            else return RDM.EnchantedRedoublement;
                        }

                        if (step == 12)
                        {
                            if (corpsCharges < 2) step++;
                            else return RDM.Corpsacorps;
                        }

                        if (step == 13)
                        {
                            if (engagementCharges < 2) step++;
                            else return RDM.Engagement;
                        }

                        if (step == 14)
                        {
                            if (lastComboMove == RDM.Verholy) step++;
                            else return RDM.Verholy;
                        }

                        if (step == 15)
                        {
                            if (corpsCharges < 1) step++;
                            else return RDM.Corpsacorps;
                        }

                        if (step == 16)
                        {
                            if (engagementCharges < 1) step++;
                            else return RDM.Engagement;
                        }

                        if (step == 17)
                        {
                            if (lastComboMove == RDM.Scorch) step++;
                            else return RDM.Scorch;
                        }

                        if (step == 18)
                        {
                            if (lastComboMove == RDM.Resolution) step++;
                            else return RDM.Resolution;
                        }

                        openerFinished = true;
                    }
                }

                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave)
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }

                if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && corpsCharges >= 1 && corpsCharges >= engagementCharges &&
                    InCorpsRange(true) && level >= RDM.Levels.Engagement && (emboldenCD > GetCooldownRemainingTime(RDM.Corpsacorps) - 10))
                {
                    return RDM.Corpsacorps;
                }

                if (IsEnabled(CustomComboPreset.RedMageEngagementFeature) && canWeave && engagementCharges >= 1 && corpsCharges <= engagementCharges &&
                    InMeleeRange(true) && level >= RDM.Levels.Engagement && (emboldenCD > GetCooldownRemainingTime(RDM.Engagement) - 10))
                {
                    return RDM.Engagement;
                }

                if (gauge.ManaStacks < 3 && InMeleeRange(true) && (lastComboMove == RDM.Riposte || lastComboMove == RDM.EnchantedRiposte) && gauge.WhiteMana >= 30 && gauge.BlackMana >= 30 )
                {
                    if (level >= RDM.Levels.Zwerchhau )
                    {
                        return OriginalHook(RDM.Zwerchhau);
                    } else  return RDM.EnchantedRiposte;
                }
                    

                if (gauge.ManaStacks < 3 && InMeleeRange(true) && (lastComboMove == RDM.Zwerchhau || lastComboMove == RDM.EnchantedRiposte) && gauge.WhiteMana >= 15 && gauge.BlackMana >= 15)
                {
                    if (level >= RDM.Levels.Redoublement)
                    {
                        return OriginalHook(RDM.Redoublement);
                    }
                    else return RDM.EnchantedRiposte;
                }

                if (gauge.ManaStacks < 3
                    && !HasEffect(RDM.Buffs.Dualcast)
                    && InMeleeRange(true)
                    && gauge.WhiteMana >= 75 && gauge.BlackMana >= 75
                    && emboldenCD > 21 // Time taken for the melee combo + embolden setup time
                    && lastComboMove is not (RDM.Verholy or RDM.Verflare or RDM.Scorch or RDM.Resolution or RDM.Resolution))
                {
                    PluginLog.LogInformation("LastComboMove = " + lastComboMove);
                    return RDM.EnchantedRiposte;
                }
                
                if (IsEnabled(CustomComboPreset.RedMageVerprocOpenerSmartCastFeature))
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasCondition(ConditionFlag.InCombat) && level >= RDM.Levels.Verthunder)
                        return OriginalHook(RDM.Verthunder);
                }

                if (IsEnabled(CustomComboPreset.SimpleRedMageFishing) && canWeave) 
                {
                    if (!HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.Dualcast) && gauge.ManaStacks != 3)
                    {
                        if (!HasEffect(RDM.Buffs.Acceleration) && HasCharges(RDM.Acceleration) && level >= RDM.Levels.Acceleration)
                        {
                            return RDM.Acceleration;
                        }
                    }

                    if (level >= RDM.Levels.Acceleration && !IsEnabled(CustomComboPreset.SimpleRedMageAccelOnlyFishing)
                            && !HasEffect(RDM.Buffs.Acceleration) && GetRemainingCharges(RDM.Acceleration) == 2
                            && !(HasEffect(RDM.Buffs.Swiftcast)
                                    && lastComboMove is RDM.EnchantedRedoublement or RDM.Verholy or RDM.Verflare or RDM.Scorch or RDM.Resolution or RDM.Resolution))
                    {
                        return RDM.Acceleration;
                    }

                    if (level >= RDM.Levels.Swiftcast && !IsEnabled(CustomComboPreset.SimpleRedMageAccelOnlyFishing)
                            && !HasEffect(RDM.Buffs.Swiftcast) && IsOffCooldown(RDM.Swiftcast)
                            && !(HasEffect(RDM.Buffs.Acceleration) && lastComboMove == RDM.EnchantedRedoublement))
                    {
                        return RDM.Swiftcast;
                    }

                    
                }

                if (actionID is RDM.Veraero or RDM.Verthunder)
                {

                    if (level < RDM.Levels.Verthunder)
                        return RDM.Jolt;

                    if (level is < RDM.Levels.Veraero and >= RDM.Levels.Verthunder)
                        return OriginalHook(RDM.Verthunder);

                    // This is for the long opener only, so we're not bothered about fast casting or finishers or anything like that
                    if (black < white)
                        return OriginalHook(RDM.Verthunder);

                    if (white < black)
                        return OriginalHook(RDM.Veraero);

                    return actionID;
                }

                bool fastCasting = HasEffect(RDM.Buffs.Dualcast) || HasEffect(RDM.Buffs.Swiftcast);
                bool accelerated = HasEffect(RDM.Buffs.Acceleration);
                bool isFinishing1 = gauge.ManaStacks == 3;
                bool isFinishing2 = comboTime > 0 && lastComboMove is RDM.Verholy or RDM.Verflare;
                bool isFinishing3 = comboTime > 0 && lastComboMove is RDM.Scorch;
                bool canFinishWhite = level >= RDM.Levels.Verholy;
                bool canFinishBlack = level >= RDM.Levels.Verflare;
                int blackThreshold = white + IMBALANCE_DIFF_MAX;
                int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                // If we're ready to Scorch or Resolution, just do that. Nice and simple. Sadly, that's where the simple ends.
                if (isFinishing3 && level >= 90)
                    return RDM.Resolution;
                if (isFinishing2 && level >= RDM.Levels.Scorch)
                    return RDM.Scorch;

                if (isFinishing1 && canFinishBlack)
                {

                    if (black >= white && canFinishWhite)
                    {

                        // If we can already Verstone, but we can't Verfire, and Verflare WON'T imbalance us, use Verflare
                        if (verstoneUp && !verfireUp && (black + FINISHER_DELTA <= blackThreshold))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }

                    // If we can already Verfire, but we can't Verstone, and we can use Verholy, and it WON'T imbalance us, use Verholy
                    if (verfireUp && !verstoneUp && canFinishWhite && (white + FINISHER_DELTA <= whiteThreshold))
                        return RDM.Verholy;

                    return RDM.Verflare;
                }

                if (fastCasting || accelerated)
                {

                    if (level is < RDM.Levels.Veraero and >= RDM.Levels.Verthunder)
                        return RDM.Verthunder;

                    if (verfireUp == verstoneUp)
                    {

                        // Either both procs are already up or neither is - use whatever gives us the mana we need
                        if (black < white)
                            return OriginalHook(RDM.Verthunder);

                        if (white < black)
                            return OriginalHook(RDM.Veraero);

                        // If mana levels are equal, prioritise the colour that the original button was
                        return actionID is RDM.Verstone
                            ? OriginalHook(RDM.Veraero)
                            : OriginalHook(RDM.Verthunder);
                    }

                    if (verfireUp)
                    {

                        // If Veraero is feasible, use it
                        if (white + LONG_DELTA <= whiteThreshold)
                            return OriginalHook(RDM.Veraero);

                        return OriginalHook(RDM.Verthunder);
                    }

                    if (verstoneUp)
                    {

                        // If Verthunder is feasible, use it
                        if (black + LONG_DELTA <= blackThreshold)
                            return OriginalHook(RDM.Verthunder);

                        return OriginalHook(RDM.Veraero);
                    }
                }

                if (verfireUp && verstoneUp)
                {

                    // Decide by mana levels
                    if (black < white)
                        return RDM.Verfire;

                    if (white < black)
                        return RDM.Verstone;

                    // If mana levels are equal, prioritise the original button
                    return actionID;
                }

                // Only use Verfire if it won't imbalance us
                if (verfireUp && black + PROC_DELTA <= blackThreshold)
                    return RDM.Verfire;

                // Only use Verstone if it won't imbalance us
                if (verstoneUp && white + PROC_DELTA <= whiteThreshold)
                    return RDM.Verstone;

                // If neither's up or the one that is would imbalance us, just use Jolt
                return OriginalHook(RDM.Jolt2);
            }

            return actionID;
        }
    }
    internal class SimpleRedMageAoE : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SimpleRedMageAoE;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RDM.Veraero2 or RDM.Verthunder2 )
            {
                const int
                FINISHER_DELTA = 11,
                IMBALANCE_DIFF_MAX = 30;
                var accelBuff = HasEffect(RDM.Buffs.Acceleration);
                var dualcastBuff = HasEffect(RDM.Buffs.Dualcast);
                var swiftcastBuff = HasEffect(RDM.Buffs.Swiftcast);
                var gauge = GetJobGauge<RDMGauge>();
                int black = gauge.BlackMana;
                int white = gauge.WhiteMana;
                int blackThreshold = white + IMBALANCE_DIFF_MAX;
                int whiteThreshold = black + IMBALANCE_DIFF_MAX;

                var canWeave = CanWeave(actionID);
                var inCombat = HasCondition(ConditionFlag.InCombat);

                if (inCombat && (lastComboMove is RDM.Veraero2 or RDM.Verthunder2))
                {
                    SimpleRedMage.openerFinished = true;
                    SimpleRedMage.inOpener = true;
                }

                if (!inCombat)
                {
                    SimpleRedMage.inOpener = false;
                    SimpleRedMage.step = 0;
                }

                if (IsEnabled(CustomComboPreset.RedMageOgcdComboOnCombos) && canWeave && !HasEffect(RDM.Buffs.Dualcast) )
                {
                    if (level >= RDM.Levels.Fleche && IsOffCooldown(RDM.Fleche))
                        return RDM.Fleche;
                    if (level >= RDM.Levels.ContreSixte && IsOffCooldown(RDM.ContreSixte))
                        return RDM.ContreSixte;
                }

                if (!HasEffect(RDM.Buffs.Dualcast) && level >= RDM.Levels.Moulinet &&
                    lastComboMove is not RDM.Verholy or RDM.Verflare or RDM.Scorch )
                {
                    if (gauge.WhiteMana >= 20 && gauge.BlackMana >= 20 && lastComboMove == RDM.Moulinet)
                    {
                        return RDM.EnchantedMoulinet;
                    }
                    if (gauge.WhiteMana >= 40 && gauge.BlackMana >= 40 && lastComboMove == RDM.Moulinet)
                    {
                        return RDM.EnchantedMoulinet;
                    }
                    if (gauge.WhiteMana >= 60 && gauge.BlackMana >= 60 && InMeleeRange(true))
                    {
                        return RDM.EnchantedMoulinet;
                    }
                }

                if (lastComboMove == RDM.Scorch && level >= RDM.Levels.Resolution)
                    return RDM.Resolution;
                if (lastComboMove == RDM.Verholy && level >= RDM.Levels.Scorch || lastComboMove == RDM.Verflare && level >= RDM.Levels.Scorch)
                    return RDM.Scorch;
                if (gauge.ManaStacks == 3 && level >= RDM.Levels.Verflare)
                {
                    if (black >= white && level >= RDM.Levels.Verholy)
                    {
                        if (HasEffect(RDM.Buffs.VerstoneReady) && !HasEffect(RDM.Buffs.VerfireReady) && (black + FINISHER_DELTA <= blackThreshold))
                            return RDM.Verflare;

                        return RDM.Verholy;
                    }
                    if (HasEffect(RDM.Buffs.VerfireReady) && !HasEffect(RDM.Buffs.VerstoneReady) && level >= RDM.Levels.Verholy && (white + FINISHER_DELTA <= whiteThreshold))
                        return RDM.Verholy;

                    return RDM.Verflare;
                }
                if (dualcastBuff || accelBuff || swiftcastBuff || HasEffect(RDM.Buffs.Chainspell) )
                    return OriginalHook(RDM.Impact);

                if (level < RDM.Levels.Verthunder2)
                    return RDM.Jolt;

                if (gauge.BlackMana > gauge.WhiteMana && level >= RDM.Levels.Veraero2)
                    return RDM.Veraero2;
                
                if (gauge.WhiteMana > gauge.BlackMana && level >= RDM.Levels.Verthunder2) 
                    return RDM.Verthunder2;
                
            }

            return actionID;
        }
    }
    internal class RedMageMovementFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageMovementFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if(actionID == RDM.Corpsacorps)
            {
                if (InMeleeRange(true) && HasCharges(RDM.Displacement) && level >= 40)
                    return RDM.Displacement;
            }
            return actionID;
        }
    }

    internal class RedMageJoltVerprocCombo : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageJoltVerprocCombo;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if ((actionID == RDM.Jolt) || (actionID == RDM.Jolt2))
            {
                
                if (lastComboMove is RDM.Verflare or RDM.Verholy or RDM.Scorch) return OriginalHook(actionID);

                var gauge = GetJobGauge<RDMGauge>();
                //If both are proc'd, use the one based on Mana
                if ((HasEffect(RDM.Buffs.VerfireReady)) && (HasEffect(RDM.Buffs.VerstoneReady)))
                {
                    if (gauge.WhiteMana > gauge.BlackMana) return RDM.Verfire; else return RDM.Verstone;
                }
                //Avoiding Offbalance difference of 30, Proc adds 5, so 25
                else if ((HasEffect(RDM.Buffs.VerfireReady)) && (gauge.BlackMana - gauge.WhiteMana) < 25) return RDM.Verfire;
                else if ((HasEffect(RDM.Buffs.VerstoneReady)) && (gauge.WhiteMana - gauge.BlackMana) < 25) return RDM.Verstone;
            }
            return actionID;
        }
    }

    internal class RedMageLucidOnJolt : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageLucidOnJolt;

        internal static bool showLucid = false;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RDM.Verthunder or RDM.Veraero or RDM.Scatter or RDM.Verthunder3 or RDM.Veraero3 or RDM.Impact)
            {
                var lucidThreshold = Service.Configuration.GetCustomIntValue(RDM.Config.RdmLucidMpThreshold);

                if (level >= RDM.Levels.LucidDreaming && LocalPlayer.CurrentMp <= lucidThreshold) // Check to show Lucid Dreaming
                {
                    showLucid = true;
                }

                if (showLucid && CanSpellWeave(actionID) && HasCondition(ConditionFlag.InCombat) && IsOffCooldown(RDM.LucidDreaming) 
                    && lastComboMove != RDM.EnchantedRiposte && lastComboMove != RDM.EnchantedZwerchhau 
                    && lastComboMove != RDM.EnchantedRedoublement && lastComboMove != RDM.Verflare 
                    && lastComboMove != RDM.Verholy && lastComboMove != RDM.Scorch) // Change abilities to Lucid Dreaming for entire weave window
                {
                    return RDM.LucidDreaming;
                }
                showLucid = false;
            }
            return actionID;
        }
    }

    //RedMageSwiftVerraise
    //Swiftcast combos to Verraise when:
    //  Swiftcast is on cooldown.
    //  Swiftcast is available, but we we have Dualcast (Dualcasting verraise)
    //Using this variation other than the alternatefeature style, as verrise is level 63
    //  and swiftcast is unlocked way earlier and in theory, on a hotbar somewhere
    internal class RedMageSwiftVerraise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageSwiftVerraise;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is RDM.Swiftcast)
            {
                if (IsEnabled(CustomComboPreset.RedMageSwiftVerraise) && (level >= RDM.Levels.Verraise))
                {
                    var swiftCD = GetCooldown(RDM.Swiftcast);
                    if (
                        (swiftCD.CooldownRemaining > 0) || //Condition 1: Swiftcast is on cooldown
                        HasEffect(RDM.Buffs.Dualcast)     //Condition 2: Swiftcast is available, but we have DualCast
                       ) return RDM.Verraise;
                } //Don't meet level requirements and preset
                return OriginalHook(RDM.Swiftcast);
            }
            return actionID;
        }
    }
}


