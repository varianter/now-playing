using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Common;
using Functions.Repositories;
using Functions.Models.Orchestrator;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Functions
{
    public static class HttpSlack
    {
        [FunctionName(FunctionNames.HttpSlack)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "slack")] HttpRequest req,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            ILogger log)
        {
            await userTable.CreateIfNotExistsAsync();

            var bodyString = await req.ReadAsStringAsync();
            var slackPayload = HttpUtility.ParseQueryString(bodyString);
            var token = slackPayload["token"];
            if (!string.Equals(token, Config.SlackToken))
            {
                throw new System.Exception("Not allowed");
            }

            var args = slackPayload["text"]?.Trim();

            if (string.Equals("list", args))
            {
                var repo = new UserRepository(userTable);
                var entities = await repo.GetUsersAsync();
                var nonHiddenUserIds = entities
                        .Where(e => e.Active)
                        .Select(e => e.RowKey)
                        .ToList();

                log.LogInformation("Found these users: {Users}", string.Join(", ", nonHiddenUserIds));

                var listenerTracks = await SpotifyHelper.GetListenerTracks(nonHiddenUserIds, log);

                return new JsonResult(new
                {
                    blocks = new
                    {
                        type = "section",
                        block_id = "listeners",
                        fields = listenerTracks.Select(lt => new { type = "mrkdwn", text = $"- {lt.userId}: {lt.currentTrack}" }).ToArray<object>()
                    }
                });
            }

            return new JsonResult(new
            {
                blocks = new object[] {
                    new {
                            type = "section",
                            block_id = "help_text_commands",
                            fields = new [] {
                                new {
                                    type = "mrkdwn",
                                    text = "*Hey! Welcome to now playing* \n Press the button below to go to the website, or register with Now Playing:"
                                }
                            }
                    },
                    new { type = "actions", elements = new object[] {
                        new {
                            type = "button",
                            action_id = "dashboard_nowplaying",
                            url = Config.HostName,
                            text = new {
                                type = "plain_text",
                                text = "Dashboard website",
                            }
                        },
                        new {
                            type = "button",
                            action_id = "register_nowplaying",
                            url = $"{Config.HostName}/api/authorize",
                            text = new {
                                type = "plain_text",
                                text = "Register with Now Playing",
                            }
                        }
                    }}
                }
            });
        }
    }
}
