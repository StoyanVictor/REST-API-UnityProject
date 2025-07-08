using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
namespace CodeBase
{
    public class AnimeSearcher : MonoBehaviour
    {
        [SerializeField] private string _frameUrl;
        [SerializeField] private TextMeshProUGUI _animeName;
        private void Start()
        {
            StartCoroutine(AnimeApiRequesting());
        }
        private IEnumerator AnimeApiRequesting()
        {
            string url = $"https://api.trace.moe/search?anilistInfo&url={_frameUrl}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            var requestAsyncOperation = request.SendWebRequest();
            yield return  requestAsyncOperation;
                            
            if (request.isDone)
            {
                string data = request.downloadHandler.text;
                var clas = JsonUtility.FromJson<TraceMoeResponse>(data);
                _animeName.text = clas.result[0].anilist.title.english;
            }
            else
            {
                print($"im sad :(!");
            }
           
        }
    }
}