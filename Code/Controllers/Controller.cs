using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.Client;
using Celeste.Mod.CelesteNet.Client.Components;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.CelesteNet.DataTypes;
using Celeste.Mod.Entities.SpawnPoints;
using Celeste.Mod.HeartWars.Entities.Renderers;
using Celeste.Mod.HeartWars.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.HeartWars.Controllers {
    public class Controller : GameComponent {
        public static CelesteNetClientModule module;
        private static Game game;

        public static Player player;
        public static Level level;
        public static bool featuresEnabled;

        private Delegate initHook;
        private Delegate disposeHook;
        private Hook ghostNameRenderHook, celesteNetChatComponentHandleHook;

        private Hook onPlayerExit;

        private ConcurrentQueue<Action> _updateQueue = new ConcurrentQueue<Action>();
        public Controller(Game game) : base(game) {
            HeartWarsModule.info("Getting CelesteNET by Everest API");
            if ((module = (CelesteNetClientModule) Everest.Modules.FirstOrDefault(m => m is CelesteNetClientModule)) == null) throw new Exception("CelesteNET not loaded!");

            Everest.Events.Level.OnLoadLevel += onLoadLevel;
            Everest.Events.Level.OnExit += onExitLevel;
            On.Celeste.PlayerHair.GetHairColor += onGetHairColor;
            On.Celeste.Player.Die += GameController.onPlayerDie;

            EventInfo initEvt = typeof(CelesteNetClientContext).GetEvent("OnInit");
            if(initEvt.EventHandlerType.GenericTypeArguments[0] == typeof(CelesteNetClientContext)) {
                initEvt.AddEventHandler(null, initHook = (Action<CelesteNetClientContext>) (_ => clientInit()));
            } else {
                initEvt.AddEventHandler(null, initHook = (Action<object>) (_ => clientInit()));
            }

            EventInfo disposeEvt = typeof(CelesteNetClientContext).GetEvent("OnDispose");
            if(disposeEvt.EventHandlerType.GenericTypeArguments[0] == typeof(CelesteNetClientContext)) {
                disposeEvt.AddEventHandler(null, disposeHook = (Action<CelesteNetClientContext>) (_ => clientDispose()));
            } else {
                disposeEvt.AddEventHandler(null, disposeHook = (Action<object>) (_ => clientDispose()));
            }

            ghostNameRenderHook = new Hook(typeof(GhostNameTag).GetMethod(nameof(GhostNameTag.Render)), GetType().GetMethod(nameof(GhostNameTagRenderHook), BindingFlags.NonPublic | BindingFlags.Instance), this);
            celesteNetChatComponentHandleHook = new Hook(typeof(CelesteNetChatComponent).GetMethod(nameof(CelesteNetChatComponent.Handle)), GetType().GetMethod(nameof(CelesteNetChatComponentHandleHook), BindingFlags.NonPublic | BindingFlags.Instance), this);
            Controller.game = game;

            InventoryController.initialize();
        }
        private void clientInit() {
            HeartWarsModule.info("CelesteNET init event handled");
            module.Context.Client.Data.RegisterHandlersIn(this);
            // module.Context.Client.Con.OnDisconnect += onDisconnected;
        }

        private static void clientDispose() {
        }

        public void Unload() {
            Everest.Events.Level.OnLoadLevel -= onLoadLevel;
            Everest.Events.Level.OnExit -= onExitLevel;
            On.Celeste.PlayerHair.GetHairColor -= onGetHairColor;

            if (module.Context?.Client != null) {
                HeartWarsModule.info("Unregister handlers");
                module.Context.Client.Data.UnregisterHandlersIn(this);
            }

            // module.Context.Client.Con.OnDisconnect -= onDisconnected;

            if(initHook != null) typeof(CelesteNetClientContext).GetEvent("OnInit").RemoveEventHandler(null, initHook);
            initHook = null;

            if(disposeHook != null) typeof(CelesteNetClientContext).GetEvent("OnDispose").RemoveEventHandler(null, disposeHook);
            disposeHook = null;

            ghostNameRenderHook.Dispose();
            celesteNetChatComponentHandleHook.Dispose();
            ghostNameRenderHook = null;
            celesteNetChatComponentHandleHook = null;
        }

        public static void enableFeatures() {
            game.IsMouseVisible = true;
            if (!featuresEnabled) {
                level.Add(Engine.Pooler.Create<HeartWarsRenderer>().initialize(player));
            }
            featuresEnabled = true;
        }

        public override void Update(GameTime gameTime) {
            handleMouse();
            var queue = _updateQueue;
            _updateQueue = new ConcurrentQueue<Action>();
            foreach (var action in queue) action();
            base.Update(gameTime);
        }

        public static void disableFeatures() {
            game.IsMouseVisible = false;
            featuresEnabled = false;
        }
        
        public static void onLoadLevel(Level level, Player.IntroTypes intro, bool fromLoader) {
            Controller.level = level;
            PlayerInfo.initialize();
            InventoryController.initialize();
            PlayerInfo.Area = level.Session.Area.SID;
            player = level.Tracker.GetEntity<Player>();
            if (level.Tracker.GetEntities<DefaultSpawnPoint>().Count == 1) {
                level.Session.RespawnPoint = player.Position = level.Tracker.GetEntity<DefaultSpawnPoint>().Position;
                GameController.broadcast("Thanks for playing HeartWars, please select a team and start the game.");
            }
        }
        public static void onExitLevel(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow) {
            disableFeatures();
            PlayerInfo.initialize();
            GameController.isWaitingForGameStartResponse = false;
        }

        private static bool releasedLeftButton = false;
        private static DateTime lastShoot = DateTime.UtcNow;

        private static void handleMouse() {
            if (!featuresEnabled || level.Paused) return;
            MouseState mouseState = MInput.Mouse.CurrentState;
            if (mouseState.LeftButton == ButtonState.Pressed && releasedLeftButton) {
                releasedLeftButton = false;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                float H = Engine.Graphics.PreferredBackBufferHeight;
                float W = Engine.Graphics.PreferredBackBufferWidth;
                float scale = Calc.Min(1f * H / 9, 1f * W / 16);
                float scaledH = scale * 9;
                float scaledW = scale * 16;
                float deltaH = H / 2 - scaledH / 2;
                float deltaW = W / 2 - scaledW / 2;
                mousePosition.X -= deltaW;
                mousePosition.Y -= deltaH;
                Camera cam = level.Camera;
                float viewScale = (float) Engine.ViewWidth / Engine.Width;
                Vector2 mouseWorldPosition = Calc.Floor(level.ScreenToWorld(mousePosition / viewScale));
                Vector2 PlayerHead = (player.Position + player.TopCenter) / 2;
                Vector2 dir = (mouseWorldPosition - PlayerHead).SafeNormalize();
                if ((DateTime.Now - lastShoot).TotalSeconds >= PlayerInfo.WeaponInfoCooldown) {
                    if (PlayerInfo.Hand == ItemType.GUN) {
                        doMissileShoot(dir, PlayerHead);
                    } else if (PlayerInfo.Hand == ItemType.FIREBALL) {
                        InventoryController.inventory.ForEach(i => {
                            if (i is FireBallItem) {
                                i.count -= 1;
                                if (i.count == 0) {
                                    PlayerInfo.Hand = ItemType.GUN;
                                }
                            }
                        });
                        doFireBallShoot(dir, PlayerHead);
                    } else if (PlayerInfo.Hand == ItemType.BADPEARL) {
                        InventoryController.inventory.ForEach(i => {
                            if (i is BadPearlItem) {
                                i.count -= 1;
                                if (i.count == 0) {
                                    PlayerInfo.Hand = ItemType.GUN;
                                }
                            }
                        });
                        doBadPearlShoot(dir, PlayerHead);
                    }
                    lastShoot = DateTime.Now;
                }
            } else if (mouseState.LeftButton == ButtonState.Released && !releasedLeftButton) {
                releasedLeftButton = true;
            }
        }

        public static void sendData(DataType data) {
            if (module.Context?.Client == null) return;
            module.Context.Client.Send(data);
        }

        public static void sendAndHandleData(DataType data) {
            if (module.Context?.Client == null) return;
            module.Context.Client.SendAndHandle(data);
        }

        private static void doMissileShoot(Vector2 dir, Vector2 pos) {
            if (module.Context?.Client == null) return;
            PlayerInfo.MissileIndex++;
            GameController.createMissile(PlayerInfo.getPlayerName(), PlayerInfo.PlayerTeam, dir, pos, PlayerInfo.MissileSpeed, PlayerInfo.MissileDamage, PlayerInfo.MissileMaxTime, PlayerInfo.MissileIndex);
            sendData(new DataMissileShoot() {
                Sender = module.Context.Client.PlayerInfo,
                area = level.Session.Area.SID,
                shooterName = PlayerInfo.getPlayerName(),
                shooterTeam = PlayerInfo.PlayerTeam,
                dir = dir,
                pos = pos,
                speed = PlayerInfo.MissileSpeed,
                damage = PlayerInfo.MissileDamage,
                maxTime = PlayerInfo.MissileMaxTime,
                id = PlayerInfo.MissileIndex
            });
        }

        private static void doFireBallShoot(Vector2 dir, Vector2 pos) {
            if (module.Context?.Client == null) return;
            GameController.createFireBall(PlayerInfo.getPlayerName(), PlayerInfo.PlayerTeam, dir, pos);
            sendData(new DataFireBallShoot() {
                Sender = module.Context.Client.PlayerInfo,
                area = level.Session.Area.SID,
                shooterName = PlayerInfo.getPlayerName(),
                shooterTeam = PlayerInfo.PlayerTeam,
                dir = dir,
                pos = pos,
            });
        }

        private static void doBadPearlShoot(Vector2 dir, Vector2 pos) {
            if (module.Context?.Client == null) return;
            GameController.createBadPearl(PlayerInfo.getPlayerName(), PlayerInfo.PlayerTeam, dir, pos);
            sendData(new DataBadPearlShoot() {
                Sender = module.Context.Client.PlayerInfo,
                area = level.Session.Area.SID,
                shooterName = PlayerInfo.getPlayerName(),
                shooterTeam = PlayerInfo.PlayerTeam,
                dir = dir,
                pos = pos,
            });
        }

        public void Handle(CelesteNetConnection con, DataMissileShoot data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.createMissile(data.shooterName, data.shooterTeam, data.dir, data.pos, data.speed, data.damage, data.maxTime, data.id);
            });
        }

        public void Handle(CelesteNetConnection con, DataFireBallShoot data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.createFireBall(data.shooterName, data.shooterTeam, data.dir, data.pos);
            });
        }

        public void Handle(CelesteNetConnection con, DataBadPearlShoot data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.createBadPearl(data.shooterName, data.shooterTeam, data.dir, data.pos);
            });
        }

        public void Handle(CelesteNetConnection con, DataPlayerDie data) {
            if (!featuresEnabled || data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.handlePlayerDie(data);
            });
        }

        public void Handle(CelesteNetConnection con, DataGameStartRequest data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.handleGameStartRequest(data);
            });
        }

        public void Handle(CelesteNetConnection con, DataGameStartResponse data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.handleGameStartResponse(data);
            });
        }

        public void Handle(CelesteNetConnection con, DataGameStart data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.handleGameStart(data);
            });
        }

        // public void Handle(CelesteNetConnection con, DataGameEnd data) {
        //     if (data.area != Controller.level.Session.Area.SID) return;
        //     _updateQueue.Enqueue(() => {
        //         GameController.handleGameEnd(data);
        //     });
        // }

        public void Handle(CelesteNetConnection con, DataHeartBreak data) {
            if (data.area != Controller.level.Session.Area.SID) return;
            _updateQueue.Enqueue(() => {
                GameController.handleHeartBreak(data);
            });
        }

        private void GhostNameTagRenderHook(Action<GhostNameTag> orig, GhostNameTag nameTag) {
            if (!featuresEnabled) orig(nameTag);
            if (PlayerInfo.PlayerTeam == "RED" && GameController.RedTeamPlayers.Contains(nameTag.Name.Trim())) orig(nameTag);
            if (PlayerInfo.PlayerTeam == "BLUE" && GameController.BlueTeamPlayers.Contains(nameTag.Name.Trim())) orig(nameTag);
        }

        private delegate void orig_HandleMessage(CelesteNetChatComponent self, CelesteNetConnection con, DataChat msg);
        private void CelesteNetChatComponentHandleHook(orig_HandleMessage orig, CelesteNetChatComponent self, CelesteNetConnection con, DataChat msg) {
            for (int i = 0; i < self.Log.Count; i++) {
                if (i >= self.Log.Count) break;
                if (self.Log[i]?.Text?.Contains(HeartWarsModule.IdentifyCode) == true) {
                    self.Log.Remove(self.Log[i]);
                }
            }
            if (msg.Text.Trim().Contains(HeartWarsModule.IdentifyCode)) {
                if (GameController.messagesUnbroadcasted == "") {
                    msg.Text = "";
                    msg.ID = 0;
                    return;
                }
                msg.Text = GameController.messagesUnbroadcasted;
                msg.Color = Color.LightCyan;
                msg.Tag = "";
                msg.Player = new DataPlayerInfo() {
                    DisplayName = "[HeartWars]",
                    Name = "[HeartWars]",
                    ID = 0,
                };
                HeartWarsModule.info("deque message: " + msg.Text);
                GameController.messagesUnbroadcasted = "";
            }
            HeartWarsModule.info("ChatData: " + msg.Version.ToString() + " " + msg.ID.ToString() + " " + msg.CreatedByServer.ToString() + " " + msg.Text);
            if (msg.Text.Trim().StartsWith("Cya, ") && msg.Player == null && featuresEnabled) {
                GameController.onPlayerExit(msg.Text.Trim().Substring(4).Trim());
            }
            orig(self, con, msg);
        }

        private static Color onGetHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index) {
            Color colorOrig = orig(self, index);
            if (!(self.Entity is Player) || (PlayerInfo.PlayerTeam != "RED" && PlayerInfo.PlayerTeam != "BLUE")) {
                return colorOrig;
            }
            else {
                return PlayerInfo.PlayerHairColor;
            }
        }

        private static void onDisconnected(CelesteNetConnection con) {
            HeartWarsModule.info("CelesteNET disconnect event hooked!");
        }
    }
}