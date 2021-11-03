using System;

namespace TPEBot {
    class Program {
        static void Main(string[] args) {
            var discordBot = new Bot();
            discordBot.RunAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}
