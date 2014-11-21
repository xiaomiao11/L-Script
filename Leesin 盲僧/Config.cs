using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Leesin
{
    class Config
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static bool StealthChampiopns;
        public static readonly List<Spell> SpellList = new List<Spell>();
        public Config()
        {
            StealthChampiopns = ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsEnemy && (hero.ChampionName == "Akali" || hero.ChampionName == "MonkeyKing"));

            Console.WriteLine(Utility.Map.GetMap().Name);
            if (Game.MapId == GameMapId.SummonersRift)
            {
                LeeMethods.JungleCamps.Add("Worm");
                LeeMethods.JungleCamps.Add("Dragon");
                LeeMethods.JungleCamps.Add("AncientGolem");
                LeeMethods.JungleCamps.Add("LizardElder");
                
                LeeMethods.SmallMinionCamps.Add("Wraith");
                LeeMethods.SmallMinionCamps.Add("Golem");
                LeeMethods.SmallMinionCamps.Add("GreatWraith");
                LeeMethods.SmallMinionCamps.Add("GiantWolf");
            }
            else if(Game.MapId == (GameMapId)11)
            {
                LeeMethods.JungleCamps.Add("SRU_Baron");
                LeeMethods.JungleCamps.Add("SRU_Dragon");
                LeeMethods.JungleCamps.Add("SRU_Blue");
                LeeMethods.JungleCamps.Add("SRU_Red");

                LeeMethods.SmallMinionCamps.Add("SRU_Razorbeak");
                LeeMethods.SmallMinionCamps.Add("SRU_Krug");
                LeeMethods.SmallMinionCamps.Add("SRU_Gromp");
                LeeMethods.SmallMinionCamps.Add("SRU_Murkwolf");

            }

            Menu = new Menu("盲僧#二狗汉化", "LeeSinSharp", true);
            //Target Selector
            var targetSelector = new Menu("目标选择", "TargetSelector");
            SimpleTs.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);
            //Orbwalker
            Menu.AddSubMenu(new Menu("走砍", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));
            //
            //Combo Menu
            //
            Menu.AddSubMenu(new Menu("连招", "Combo"));
            //Normal Combo
            Menu.SubMenu("Combo").AddSubMenu(new Menu("连招设置", "ComboSettings")); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useQC", "使用 Q").SetValue(true)); //Done
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("smiteCombo", "自动惩戒阻挡R的小兵|").SetValue(true));  //Done
            }
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW1C", "使用 W1").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW2C", "使用 W2").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE1C", "使用 E1").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE2C", "使用 E2").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("aaBetween", "使用连招敌方人数").SetValue(new Slider(1, 0, 2))); //Done
            //Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E1CMode", "E1 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            //Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E2CMode", "E2 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            //Insec Combo
            Menu.SubMenu("Combo").AddSubMenu(new Menu("大招设置", "InsecSettings"));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToTower", "R向炮塔|").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToAlly", "R向盟友|").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToMouse", "R向鼠标位置|").SetValue(false));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("moveToMouse", "移动到鼠标位置|").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("useFlashInsec", "使用闪现").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecDistance", "显示R距离位置").SetValue(new Slider(250,100,375)));
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("smiteInsec", "自动惩戒阻挡R的小兵|").SetValue(true)); //Done 
            }
            //Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecOrder", "Insec order").SetValue(new StringList(new[] { "R -> Flash/W", "Flash/W -> R" })));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecMode", "R模式").SetValue(new StringList(new[] { "优先W", "优先闪现" })));
            //General Combo Seetings
            Menu.SubMenu("Combo").AddItem(new MenuItem("infoText1", "连招键位: \"" + Utils.KeyToText(Menu.Item("Orbwalk").GetValue<KeyBind>().Key)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec1", "Q -> R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec2", "R -> Q").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            //
            //Harass Menu
            //
            Menu.AddSubMenu(new Menu("骚扰", "Harass")); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQ1H", "使用 Q1").SetValue(true)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQ2H", "使用 Q2").SetValue(false)); //Done
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Harass").AddItem(new MenuItem("smiteHarass", "自动惩戒阻挡R的小兵|").SetValue(false)); //Done
            }
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWH", "使用 W(QQ>跳回)").SetValue(false)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWardWH", "使用 W 跳回").SetValue(false)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseE1H", "使用 E1").SetValue(true)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseE2H", "使用 E2").SetValue(true)); //Done

            Menu.SubMenu("Harass").AddItem(new MenuItem("infoText2", "骚扰键位: \"" + Utils.KeyToText(Menu.Item("Farm").GetValue<KeyBind>().Key))); //Done
            //
            //Laneclear Menu
            //
            Menu.AddSubMenu(new Menu("清线", "LaneClear"));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("infoText3", "清线|清野键位: \"" + Utils.KeyToText(Menu.Item("LaneClear").GetValue<KeyBind>().Key))); //Done
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseQW", "使用 Q").SetValue(true));  //Done 
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseWW", "使用 W").SetValue(false));  
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseEW", "使用 E").SetValue(true));  //Done 
            //
            //KillSteal Menu
            //
            Menu.AddSubMenu(new Menu("抢人头|", "KillSteal"));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("enabledKS", "启用")).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle, true));//Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useQ1KS", "使用 Q1").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "使用 Q2").SetValue(false)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useE1KS", "使用 E1").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRKS", "使用 R").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("rOverKill", "使用R结束击杀血�?").SetValue(new Slider(50))); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRCollisionKS", "使用R-Q击杀").SetValue(true)); // Done

            //
            //Jungle Menu
            //
            Menu.AddSubMenu(new Menu("打野设置", "Jungle"));
            Menu.SubMenu("Jungle")
                .AddItem(new MenuItem("smiteEnabled", "使用惩戒").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Press))); //Done
            Menu.SubMenu("Jungle").AddSubMenu(new Menu("惩戒BUFF野怪|","buffCamp"));
            foreach (var jungleCamp in LeeMethods.JungleCamps)
            {
                Menu.SubMenu("Jungle").SubMenu("buffCamp").AddItem(new MenuItem(jungleCamp, (LeeSinSharp.SmiteSlot != SpellSlot.Unknown ? "Smite " : "Steal ") + jungleCamp.Remove(0,4)).SetValue(true)); //Done
            }
            Menu.SubMenu("Jungle").AddSubMenu(new Menu("惩戒小野怪|", "smallCamp"));
            foreach (var smallMinionCamp in LeeMethods.SmallMinionCamps)
            {
                Menu.SubMenu("Jungle").SubMenu("smallCamp").AddItem(new MenuItem(smallMinionCamp, (LeeSinSharp.SmiteSlot != SpellSlot.Unknown ? "Smite " : "Steal ") + smallMinionCamp.Remove(0, 4)).SetValue(false)); //Done
            }
            Menu.SubMenu("Jungle")
                .AddItem(new MenuItem("stealCamp", "自动惩戒野怪|").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
            //
            //Drawing Menu
            //
            Menu.AddSubMenu(new Menu("范围设置", "Draw"));
            foreach (var spell in SpellList)
            {
                Menu.SubMenu("Draw")
                    .AddItem(
                        new MenuItem(spell.Slot + "Draw", "Draw " + spell.Slot + "range").SetValue(
                            new Circle(true, Color.FromArgb(128, 128, 0, 128))));
            }
            Menu.SubMenu("Draw")
                .AddItem(new MenuItem("drawInsec", "显示踢回敌人位置|").SetValue(new Circle(true, Color.Red)));
            //
            //Misc Menu
            //
            Menu.AddSubMenu(new Menu("杂项", "Misc"));
            if (StealthChampiopns)
            {
                Menu.SubMenu("Misc").AddItem(new MenuItem("autoEEStealth", "自动EE隐身单位|).").SetValue(true));     //Done           
            }
            Menu.SubMenu("Misc").AddItem(new MenuItem("wardJump", "瞬眼").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press))); //Done
            Menu.SubMenu("Misc").AddItem(new MenuItem("wardFullRange", "瞬眼范围|").SetValue(false));                //Done
            Menu.SubMenu("Misc").AddItem(new MenuItem("packetCast", "使用封包").SetValue(false));                //Done
            Menu.SubMenu("Misc").AddItem(new MenuItem("qHitchance", "Q 命中率|").SetValue(new StringList(new[] { "低|", "中|", "高|" }, 1)));
            //Add to main menu
            Menu.AddToMainMenu();

        }
    }
}
