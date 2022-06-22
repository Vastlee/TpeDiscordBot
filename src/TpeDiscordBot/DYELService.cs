using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace TpeDiscordBot;

class DYELService {
    const ulong DYELChannelId = 842617395440517161;
    const int HourToAnnounceRoutine = 6;

    readonly DateTime DYELStartTime = new(2021, 11, 3);

    DiscordClient client;
    DiscordEmoji muscleEmoji;

    public static DateTime NextTopicUpdateDate { get; private set; } = DateTime.Now;
    public static DateTime NextChannelAnnouncementDate { get; private set; } = DateTime.Now;

    public DYELService(DiscordClient client) {
        this.client = client;
        muscleEmoji = DiscordEmoji.FromName(client, ":muscle:");
        Task monitorTask = StartMonitoringAsync();
    }

    public async Task StartMonitoringAsync() {
        while(true) {
            if(IsTimeToChangeTopic()) {
                NextTopicUpdateDate = DateTime.Now.AddDays(1d);
                UpdateChannelTopicWithRoutine();
            }

            if(IsTimeToAnnounce()) {
                NextChannelAnnouncementDate = DateTime.Now.AddDays(1d);
                UpdateChannelTopicWithRoutine();
            }

            await Task.Delay(TimeSpan.FromHours(1d));
        }

        static bool IsTimeToChangeTopic() => NextTopicUpdateDate.Day == DateTime.Now.Day;
        static bool IsTimeToAnnounce() {
            return (NextChannelAnnouncementDate.Day == DateTime.Now.Day
                    && DateTime.Now.Hour == HourToAnnounceRoutine);
        }
    }

    void AnnounceRoutineInChannel() {
        client
            .GetChannelAsync(DYELChannelId)
            .Result
            .SendMessageAsync(RoutineMessage())
            .ConfigureAwait(false);
    }

    void UpdateChannelTopicWithRoutine() {
        Action<ChannelEditModel> changeTopic;

        changeTopic = new(x => x.Topic = RoutineMessage());

        client
            .GetChannelAsync(DYELChannelId)
            .Result
            .ModifyAsync(changeTopic)
            .ConfigureAwait(false);
    }

    string RoutineMessage() {
        int sinceStart = DateTime.Now.Subtract(DYELStartTime).Days % 6;
        string liftDay = sinceStart switch {
            0 => "First Push Day!",
            1 => "First Pull Day!",
            2 => "First Hybrid Day!",
            3 => "Second Push Day!",
            4 => "Second Pull Day!",
            5 => "Second Hybrid Day!",
            _ => "... something broke. I have no idea what day it is!"
        };

        string topic = $"Today's Routine: [ **{liftDay}** ]! Put In That Work!!! {muscleEmoji} {muscleEmoji}";
        return topic;
    }
}
