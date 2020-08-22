using System.Collections.Generic;
using UnityEngine;
#if FACEBOOK
using Facebook.Unity;

namespace SweetSugar.Scripts.Integrations
{
    public class FBTest : MonoBehaviour
    {

        public string obj = "object";
        public string objType = "object";
        public string objRequest = "object";
        public string readRequest = "me/objects/object?fields=description";
        public string saveObjectLine = "{\"fb:app_id\":\"1040909022611487\",\"og:type\":\"object\",\"og:title\":\"level scores\",\"og:description\":\"111\"}";

        void OnGUI()
        {
            if (GUILayout.Button("Save"))
            {
                SaveScores();
            }
            if (GUILayout.Button("Read"))
            {
                ReadScores();
            }
            if (GUILayout.Button("Delete"))
            {
                DeleteScores();
            }

        }

        public void SaveScores()
        {
            int score = 10000;

            var scoreData =
                new Dictionary<string, string> { { "score", score.ToString() } };

            string value = "1000";
            FB.API("/me/scores", HttpMethod.POST, APICallBack, scoreData);
        }

        public void ReadScores()
        {

            FB.API(readRequest, HttpMethod.GET, RequestCallback);
        }

        private void RequestCallback(IGraphResult result)
        {
            if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.Log(result.RawResult);

                var resultDictionary = result.ResultDictionary;
                if (resultDictionary.ContainsKey("data"))
                {
                    var dataArray = (List<object>)resultDictionary["data"];

                    if (dataArray.Count > 0)
                    {
                        for (int i = 0; i < dataArray.Count; i++)
                        {
                            var firstGroup = (Dictionary<string, object>)dataArray[i];
                            foreach (KeyValuePair<string, object> entry in firstGroup)
                            {
                                print(entry.Key + " " + entry.Value);
                            }
                            //print(firstGroup["id"] + " " + firstGroup["title"]);
                        }
                        //this.gamerGroupCurrentGroup = (string)firstGroup["id"];
                    }
                }
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log(result.Error);

            }
        }

        void DeleteScores()
        {
            FB.API(objRequest, HttpMethod.GET, DeleteCallback);

        }

        private void DeleteCallback(IGraphResult result)
        {
            if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.Log(result.RawResult);

                var resultDictionary = result.ResultDictionary;
                if (resultDictionary.ContainsKey("data"))
                {
                    var dataArray = (List<object>)resultDictionary["data"];

                    if (dataArray.Count > 0)
                    {
                        for (int i = 0; i < dataArray.Count; i++)
                        {
                            var firstGroup = (Dictionary<string, object>)dataArray[i];
                            FB.API((string)firstGroup["id"], HttpMethod.DELETE, APICallBack);
                        }
                        //this.gamerGroupCurrentGroup = (string)firstGroup["id"];
                    }
                }
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log(result.Error);

            }
        }

        public void APICallBack(IGraphResult result)
        {
            Debug.Log(result);
        }

    }
}
#endif
