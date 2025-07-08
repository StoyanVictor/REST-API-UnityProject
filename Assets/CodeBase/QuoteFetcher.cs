using System.Collections;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;
public class QuoteFetcher : MonoBehaviour
{
    [SerializeField] private RawImage imageDisplay;
    [SerializeField] private string frameUrl;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI animeName;
    [SerializeField] private Image profileImage;
   private void Start()
   {
       StartCoroutine(LoadImageFromURL());
   }
   private IEnumerator LoadImageFromURL()
   {
       string animeApi = $"https://api.trace.moe/search?anilistInfo&url={frameUrl}";
       UnityWebRequest webRequest = UnityWebRequest.Get(animeApi);
       var operation = webRequest.SendWebRequest();
       yield return operation;
       if (operation.isDone)
       {
           string path = Application.persistentDataPath + "/api_response.txt";
           var alltext = webRequest.downloadHandler.text;
           File.WriteAllText(path,alltext);
           Debug.Log("Файл сохранён по пути: " + path);
           var json = operation.webRequest.downloadHandler.text;
           var anime = JsonUtility.FromJson<TraceMoeResponse>(json);
           var currentAnimeTitleEng = anime.result[0].anilist.title.english;
           animeName.text = currentAnimeTitleEng;
           print(currentAnimeTitleEng);
           StartCoroutine(DownloadVideo(anime.result[0].video));
       }
   }
   IEnumerator DownloadImage(string MediaUrl)
   {
       UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
       yield return request.SendWebRequest();
       if(request.isNetworkError || request.isHttpError) 
           Debug.Log(request.error);
       else
           imageDisplay.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
   } 
   public IEnumerator DownloadVideo(string URLVideo)
   {
       UnityWebRequest wwwVideo = UnityWebRequest.Get(URLVideo);
       yield return wwwVideo.SendWebRequest();
       if (wwwVideo.isNetworkError || wwwVideo.isHttpError)
       {
           Debug.Log(wwwVideo.error);
       }
       else
       {
           File.WriteAllBytes(Application.persistentDataPath + "Video"+ ".avi" , wwwVideo.downloadHandler.data);
           var input  = Application.persistentDataPath + "Video" + ".avi";
           var outPut  = Application.persistentDataPath + "VideoFinal" + ".avi";
           StartCoroutine(RunFFmpeg(input, outPut));
           print(URLVideo);

       }   
   }
   private IEnumerator RunFFmpeg(string inputPath, string outputPath)
   {
       ProcessStartInfo startInfo = new ProcessStartInfo
       {
           FileName = "ffmpeg",
           Arguments = $"-y -i \"{inputPath}\" -c:v libx264 -c:a aac \"{outputPath}\"",
           UseShellExecute = false,
           RedirectStandardOutput = true,
           RedirectStandardError = true,
           CreateNoWindow = true
       };

       Process process = new Process { StartInfo = startInfo };
       process.Start();

       string stderr = process.StandardError.ReadToEnd(); // для отладки
       process.WaitForExit();

       if (process.ExitCode != 0)
           Debug.LogError("FFmpeg ошибка:\n" + stderr);
       else
       {
           Debug.Log("FFmpeg завершил перекодировку успешно");
            
           videoPlayer.source = VideoSource.Url;
           videoPlayer.url = "file:///" + outputPath.Replace("\\", "/");
           videoPlayer.Prepare();
       }


       yield return null;
   }
   
}

