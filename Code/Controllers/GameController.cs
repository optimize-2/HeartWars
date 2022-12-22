using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CelesteNet.DataTypes;
using Celeste.Mod.Entities.SpawnPoints;
using Celeste.Mod.HeartWars.Entities.Equipments;
using Celeste.Mod.HeartWars.Entities.Functions;
using Celeste.Mod.HeartWars.Entities.Hearts;
using Celeste.Mod.HeartWars.Entities.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.HeartWars.Controllers {
    public class GameController : GameComponent {
        private static Game game;

        public static HashSet<string> RedTeamPlayers = new HashSet<string>(), BlueTeamPlayers = new HashSet<string>();
        public static Dictionary<KeyValuePair<String, int>, Missile> missiles = new Dictionary<KeyValuePair<string, int>, Missile>();

        private static DateTime lastGameStartRequest;
        public static DateTime lastGameStartResponse;
        public static DateTime gameStartTime;

        public static bool isWaitingForGameStartResponse = false;

        public static bool unhandledGameEnd = false;

        public static bool RedTeamHeartBroke = false;
        public static bool BlueTeamHeartBroke = false;

        public static bool disableDash = false;

        public static float regenerationTime;

        public static string messagesUnbroadcasted;

        public GameController(Game game) : base(game) {
            GameController.game = game;
            initialize();
        }

        public void Unload() {
        }

        public static void initialize() {
            isWaitingForGameStartResponse = false;
            unhandledGameEnd = false;
            RedTeamHeartBroke = false;
            BlueTeamHeartBroke = false;
            disableDash = false;
        }

        public override void Update(GameTime gameTime) {
            regenerationTime -= Engine.DeltaTime;
            if (PlayerInfo.RegenerationEffect >= 0) {
                PlayerInfo.RegenerationEffect -= Engine.DeltaTime;
                if (regenerationTime <= 0) {
                    regenerationTime = 1f;
                    PlayerInfo.PlayerHealth = Calc.Min(PlayerInfo.PlayerHealth + 1, 100f);
                }
            } else {
                if (regenerationTime <= 0) {
                    regenerationTime = 2f;
                    PlayerInfo.PlayerHealth = Calc.Min(PlayerInfo.PlayerHealth + 1, 100f);
                }
            }
            if (PlayerInfo.DamageAbsorbEffect >= 0) {
                PlayerInfo.DamageAbsorbEffect -= Engine.DeltaTime;
            } else {
                PlayerInfo.PlayerExtraHealth = 0f;
            }
            if ((DateTime.UtcNow - lastGameStartRequest).TotalSeconds >= HeartWarsModule.maxPing && isWaitingForGameStartResponse) {
                isWaitingForGameStartResponse = false;
                HeartWarsModule.info("DataGameStartRequest expired");

                GameStartButton gameStartButton = Controller.player?.Scene?.Tracker?.GetEntity<GameStartButton>();
                if (gameStartButton == null) {
                    return;
                }
                HeartWarsModule.info("R/B players: " + RedTeamPlayers.Count + " " + BlueTeamPlayers.Count);
                if (RedTeamPlayers.Count == 0) {
                    gameStartButton.setContent(Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_FAIL_RED"));
                    gameStartButton.onGameMatchFailed();
                } else if (BlueTeamPlayers.Count == 0) {
                    gameStartButton.setContent(Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_FAIL_BLUE"));
                    gameStartButton.onGameMatchFailed();
                } else {
                    sendGameStart();
                }
            }
            base.Update(gameTime);
        }

        public static PlayerDeadBody onPlayerDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
            if (Controller.featuresEnabled) {
                damage(1, new DataPlayerDie() {
                    Sender = Controller.module.Context.Client.PlayerInfo,
                    area = PlayerInfo.Area,
                    playerName = PlayerInfo.getPlayerName(),
                    playerTeam = PlayerInfo.PlayerTeam,
                    killerName = "NULL",
                    killerTeam = "NULL",
                    money = 0
                }, self.Position, false);
                return null;
            }
            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }

        public static void sendGameStartRequest() {
            isWaitingForGameStartResponse = true;
            RedTeamPlayers = new HashSet<string>();
            BlueTeamPlayers = new HashSet<string>();
            lastGameStartRequest = DateTime.UtcNow;
            HeartWarsModule.info("DataGameStartRequest time: " + lastGameStartRequest.ToString());
            if (Controller.module.Context?.Client == null) return;
            HeartWarsModule.info("Send dataGameStartRequest");
            Controller.sendAndHandleData(new DataGameStartRequest() {
                Sender = Controller.module.Context.Client.PlayerInfo,
                area = PlayerInfo.Area
            });
        }

        public static void sendGameStartResponse() {
            lastGameStartResponse = DateTime.UtcNow;
            if (Controller.module.Context?.Client == null) return;
            Controller.sendAndHandleData(new DataGameStartResponse() {
                Sender = Controller.module.Context.Client.PlayerInfo,
                area = PlayerInfo.Area,
                team = PlayerInfo.PlayerTeam,
                name = PlayerInfo.getPlayerName()
            });
        }

        public static void sendGameStart() {
            lastGameStartResponse = DateTime.UtcNow;
            if (Controller.module.Context?.Client == null) return;
            Controller.sendAndHandleData(new DataGameStart() {
                Sender = Controller.module.Context.Client.PlayerInfo,
                area = PlayerInfo.Area
            });
        }

        public static void sendHeartBreak() {
            lastGameStartResponse = DateTime.UtcNow;
            if (Controller.module.Context?.Client == null) return;
            Controller.sendData(new DataHeartBreak() {
                Sender = Controller.module.Context.Client.PlayerInfo,
                area = PlayerInfo.Area,
                player = PlayerInfo.getPlayerName(),
                team = PlayerInfo.PlayerTeam == "RED" ? "BLUE" : "RED"
            });
        }

        private static void gameEnd(String team) {
            Controller.level.Session.RespawnPoint = Controller.player.Position = Controller.level.Tracker.GetEntities<DefaultSpawnPoint>().First().Position;
            PlayerInfo.initialize();
            broadcast(Dialog.Get("OPTIMIZE2_HEARTWARS_WIN_" + team));
            Controller.disableFeatures();
            GameStartButton button = Controller.level.Tracker.GetEntity<GameStartButton>();
            button.reset();
            button.setContent(Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME"));
            Controller.player.Die(new Vector2(0, 0));
        }

        private static void GameEndCheck() {
            if (RedTeamPlayers.Count == 0) {
                gameEnd("BLUE");
            } else if (BlueTeamPlayers.Count == 0) {
                gameEnd("RED");
            }
        }

        public static void onPlayerExit(String name) {
            Task.Run(() => {
                name = name.Remove(name.Length - 1, 1);
                Thread.Sleep(1000);
                DataPlayerInfo[] all = Controller.module?.Context?.Client?.Data?.GetRefs<DataPlayerInfo>();
                if (all == null) {
                    return;
                }
                if (all.FirstOrDefault(playerData => playerData.DisplayName == name) != null) {
                    HeartWarsModule.info("Fake exit message: " + name);
                } else {
                    HeartWarsModule.info("PlayerExit: " + name);
                    if (RedTeamPlayers.Contains(name)) {
                        RedTeamPlayers.Remove(name);
                    }
                    if (BlueTeamPlayers.Contains(name)) {
                        BlueTeamPlayers.Remove(name);
                    }
                    GameEndCheck();
                }
            });
        }

        public static void broadcastPlayerDie(DataPlayerDie data) {
            if (data.playerTeam == "RED" && RedTeamHeartBroke) {
                RedTeamPlayers.Remove(data.playerName);
            } else if (data.playerTeam == "BLUE" && BlueTeamHeartBroke) {
                BlueTeamPlayers.Remove(data.playerName);
            }
            GameEndCheck();
            if (data.killerTeam == "NULL") {
                broadcast(data.playerTeam + " " + data.playerName + " died");
            } else {
                broadcast(
                    data.playerTeam + " " +
                    data.playerName + " was killed by " +
                    data.killerTeam + " " + data.killerName
                );
            }
        }

        public static void broadcast(string s) {
            if (Controller.module?.Context?.Chat == null) return;
            GameController.messagesUnbroadcasted = s;
            HeartWarsModule.info("broadcast: " + s);
            HeartWarsModule.info("enqueue: " + s);
            Controller.module.Context.Chat.Send("/ " + HeartWarsModule.IdentifyCode + (++HeartWarsModule.IdentifyStamp).ToString());

            // HeartWarsModule.info("ID assigned: " + (Controller.module.Context.Chat.Log?.First()?.ID ?? 0).ToString());
            // Controller.module.Context.Chat.Handle(Controller.module.Context.Client.Con, new DataChat() {
            //     Version = (byte)(Controller.module.Context.Chat.Log?.Last()?.Version ?? 0),
            //     Tag = "[HeartWars]",
            //     Text = s,
            //     CreatedByServer = false,
            //     Color = Color.LightBlue,
            //     Date = DateTime.UtcNow,
            //     ReceivedDate = DateTime.UtcNow,
            //     ID = Controller.module.Context.Chat.Log?.First()?.ID ?? 0
            // });

            // Controller.module.Context.Chat.Log.Add(new DataChat() {
            //     Version = Controller.module.Context.Chat.Log?.Last().Version ?? 0,
            //     Tag = "[HeartWars]",
            //     Text = s,
            //     CreatedByServer = true,
            //     Color = Color.LightBlue,
            //     Date = DateTime.UtcNow,
            //     ReceivedDate = DateTime.UtcNow
            // });
        }

        public static void handlePlayerDie(DataPlayerDie data) {
            broadcastPlayerDie(data);
            if (PlayerInfo.getPlayerName() == data.killerName) {
                PlayerInfo.PlayerMoney += data.money;
            }
        }
 
        public static void handleGameStartRequest(DataGameStartRequest data) {
            HeartWarsModule.info("GameStartRequest received");
            lastGameStartRequest = DateTime.UtcNow;
            GameStartButton button = Controller.level.Tracker.GetEntity<GameStartButton>();
            button.setContent(Dialog.Get("OPTIMIZE2_HEARTWARS_START_GAME_WAITING"));
            button.matching = true;
            RedTeamPlayers.Clear();
            BlueTeamPlayers.Clear();
            if (PlayerInfo.PlayerTeam != "UNSELECTED") {
                sendGameStartResponse();
            }
        }

        public static void handleGameStartResponse(DataGameStartResponse data) {
            if (
                (DateTime.UtcNow - lastGameStartRequest).TotalSeconds <= HeartWarsModule.maxPing
            ) {
                HeartWarsModule.info("GameStartResponse: " + data.name + " " + data.team);
                if (data.team == "RED") {
                    RedTeamPlayers.Add(data.name);
                } else {
                    BlueTeamPlayers.Add(data.name);
                }
            }
        }

        public static void handleGameStart(DataGameStart data) {
            if (PlayerInfo.PlayerTeam == "RED") {
                Controller.player.Position = Controller.level.Tracker.GetEntity<RedTeamSpawnPoint>().Position;
                Controller.level.Session.RespawnPoint = Controller.level.Tracker.GetEntity<RedTeamSpawnPoint>().Position;
            } else if (PlayerInfo.PlayerTeam == "BLUE") {
                Controller.player.Position = Controller.level.Tracker.GetEntity<BlueTeamSpawnPoint>().Position;
                Controller.level.Session.RespawnPoint = Controller.level.Tracker.GetEntity<BlueTeamSpawnPoint>().Position;
            }
            Controller.level.Tracker.GetEntities<ResourceGenerator>().ForEach(e => ((ResourceGenerator) e).initialize());
            gameStartTime = DateTime.UtcNow;
            RedTeamHeartBroke = false;
            BlueTeamHeartBroke = false;
            InventoryController.initialize();
        }

        // public static void handleGameEnd(DataGameEnd data) {
        //     unhandledGameEnd = true;
        //     Controller.disableFeatures();
        //     Controller.level.Session.RespawnPoint = Controller.player.Position = Controller.level.Tracker.GetEntities<BlueTeamSpawnPoint>().First().Position;
        //     if (!Controller.player.Dead) {
        //         Controller.player.Die(Vector2.Zero);
        //         unhandledGameEnd = true;
        //     }
        // }

        public static void handleHeartBreak(DataHeartBreak data) {
            broadcast(data.player + " destoryed " + data.team.ToLower() + " team's heart!");
            if (data.team == "RED") {
                RedTeamHeartBroke = true;
                Controller.level.Tracker.GetEntity<RedTeamHeart>()?.RemoveSelf();
                if (Controller.featuresEnabled && PlayerInfo.PlayerTeam == "RED") {
                    Controller.level.Session.RespawnPoint =
                        Controller.level.Tracker.GetEntity<DefaultSpawnPoint>().Position;
                }
            } else if (data.team == "BLUE") {
                BlueTeamHeartBroke = true;
                Controller.level.Tracker.GetEntity<BlueTeamHeart>()?.RemoveSelf();
                if (Controller.featuresEnabled && PlayerInfo.PlayerTeam == "RED") {
                    Controller.level.Session.RespawnPoint =
                        Controller.level.Tracker.GetEntity<DefaultSpawnPoint>().Position;
                }
            }
        }

        public static void createMissile(string shooterName, string shooterTeam, Vector2 dir, Vector2 pos, float speed, float damage, float maxDis, int id) {
            Controller.level.Add(Engine.Pooler.Create<Missile>().Init(shooterName, shooterTeam, dir, pos, speed, damage, maxDis, id));
        }

        public static void createFireBall(string shooterName, string shooterTeam, Vector2 dir, Vector2 pos) {
            Controller.level.Add(Engine.Pooler.Create<Entities.Equipments.FireBall>().Init(shooterName, shooterTeam, dir, pos));
        }

        public static void createBadPearl(string shooterName, string shooterTeam, Vector2 dir, Vector2 pos) {
            Controller.level.Add(Engine.Pooler.Create<Entities.Equipments.BadPearl>().Init(shooterName, shooterTeam, dir, pos));
        }

        public static void onCreateMissile(Missile missile) {
            // GameController.missiles.Add(new KeyValuePair<string, int>(missile.shooterName, missile.id), missile);
        }

        public static void removeMissile(String shooterName, int id) {
            // GameController.missiles.Remove(new KeyValuePair<string, int>(shooterName, id));
        }

        public static void damage(float dmg, DataPlayerDie data, Vector2 src, bool bounce) {
            Audio.Play("event:/char/madeline/landing", Controller.player.Position);
            if (PlayerInfo.PlayerExtraHealth >= dmg) {
                PlayerInfo.PlayerExtraHealth -= dmg;
            } else {
                PlayerInfo.PlayerHealth -= dmg - PlayerInfo.PlayerExtraHealth;
                PlayerInfo.PlayerExtraHealth = 0;
            }
            PlayerInfo.PlayerHealth = Calc.Max(PlayerInfo.PlayerHealth, 0f);
            if (bounce) Controller.player.PointBounce(src);
            if (PlayerInfo.PlayerHealth < 1) {
                Audio.Play( (
                    (PlayerInfo.PlayerTeam == "RED" && GameController.RedTeamHeartBroke)
                    || (PlayerInfo.PlayerTeam == "BLUE" && GameController.BlueTeamHeartBroke)
                    ) ? "event:/new_content/char/madeline/death_golden"
                    : "event:/char/madeline/death", Controller.player.Position
                );
                PlayerInfo.PlayerMoney -= PlayerInfo.PlayerMoney / 2;
                PlayerInfo.PlayerHealth = 100f;
                PlayerInfo.PlayerExtraHealth = 0f;
                PlayerInfo.RegenerationEffect = 0f;
                PlayerInfo.DamageAbsorbEffect = 0f;
                PlayerInfo.BlindEffect = 0f;
                Controller.player.Position = Controller.level.Session.RespawnPoint ?? Controller.level.Tracker.GetEntity<DefaultSpawnPoint>().Position;
                Controller.player.Speed = new Vector2(0, 0);
                
                Controller.sendAndHandleData(data);
            }
        }
    }
}