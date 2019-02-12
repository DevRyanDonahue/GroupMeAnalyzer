using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GroupMeAnalysis.Models;
using GroupMeAnalysis.Util;

namespace GroupMeAnalysis {
    class Program {
        private static List<Conversation> totalConversations_ = new List<Conversation>();
        private static List<Message> totalMessages_ = new List<Message>();
        private static List<User> totalMembers_ = new List<User>();

        static void Main(string[] args) {
            Console.WriteLine("Enter path to unzipped folder of GroupMe conversation folders:");
            var path = Console.ReadLine();
            if (!Directory.Exists(path)) {
                Console.WriteLine("That directory does not exist. Exiting.");
                Console.WriteLine("Exiting.");
                Console.ReadLine();
                return;
            }
            var directories = Directory.GetDirectories(path);
            if (!directories.Any(dir => File.Exists(dir + "\\conversation.json") && File.Exists(dir + "\\message.json"))) {
                Console.WriteLine("None of the child directories contain both conversation.json and message.json");
                Console.WriteLine("Exiting.");
                Console.ReadLine();
                return;
            }

            foreach (var directory in directories) {
                var filepathToFolder = $@"{directory}";
                ParseConversation(filepathToFolder);
            }

            AnalyzeStatsPerUser();

            PrintTopMessages();

            Console.WriteLine("========GENERAL STATS======");
            Console.WriteLine($"Total Messages: { totalMessages_.Count }");
            var firstMessage = totalMessages_.OrderBy(mess => mess.created_at).ToArray()[1];//the 0th item is system announcing groupme chat
            Console.WriteLine($"First Message: {firstMessage.name} : { firstMessage.text }");

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }

        private static void PrintTopMessages() {
            Console.WriteLine("========TOP MESSAGES======");
            var mostLikesOnSingleMessage = totalMessages_.Max(x => x.favorited_by.Count);
            var mostLikedMessages = totalMessages_.Where(x => x.favorited_by.Count + 1 >= mostLikesOnSingleMessage);//within 1 like from top liked
            foreach (var mostLikedMessage in mostLikedMessages) {
                var whoLikedTheMessage = mostLikedMessage.favorited_by.Count > 0
                        ? $"({String.Join(",", totalMembers_.Where(x => mostLikedMessage.favorited_by.Contains(x.user_id)).Select(x => x.name))})"
                        : "";
                Console.WriteLine($"{FormatMessageSummary(mostLikedMessage)} has {mostLikedMessage.favorited_by.Count} likes. {whoLikedTheMessage}");
                Console.WriteLine();
            }
        }

        private static void AnalyzeStatsPerUser() {
            Console.WriteLine("========USER STATS======");
            totalMembers_ = totalConversations_.SelectMany(x => x.members).DistinctBy(x => x.user_id).ToList();

            foreach (var user in totalMembers_.OrderBy(x => x.name)) {
                var totalMessages = totalMessages_.Count(x => x.user_id == user.user_id);
                if (totalMessages.Equals(0)) continue;
                var totalLikes = totalMessages_.Where(x => x.user_id == user.user_id)
                    .Sum(x => x.favorited_by.Count);
                Console.WriteLine(
                    $"{user.name} {totalMessages} messages., {totalLikes} likes. {(double)totalLikes / totalMessages} likes per message");
            }
        }

        private static string FormatMessageSummary(Message mostLikedMessage) {
            var group = totalConversations_.FirstOrDefault(x => x.group_id == mostLikedMessage.group_id);
            return $"{mostLikedMessage.name} : {mostLikedMessage.text} - {DateTimeUtil.UnixTimeStampToDateTime(mostLikedMessage.created_at) } { @group.name}";
        }

        private static void ParseConversation(string filepathToFolder) {
            var conversationJsonPath = filepathToFolder + "\\conversation.json";
            var messagesJsonPath = filepathToFolder + "\\message.json";
            if (!(File.Exists(messagesJsonPath))) {
                Console.WriteLine($"Skipping {filepathToFolder} - no messages exist");
                return;
            }
            var conversations = JsonUtil.LoadJson<Conversation>(conversationJsonPath);
            //foreach (var user in conversations.members) {
            //    Console.WriteLine($"{user.name} - {user.nickname}");
            //}

            var messages = JsonUtil.LoadJson<Message[]>(messagesJsonPath);
            //foreach (var message in messages)
            //{
            //    var likedByList = message.favorited_by.Count > 0
            //        ? $"({String.Join(",", conversations.members.Where(x => message.favorited_by.Contains(x.user_id)).Select(x => x.name))})"
            //        : "";
            //    Console.WriteLine($"{message.name}: '{message.text}' has {message.favorited_by.Count} likes. {likedByList}");
            //}

            totalConversations_.Add(conversations);
            totalMessages_.AddRange(messages);
        }
    }
}

