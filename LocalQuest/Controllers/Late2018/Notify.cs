using LocalQuest.Models.Late2018;
using LocalQuest.Models.Mid2018;
using LocalQuest.Models.MidLate2018;
using QuerryNetworking.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalQuest.Controllers.Late2018
{
    public static class Notify
    {
        static WebSocket? Current;
        public static async void ConnectNotify(WebSocketContext Context)
        {
            Log.Debug("connecting...");
            Current = Context.WebSocket;

            while (Current.State == WebSocketState.Open)
            {
                byte[] Buffer = new byte[1024];
                try
                {
                    Log.Debug("data...");
                    WebSocketReceiveResult Received = await Current.ReceiveAsync(Buffer, CancellationToken.None);
                    Log.Debug("got data!");
                    if (Received.MessageType == WebSocketMessageType.Close)
                    {
                        await Current.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        return;
                    }
                    string NotifyString = Encoding.UTF8.GetString(Buffer);
                    Log.Debug(NotifyString);
                    if(NotifyString.Contains("protocol"))
                    {
                        // SignalR Core handshake response: {}<RS>
                        await Current.SendAsync(Encoding.UTF8.GetBytes("{}\x1e"), WebSocketMessageType.Text, true, CancellationToken.None);

                        long AccountId = long.Parse(LocalQuest.Config.GetString("AccountId"));

                        await SendNotification(new Notification()
                        {
                            Id = "SelfAccountUpdate",
                            Msg = new
                            {
                                email = "player@localquest.local",
                                phone = (string?)null,
                                juniorState = 0,
                                parentAccountId = (long?)null,
                                availableUsernameChanges = 1,
                                birthday = "2000-01-01T00:00:00.000Z",
                                accountId = AccountId,
                                profileImage = LocalQuest.Config.GetString("PFP"),
                                isJunior = false,
                                platforms = -1,
                                username = LocalQuest.Config.GetString("Username"),
                                displayName = LocalQuest.Config.GetString("DisplayName"),
                                createdAt = DateTime.UtcNow.ToString("o"),
                                personalPronouns = 0,
                                identityFlags = 0,
                                bannerImage = (string?)null
                            }
                        });

                        await SendNotification(new Notification()
                        {
                            Id = "AccountUpdate",
                            Msg = new Profile()
                        });

                        await SendNotification(new Notification()
                        {
                            Id = "PlayerProgressionLevelUpdate",
                            Msg = new { PlayerId = AccountId, Level = 1, XP = 0 }
                        });

                        await SendNotification(new Notification()
                        {
                            Id = "ReputationUpdate",
                            Msg = new
                            {
                                accountId = AccountId,
                                Noteriety = 0.0,
                                IsCheerful = true,
                                CheerCredit = 20,
                                CheerGeneral = 0,
                                CheerHelpful = 0,
                                CheerGreatHost = 0,
                                CheerSportsman = 0,
                                CheerCreative = 0
                            }
                        });

                        await SendNotification(new Notification()
                        {
                            Id = "PresenceUpdate",
                            Msg = new HeartbeatResponse()
                            {
                                Error = "",
                                Presence = new Presence()
                                {
                                    PlayerId = AccountId,
                                    IsOnline = true,
                                    PlayerType = PlayerType.SCREEN,
                                    GameSession = null
                                }
                            }
                        });
                    }
                    else if (NotifyString.Contains("heartbeat2"))
                    {
                        Log.Info("Requested heartbeat");
                        if (CurrentPresence != null)
                        {
                            await SendNotification(new Notification()
                            {
                                Id = Models.MidLate2018.NotificationType.PresenceHeartbeatResponse,
                                Msg = CurrentPresence
                            });
                            await SendNotification(new Notification()
                            {
                                Id = Models.MidLate2018.NotificationType.SubscriptionUpdateRoom,
                                Msg = new RoomDetails(RoomManager.AllRooms.FirstOrDefault(A => A.RoomId == CurrentPresence.GameSession.RoomId))
                            });
                        }
                    }
                    else if (NotifyString.Contains("playerSubscriptions/v1/update"))
                    {
                        Log.Info("Requested update subscriptions");
                        if (CurrentPresence != null)
                        {
                            await SendNotification(new Notification()
                            {
                                Id = Models.MidLate2018.NotificationType.SubscriptionUpdatePresence,
                                Msg = CurrentPresence
                            });
                        }
                    }
                    else if(!NotifyString.Contains("api"))
                    {
                        await Current.SendAsync(Buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch
                {
                    Log.Warn("Websocket failure?!");
                    await Current.CloseAsync(WebSocketCloseStatus.InternalServerError, "", CancellationToken.None);
                    return;
                }
            }
        }

        public static Models.MidLate2018.Presence? CurrentPresence;

        public static async Task SendNotification(Notification Message)
        {
            if(Current == null)
            {
                return;
            }
            string MessageString = JsonSerializer.Serialize(Message);

            NotifyMessage M = new NotifyMessage()
            {
                error = "",
                arguments = new object[]
                {
                    MessageString,
                },
                type = MessageTypes.Invocation
            };

            await Current.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(M) + ""), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task SendNotification(NotifyMessage M)
        {
            if (Current == null)
            {
                return;
            }

            await Current.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(M) + ""), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
