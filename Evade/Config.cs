// Copyright 2014 - 2014 Esk0r
// Config.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Evade
{
    internal static class Config
    {
        public const bool PrintSpellData = false;
        public const bool TestOnAllies = false;
        public const int SkillShotsExtraRadius = 9;
        public const int SkillShotsExtraRange = 20;
        public const int GridSize = 10;
        public const int ExtraEvadeDistance = 15;
        public const int DiagonalEvadePointsCount = 7;
        public const int DiagonalEvadePointsStep = 20;

        public const int CrossingTimeOffset = 250;

        public const int EvadingFirstTimeOffset = 250;
        public const int EvadingSecondTimeOffset = 0;

        public const int EvadingRouteChangeTimeOffset = 250;

        public const int EvadePointChangeInterval = 300;
        public static int LastEvadePointChangeT = 0;

        public static Menu Menu;

        public static void CreateMenu()
        {
            Menu = new Menu("Evade-浜岀嫍姹夊寲", "Evade", true);

            //Create the evade spells submenus.
            var evadeSpells = new Menu("娉曟湳闂伩", "evadeSpells");
            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                var subMenu = new Menu(spell.Name, spell.Name);

                subMenu.AddItem(
                    new MenuItem("DangerLevel" + spell.Name, "鍗遍櫓绛夌骇").SetValue(
                        new Slider(spell.DangerLevel, 5, 1)));

                if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                {
                    subMenu.AddItem(new MenuItem("WardJump" + spell.Name, "鐬溂").SetValue(true));
                }

                subMenu.AddItem(new MenuItem("Enabled" + spell.Name, "鎵ц").SetValue(true));

                evadeSpells.AddSubMenu(subMenu);
            }
            Menu.AddSubMenu(evadeSpells);

            //Create the skillshots submenus.
            var skillShots = new Menu("鎶€鑳介€夋嫨", "Skillshots");

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.Team != ObjectManager.Player.Team || Config.TestOnAllies)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName.ToLower() == hero.ChampionName.ToLower())
                        {
                            var subMenu = new Menu(spell.MenuItemName, spell.MenuItemName);

                            subMenu.AddItem(
                                new MenuItem("DangerLevel" + spell.MenuItemName, "鍗遍櫓绛夌骇").SetValue(
                                    new Slider(spell.DangerValue, 5, 1)));

                            subMenu.AddItem(
                                new MenuItem("IsDangerous" + spell.MenuItemName, "鍗遍櫓").SetValue(
                                    spell.IsDangerous));

                            subMenu.AddItem(new MenuItem("Draw" + spell.MenuItemName, "鑼冨洿缁樺埗").SetValue(true));
                            subMenu.AddItem(new MenuItem("Enabled" + spell.MenuItemName, "鎵ц").SetValue(true));

                            skillShots.AddSubMenu(subMenu);
                        }
                    }
                }
            }

            Menu.AddSubMenu(skillShots);

            var shielding = new Menu("鎵ц淇濇姢", "Shielding");

            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (ally.IsAlly && !ally.IsMe)
                {
                    shielding.AddItem(
                        new MenuItem("shield" + ally.ChampionName, "鍚敤淇濇姢" + ally.ChampionName).SetValue(true));
                }
            }
            Menu.AddSubMenu(shielding);

            var collision = new Menu("鎵ц鎶垫尅", "Collision");
            collision.AddItem(new MenuItem("MinionCollision", "鍏电嚎鎶垫尅").SetValue(false));
            collision.AddItem(new MenuItem("HeroCollision", "鑻遍泟鎶垫尅").SetValue(false));
            collision.AddItem(new MenuItem("YasuoCollision", "浜氱储W鎶垫尅").SetValue(true));
            collision.AddItem(new MenuItem("EnableCollision", "鍚敤鎶垫尅").SetValue(true));
            //TODO add mode.
            Menu.AddSubMenu(collision);

            var drawings = new Menu("缁樺埗鑼冨洿", "Drawings");
            drawings.AddItem(new MenuItem("EnabledColor", "浣跨敤鑼冨洿棰滆壊").SetValue(Color.White));
            drawings.AddItem(new MenuItem("DisabledColor", "绂佺敤鑼冨洿棰滆壊").SetValue(Color.Red));
            drawings.AddItem(new MenuItem("MissileColor", "鑼冨洿棰滆壊").SetValue(Color.LimeGreen));
            drawings.AddItem(new MenuItem("Border", "鑼冨洿瀹藉害").SetValue(new Slider(1, 5, 1)));

            drawings.AddItem(new MenuItem("EnableDrawings", "鍚敤鑼冨洿鏄剧ず").SetValue(true));
            Menu.AddSubMenu(drawings);

            var misc = new Menu("鏉傞」璁剧疆", "Misc");
            misc.AddItem(new MenuItem("DisableFow", "绂佺敤鎴樹簤杩烽浘").SetValue(false));
            Menu.AddSubMenu(misc);

            Menu.AddItem(
                new MenuItem("Enabled", "鍚敤").SetValue(new KeyBind("K".ToCharArray()[0], KeyBindType.Toggle, true)));

            Menu.AddItem(
                new MenuItem("OnlyDangerous", "鍙翰閬胯嚧鍛芥妧鑳絴").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu.AddToMainMenu();
			
			
		}
    }
}