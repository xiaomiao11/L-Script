using System;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace Leesin
{
    internal class LeeSinSharp
    {
        //public static Obj_AI_Hero Player = ObjectManager.Player;

        public static SpellSlot SmiteSlot;
        public static SpellSlot FlashSlot;
        private static Obj_AI_Hero _target;
        public LeeSinSharp()
        {

            LeeMethods.Player = ObjectManager.Player;
            SmiteSlot = LeeMethods.Player.GetSpellSlot("SummonerSmite");
            FlashSlot = LeeMethods.Player.GetSpellSlot("summonerflash");
            LeeMethods.Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
            LeeMethods.R.SetTargetted(0.25f, float.MaxValue);
            Config.SpellList.Add(LeeMethods.Q);
            Config.SpellList.Add(LeeMethods.W);
            Config.SpellList.Add(LeeMethods.E);
            Config.SpellList.Add(LeeMethods.R);

            // ReSharper disable once ObjectCreationAsStatement
            new Config();
            if (Config.StealthChampiopns)
            {
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += OnDraw;
            LeeUtility.SendMessage("Loaded Version: " + Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //Console.WriteLine("called1");
            if (LeeUtility.WaittingForWard <= Environment.TickCount || !(sender is Obj_AI_Base) || sender.IsEnemy)
            {
                return;
            }
            var wardObject = (Obj_AI_Base) sender;
            if (wardObject.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                Vector3.DistanceSquared(sender.Position, LeeUtility.WardCastPosition) <= 150 * 150)
            {
                LeeMethods.W.Cast(wardObject);
            }
        }

        private void OnDraw(EventArgs args)
        {

            try
            {
                foreach (var spell in Config.SpellList)
                {
                    var menuItem = Config.Menu.Item(spell.Slot + "Draw").GetValue<Circle>();
                    if (menuItem.Active)
                    {
                        Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                    }
                }
                var insecItem = Config.Menu.Item("drawInsec").GetValue<Circle>();
                if (insecItem.Active && _target.IsValidTarget() && (Config.Menu.Item("insec1").GetValue<KeyBind>().Active ||
                        Config.Menu.Item("insec2").GetValue<KeyBind>().Active))
                {
                    var startPoint = _target.Position.To2D();
                    var insecPoint = LeeUtility.GetInsecVector3(_target, false, LeeMethods.Player.Position.To2D()).To2D();
                    var endPoint = startPoint.Extend(insecPoint, -1200);
                    var insecRectangle = LeeUtility.Rectangle(startPoint, endPoint, _target.BoundingRadius);
                    for (int i = 0; i < insecRectangle.Count; i++)
                    {
                        Drawing.DrawLine(Drawing.WorldToScreen(insecRectangle[i].To3D()), Drawing.WorldToScreen(insecRectangle[i == 3 ? 0 : i + 1].To3D()), 1, insecItem.Color);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            /*try
            {
                var startPos = LeeMethods.Player.ServerPosition.To2D();
                var endPos = LeeMethods.Player.ServerPosition.To2D().Extend(Game.CursorPos.To2D(), 1200);
                Utility.DrawCircle(endPos.To3D(),10,Color.Red);
                Utility.DrawCircle(startPos.To3D(), 10, Color.Red);

                var rectangle = LeeUtility.Rectangle(startPos, endPos, LeeMethods.Player.BoundingRadius);
                Utility.DrawCircle(rectangle[0].To3D(), 10, Color.Red);
                Utility.DrawCircle(rectangle[1].To3D(), 10, Color.Green);
                Utility.DrawCircle(rectangle[2].To3D(), 10, Color.Blue);
                Utility.DrawCircle(rectangle[3].To3D(), 10, Color.Black);

                Drawing.DrawLine(Drawing.WorldToScreen(rectangle[0].To3D()), Drawing.WorldToScreen(rectangle[1].To3D()), 1, Color.Red);
                Drawing.DrawLine(Drawing.WorldToScreen(rectangle[1].To3D()), Drawing.WorldToScreen(rectangle[2].To3D()), 1, Color.Green);
                Drawing.DrawLine(Drawing.WorldToScreen(rectangle[2].To3D()), Drawing.WorldToScreen(rectangle[3].To3D()), 1, Color.Blue);
                Drawing.DrawLine(Drawing.WorldToScreen(rectangle[3].To3D()), Drawing.WorldToScreen(rectangle[0].To3D()), 1, Color.Black);

                var polygon = new Polygon(rectangle);
                if (polygon.Contains(Game.CursorPos.To2D()))
                {
                    Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Process();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }*/
            //var hero1 = hero;

            //Drawing.DrawText(100, 100, Color.White, "Harass Stage: " + LeeMethods.harassStage);
            //Console.WriteLine(ObjectManager.Player.BoundingRadius);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Config.Menu.Item("wardJump").GetValue<KeyBind>().Active)
                {
                    LeeMethods.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    LeeUtility.WardJump(Game.CursorPos, true, true, LeeUtility.MenuParamBool("wardFullRange"));
                }

                if (SmiteSlot != SpellSlot.Unknown && Config.Menu.Item("stealCamp").GetValue<KeyBind>().Active)
                {
                    LeeMethods.CampStealer();
                }
                if (SmiteSlot != SpellSlot.Unknown && Config.Menu.Item("smiteEnabled").GetValue<KeyBind>().Active)
                {
                    LeeMethods.Smite();
                }
                //Console.WriteLine(LeeUtility.MenuParamBool("enabledKS"));
                if (Config.Menu.Item("enabledKS").GetValue<KeyBind>().Active)
                {
                    LeeMethods.KSer();
                }

                _target = SimpleTs.GetTarget(
                    LeeMethods.Q.IsReady() ? LeeMethods.Q.Range : LeeMethods.R.Range, SimpleTs.DamageType.Physical);
                LeeMethods.InsecCombo(_target);
                switch (Config.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        LeeMethods.Combo(_target);
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        LeeMethods.Harass(_target);
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LeeMethods.LaneClear();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || !sender.Type.Equals(GameObjectType.obj_AI_Hero) ||
                (((Obj_AI_Hero) sender).ChampionName != "MonkeyKing" && ((Obj_AI_Hero) sender).ChampionName != "Akali") ||
                Vector3.DistanceSquared(sender.ServerPosition, LeeMethods.Player.ServerPosition) >= 350 * 350 ||
                !LeeMethods.E.IsReady())
            {
                return;
            }
            if (args.SData.Name == "MonkeyKingDecoy" || args.SData.Name == "AkaliSmokeBomb")
            {
                LeeMethods.E.Cast();
            }
        }
    }
}