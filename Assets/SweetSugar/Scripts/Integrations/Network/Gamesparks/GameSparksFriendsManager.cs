#if GAMESPARKS
using System;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using SweetSugar.Scripts.Core;
using UnityEngine;
#endif

#if GAMESPARKS

namespace SweetSugar.Scripts.Integrations.Network.Gamesparks
{
    public class GameSparksFriendsManager : IFriendsManager
    {

        #region IFriendsManager implementation

        public void GetFriends(Action<Dictionary<string, string>> Callback)
        {
            new SocialLeaderboardDataRequest().SetLeaderboardShortCode("Level").SetDontErrorOnNotSocial(true).SetEntryCount(100).Send(response =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Found friends Data...");
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        var FBidArray = entry.ExternalIds.BaseData;
                        foreach (var item2 in FBidArray)
                        {
                            // Debug.Log(item2);
                            dic.Add(item2.Value.ToString(), entry.UserId);
                        }

                    }
                    Callback(dic);
                }
                else
                {
                    Debug.Log("Error Retrieving friends Data...");
                }
            });

        }

        public void PlaceFriendsPositionsOnMap(Action<Dictionary<string, int>> Callback)
        {
            Debug.Log("place friends");
            new SocialLeaderboardDataRequest().SetLeaderboardShortCode("Level").SetDontErrorOnNotSocial(true).SetEntryCount(100).Send(response =>
            {
                if (!response.HasErrors)
                {
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    foreach (LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        // Debug.Log(entry.UserId + " " + entry.JSONData["Level"]);
                        dic.Add(entry.UserId, int.Parse(entry.JSONData["Level"].ToString()));

                    }
                    Callback(dic);

                }
                else
                {
                    Debug.Log("Error Retrieving friends Data...");
                }
            });

        }

        public void GetLeadboardOnLevel(int LevelNumber, Action<List<LeadboardPlayerData>> Callback)
        {
            new SocialLeaderboardDataRequest().SetDontErrorOnNotSocial(true).SetLeaderboardShortCode("LB.Level." + LevelManager.THIS.currentLevel).SetEntryCount(3).Send(response =>
            {
                if (!response.HasErrors)
                {
                    List<LeadboardPlayerData> list = new List<LeadboardPlayerData>();
                    foreach (LeaderboardDataResponse._LeaderboardData item in response.Data)
                    {
                        LeadboardPlayerData pl = new LeadboardPlayerData();
                        pl.Name = item.UserName;
                        pl.userID = item.UserId;
                        pl.position = int.Parse(item.Rank.ToString());
                        pl.score = int.Parse(item.JSONData["Score"].ToString());

                        list.Add(pl);
                        Debug.Log(item.UserName + " " + item.UserId + " " + item.Rank + " " + item.JSONData["Score"]);

                    }
                    Callback(list);

                }
                else
                {
                    Debug.Log("Error Retrieving leadboard Data...");
                }
            });

        }

        public void Logout()
        {

        }


        #endregion



    }
}
#endif