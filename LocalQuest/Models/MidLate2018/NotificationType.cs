using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest.Models.MidLate2018
{
    public enum NotificationType
    {
        RelationshipChanged = 1,
        MessageReceived,
        MessageDeleted,
        PresenceHeartbeatResponse,
        SubscriptionListUpdated = 9,
        SubscriptionUpdateProfile = 11,
        SubscriptionUpdatePresence,
        SubscriptionUpdateGameSession,
        SubscriptionUpdateRoom = 15,
        ModerationQuitGame = 20,
        ModerationUpdateRequired,
        ModerationKick,
        ModerationKickAttemptFailed,
        ServerMaintenance = 25,
        GiftPackageReceived = 30,
        ProfileJuniorStatusUpdate = 40,
        RelationshipsInvalid = 50,
        StorefrontBalanceAdd = 60,
        ConsumableMappingAdded = 70,
        ConsumableMappingRemoved,
        PlayerEventCreated = 80,
        PlayerEventUpdated,
        PlayerEventDeleted,
        PlayerEventResponseChanged,
        PlayerEventResponseDeleted,
        ChatMessageReceived = 90,
        SelfAccountUpdate = 100,
        PlayerProgressionLevelUpdate = 101,
        ReputationUpdate = 102
    }
}
