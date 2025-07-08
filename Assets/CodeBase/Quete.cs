[System.Serializable]
public class AnilistTitle
{
    public string native;
    public string romaji;
    public string english;
}

[System.Serializable]
public class Anilist
{
    public int id;
    public int idMal;
    public AnilistTitle title;
    public string[] synonyms;
    public bool isAdult;
}

[System.Serializable]
public class ResultItem
{
    public Anilist anilist;
    public string filename;
    public string episode;
    public float from;
    public float to;
    public float similarity;
    public string video;
    public string image;
}

[System.Serializable]
public class TraceMoeResponse
{
    public int frameCount;
    public string error;
    public ResultItem[] result;
}