//using LX_Orbwalker;
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace VayneHunter2._0
{
    internal class Program
    {
        public static String ChampName = "Vayne";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static Menu Menu;
        public static string[] Interrupt;
        public static string[] Notarget;
        public static string[] Gapcloser;
        public static Obj_AI_Hero Tar;
        public static Dictionary<string, SpellSlot> SpellData;
        public static Dictionary<Obj_AI_Hero, Vector3> DirDic, LastVecDic = new Dictionary<Obj_AI_Hero, Vector3>();
        public static Dictionary<Obj_AI_Hero, float> AngleDic = new Dictionary<Obj_AI_Hero, float>();
        public static Vector3 CurrentVec;
        public static Vector3 LastVec;
        //public static LXOrbwalker orb;
        public static bool Sol = false;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;
            Menu = new Menu("Vayne-浜岀嫍姹夊寲", "VHMenu", true);
            var orb_Menu = new Menu("璧扮爫", "Orbwalker1");
            //LXOrbwalker.AddToMenu(orb_Menu);
            Menu.AddSubMenu(orb_Menu);
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker1"));
            var ts = new Menu("鐩爣閫夋嫨", "TargetSelector");
            SimpleTs.AddToMenu(ts);
            Menu.AddSubMenu(ts);
            Menu.AddSubMenu(new Menu("[鐚庝汉]杩炴嫑", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQ", "浣跨敤 Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseE", "浣跨敤 E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseR", "浣跨敤 R").SetValue(true));
            Menu.AddSubMenu(new Menu("[鐚庝汉]娣峰悎妯″紡", "Harrass"));
            Menu.SubMenu("Harrass").AddItem(new MenuItem("UseQH", "浣跨敤 Q").SetValue(true));
            Menu.AddSubMenu(new Menu("[鐚庝汉]鏉傞」", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("AntiGP", "闃茬獊杩泑").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "涓柇娉曟湳").SetValue(true));
            Menu.SubMenu("Misc")
                .AddItem(
                    new MenuItem("ENextAuto", "鍦ˋA鍚庝娇鐢‥").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Menu.SubMenu("Misc").AddItem(new MenuItem("AdvE", "浣跨敤E鍑诲").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("SmartQ", "浣跨敤Q椋庣瓭").SetValue(false));
            Menu.SubMenu("Misc").AddItem(new MenuItem("UsePK", "浣跨敤灏佸寘").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("AutoE", "鑷姩E(钀藉悗锛墊").SetValue(false));
            Menu.SubMenu("Misc")
                .AddItem(new MenuItem("PushDistance", "浣跨敤E璺濈").SetValue(new Slider(425, 400, 475)));
            Menu.AddSubMenu(new Menu("[鐚庝汉]鐗╁搧浣跨敤", "Items"));
            Menu.SubMenu("Items").AddItem(new MenuItem("Botrk", "浣跨敤 鐮磋触").SetValue(true));
            Menu.SubMenu("Items").AddItem(new MenuItem("Youmuu", "浣跨敤 骞芥ⅵ").SetValue(true));
            Menu.SubMenu("Items")
                .AddItem(new MenuItem("OwnHPercBotrk", "浣跨敤鐮磋触锛堣嚜宸辫閲忥級").SetValue(new Slider(50, 1, 100)));
            Menu.SubMenu("Items")
                .AddItem(new MenuItem("EnHPercBotrk", "浣跨敤鐮磋触锛堟晫鏂硅閲忥級").SetValue(new Slider(20, 1, 100)));
            Menu.SubMenu("Items").AddItem(new MenuItem("ItInMix", "娣峰悎妯″紡浣跨敤鐗╁搧").SetValue(false));
            Menu.AddSubMenu(new Menu("[鐚庝汉]娉曞姏璁＄畻", "ManaMan"));
            Menu.SubMenu("ManaMan")
                .AddItem(new MenuItem("QManaC", "杩炴嫑浣跨敤Q(娉曞姏鍊硷級").SetValue(new Slider(30, 1, 100)));
            Menu.SubMenu("ManaMan")
                .AddItem(new MenuItem("QManaM", "娣峰悎浣跨敤E(娉曞姏鍊硷級").SetValue(new Slider(30, 1, 100)));
            Menu.SubMenu("ManaMan")
                .AddItem(new MenuItem("EManaC", "杩炴嫑浣跨敤E(娉曞姏鍊硷級").SetValue(new Slider(20, 1, 100)));
            Menu.SubMenu("ManaMan")
                .AddItem(new MenuItem("EManaM", "娣峰悎浣跨敤E(娉曞姏鍊硷級").SetValue(new Slider(20, 1, 100)));
            //Thank you blm95 ;)
            Menu.AddSubMenu(new Menu("[鐚庝汉]璋磋矗: ", "CondemnHero"));
            Menu.AddSubMenu(new Menu("[鐚庝汉]韬叉妧鑳絴", "gap"));
            Menu.AddSubMenu(new Menu("[鐚庝汉]韬叉妧鑳絴2", "gap2"));
            Menu.AddSubMenu(new Menu("[鐚庝汉]鎵撴柇鎶€鑳絴", "int"));
            GpIntmenuCreate();
            NoCondemnMenuCreate();
            // initHeroes();
            Menu.AddToMainMenu();
            Q = new Spell(SpellSlot.Q, 0f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 0f);
            E.SetTargetted(0.25f, 2200f);
            Game.OnGameUpdate += OnTick;
            Orbwalking.AfterAttack += OW_AfterAttack;
            //LXOrbwalker.AfterAttack += LXOrbwalker_AfterAttack;
            // LXOrbwalker.AfterAttack += OW_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            //Drawing.OnDraw += Drawing_OnDraw;
            Game.PrintChat("VayneHunter 2.03 By DZ191 鍔犺浇鎴愬姛!姹夊寲by浜岀嫍!QQ缇361630847");

        }
        static void Drawing_OnDraw(EventArgs args)
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
            {
                if (hero.IsValid && !hero.IsDead && hero.IsVisible && Player.Distance(hero) < 715f &&
                    Player.Distance(hero) > 0f && Menu.Item(hero.BaseSkinName).GetValue<bool>())
                {
                    PredictionOutput pred = E.GetPrediction(hero);

                    int pushDist = Menu.Item("PushDistance").GetValue<Slider>().Value;
                    for (int i = 0; i < pushDist; i += (int)hero.BoundingRadius)
                    {
                        Vector2 location = V2E(Player.Position, pred.CastPosition, i);
                        Vector3 loc2 = new Vector3(location.X,Player.Position.Y,location.Y);
                        Vector3 loc3 = pred.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -i)
                                    .To3D();
                        if (IsWall(loc3))
                        {
                            Utility.DrawCircle(loc3, 100f, System.Drawing.Color.Red, 4, 30, false);
                        }
                        else
                        {
                            Utility.DrawCircle(loc3, 100f, System.Drawing.Color.Aqua, 4, 30, false);
                        }
                        
                    }
                }
            }
        }

        public static void OW_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (!unit.IsMe) return;
            var targ = (Obj_AI_Hero) target;
            if (IsEnK("ENextAuto"))
            {
                CastE(targ);
                Menu.Item("ENextAuto")
                    .SetValue(new KeyBind(Menu.Item("ENextAuto").GetValue<KeyBind>().Key, KeyBindType.Toggle));
            }
            if (IsEn("UseQ") && IsMode("Combo"))
            {
                if (IsEn("UseR"))
                {
                    R.Cast();
                }
                CastQ(targ);
            }
            if (IsEn("UseQH") && IsMode("Mixed"))
            {
                CastQ(targ);
            }
            if (IsMode("Combo"))
            {
                UseItems(targ);
            }
            if (IsMode("Mixed") && IsEn("ItInMix"))
            {
                UseItems(targ);
            }
        }

        public static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            String spellName = args.SData.Name;
            //Interrupts
            if (IsEn(spellName) && sender.IsValidTarget(550f) && IsEn("Interrupt"))
            {
                CastE((Obj_AI_Hero) sender, true);
            }
            //Targeted GapClosers
            if (IsEn(spellName) && sender.IsValidTarget(550f) && IsEn("AntiGP") &&
                Gapcloser.Any(str => str.Contains(args.SData.Name))
                && args.Target.IsMe)
            {
                CastE((Obj_AI_Hero) sender, true);
            }
            //NonTargeted GP
            if (IsEn(spellName) && sender.IsValidTarget(550f) && IsEn("AntiGP") &&
                Notarget.Any(str => str.Contains(args.SData.Name))
                && Player.Distance(args.End) <= 320f)
            {
                CastE((Obj_AI_Hero) sender, true);
            }
        }

        public static void OnTick(EventArgs args)
        {
            if (IsEn("AutoE"))
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
                {
                    if (hero.IsValid && !hero.IsDead && hero.IsVisible && Player.Distance(hero) < 715f &&
                        Player.Distance(hero) > 0f && Menu.Item(hero.BaseSkinName).GetValue<bool>())
                    {
                        PredictionOutput pred = E.GetPrediction(hero);
                        int pushDist = Menu.Item("PushDistance").GetValue<Slider>().Value;
                        for (int i = 0; i < pushDist; i += (int) hero.BoundingRadius)
                        {
                            Vector2 location = V2E(Player.Position, pred.UnitPosition, i);
                            Vector3 loc3 =
                               pred.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -i)
                                    .To3D();
                            if (IsWall(loc3))
                            {
                                E.Cast(hero);
                                break;
                            }
                        }
                    }
                }
            }

            if (!IsMode("Combo") || !IsEn("UseE") || !E.IsReady() || !Orbwalking.CanMove(100))return;
            if (!IsEn("AdvE"))
            {
                foreach (
                    Obj_AI_Hero hero in
                        from hero in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(hero => hero.IsValidTarget(550f) && Menu.Item(hero.BaseSkinName).GetValue<bool>())
                        let prediction = E.GetPrediction(hero)
                        where NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                    -Menu.Item("PushDistance").GetValue<Slider>().Value)
                                .To3D())
                            .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                        -(Menu.Item("PushDistance").GetValue<Slider>().Value/2))
                                    .To3D())
                                .HasFlag(CollisionFlags.Wall)
                        select hero)
                {
                    CastE(hero);
                }
            }else
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
                {
                    if (hero.IsValid && !hero.IsDead && hero.IsVisible && Player.Distance(hero) < 715f &&
                        Player.Distance(hero) > 0f && Menu.Item(hero.BaseSkinName).GetValue<bool>())
                    {
                        PredictionOutput pred = E.GetPrediction(hero);

                        int pushDist = Menu.Item("PushDistance").GetValue<Slider>().Value;
                        for (int i = 0; i < pushDist; i += (int) hero.BoundingRadius)
                        {
                            Vector2 location = V2E(Player.Position, pred.CastPosition, i);
                            Vector3 loc2 = new Vector3(location.X, Player.Position.Y, location.Y);
                            Vector3 loc3 =
                               pred.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -i)
                                    .To3D();
                            if (IsWall(loc3))
                            {
                                E.Cast(hero);
                                break;
                            }
                        }
                    }
                }
                
            }
        }

        public static bool IsWall(Vector3 position)
        {
            CollisionFlags cFlags = NavMesh.GetCollisionFlags(position);
            //|| CollisionFlags.Prop
            return (cFlags == CollisionFlags.Wall || cFlags == CollisionFlags.Building);
        }

        private static void GpIntmenuCreate()
        {
            Gapcloser = new[]
            {
                "鍏▅AkaliShadowDance", "寮€|Headbutt", "灏眧DianaTeleport", "瀵箌IreliaGatotsu", "浜唡JaxLeapStrike", "JayceToTheSkies",
                "MaokaiUnstableGrowth", "MonkeyKingNimbus", "Pantheon_LeapBash", "PoppyHeroicCharge", "QuinnE",
                "XenZhaoSweep", "blindmonkqtwo", "FizzPiercingStrike", "RengarLeap"
            };
            Notarget = new[]
            {
                "鍏▅AatroxQ", "寮€|GragasE", "灏眧GravesMove", "瀵箌HecarimUlt", "浜唡JarvanIVDragonStrike", "JarvanIVCataclysm", "KhazixE",
                "khazixelong", "LeblancSlide", "LeblancSlideM", "LeonaZenithBlade", "UFSlash", "RenektonSliceAndDice",
                "SejuaniArcticAssault", "ShenShadowDash", "RocketJump", "slashCast"
            };
            Interrupt = new[]
            {
                "鍗＄壒R", "鍝ㄥ叺 鍢茶", "绛栧＋缁熼鎶€鑳絴", "鏀捐", "缁濆闆跺害", "鎱?绐佽繘", "棣栭涔嬪偛 R",
                "AlZaharNetherGrasp", "FallenOne", "Pantheon_GrandSkyfall_Jump", "缁撮瞾鏂疩", "CaitlynAceintheHole",
                "MissFortuneBulletTime", "InfiniteDuress", "鍗㈤敗瀹墊R"
            };
            for (int i = 0; i < Gapcloser.Length; i++)
            {
                Menu.SubMenu("gap").AddItem(new MenuItem(Gapcloser[i], Gapcloser[i])).SetValue(true);
            }
            for (int i = 0; i < Notarget.Length; i++)
            {
                Menu.SubMenu("gap2").AddItem(new MenuItem(Notarget[i], Notarget[i])).SetValue(true);
            }
            for (int i = 0; i < Interrupt.Length; i++)
            {
                Menu.SubMenu("int").AddItem(new MenuItem(Interrupt[i], Interrupt[i])).SetValue(true);
            }
        }

        private static void NoCondemnMenuCreate()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
            {
                Menu.SubMenu("CondemnHero").AddItem(new MenuItem(hero.BaseSkinName, hero.BaseSkinName)).SetValue(true);
            }
        }

        public static void CastQ(Obj_AI_Hero targ)
        {
            if (Q.IsReady())
                if (IsEn("SmartQ") && Player.Distance(targ) >= Orbwalking.GetRealAutoAttackRange(null))
                {
                    if (IsMode("Combo") && GetManaPer() >= Menu.Item("QManaC").GetValue<Slider>().Value)
                    {
                        var tumbleRange = 300f;
                        var canGapclose = Player.Distance(targ) <=
                                           Orbwalking.GetRealAutoAttackRange(null) + tumbleRange;
                        if ((!(Player.Distance(targ) >= Orbwalking.GetRealAutoAttackRange(null)))) return;
                        if (!canGapclose) return;
                        Vector3 PositionForQ = new Vector3(targ.Position.X, targ.Position.Y, targ.Position.Z);
                        Q.Cast(PositionForQ, IsEn("UsePK"));
                    }
                    else if (IsMode("Mixed") && GetManaPer() >= Menu.Item("QManaM").GetValue<Slider>().Value)
                    {
                        var tumbleRange = 300f;
                        var canGapclose = Player.Distance(targ) <=
                                           Orbwalking.GetRealAutoAttackRange(null) + tumbleRange;
                        if ((!(Player.Distance(targ) >= Orbwalking.GetRealAutoAttackRange(null)))) return;
                        if (!canGapclose) return;
                        Vector3 PositionForQ = new Vector3(targ.Position.X, targ.Position.Y, targ.Position.Z);
                        Q.Cast(PositionForQ, IsEn("UsePK"));
                    }
                }
                else
                {
                    if (IsMode("Combo") && GetManaPer() >= Menu.Item("QManaC").GetValue<Slider>().Value)
                    {
                        Q.Cast(Game.CursorPos, IsEn("UsePK"));
                    }
                    else if (IsMode("Mixed") && GetManaPer() >= Menu.Item("QManaM").GetValue<Slider>().Value)
                    {
                        Q.Cast(Game.CursorPos, IsEn("UsePK"));
                    }
                }
        }

        private static void CastE(Obj_AI_Hero Target, bool forGp = false)
        {
            if (E.IsReady() && Player.Distance(Target) < 550f)
            {
                if (!forGp)
                {
                    if (IsMode("Combo") && GetManaPer() >= Menu.Item("EManaC").GetValue<Slider>().Value)
                    {
                        E.Cast(Target, IsEn("UsePK"));
                    }
                    else if (IsMode("Mixed") && GetManaPer() >= Menu.Item("EManaM").GetValue<Slider>().Value)
                    {
                        E.Cast(Target, IsEn("UsePK"));
                    }
                }
                else
                {
                    E.Cast(Target, IsEn("UsePK"));
                }
            }
        }

        private static void UseItems(Obj_AI_Hero tar)
        {
            var ownH = GetPlHPer();
            if (Menu.Item("Botrk").GetValue<bool>() && (Menu.Item("OwnHPercBotrk").GetValue<Slider>().Value <= ownH) &&
                ((Menu.Item("EnHPercBotrk").GetValue<Slider>().Value <= GetEnH(tar))))
            {
                UseItem(3153, tar);
            }
            if (Menu.Item("Youmuu").GetValue<bool>())
            {
                UseItem(3142);
            }
        }

        private static bool IsEn(String opt)
        {
            return Menu.Item(opt).GetValue<bool>();
        }

        public static float GetManaPer()
        {
            float mana = (Player.Mana/Player.MaxMana)*100;
            return mana;
        }

        private static bool IsEnK(String opt)
        {
            return Menu.Item(opt).GetValue<KeyBind>().Active;
        }

        private static bool IsMode(String mode)
        {
            return (Orbwalker.ActiveMode.ToString() == mode);
        }

        public static void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        public static float GetEnH(Obj_AI_Hero target)
        {
            float h = (target.Health/target.MaxHealth)*100;
            return h;
        }

        public static float GetPlHPer()
        {
            float h = (Player.Health/Player.MaxHealth)*100;
            return h;
        }

        /// <summary>
        ///     Extends a vector using the params from, direction, distance.Credits to princer007
        /// </summary>
        /// <param name="from"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance*Vector3.Normalize(direction - from).To2D();
        }
    }
}