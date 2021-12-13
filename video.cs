using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Video;
public class video : MonoBehaviour, ICloudRecoEventHandler
{
    private CloudRecoBehaviour cloudReco;
    public ImageTargetBehaviour behaviour;
    public ImageTargetBehaviour mbehaviour;
    public GameObject MainPlayer;

    private bool mIsScanning = false;
    public int version;
    public string URL;
    public string AssetName;
    public string lastTarget;
    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        cloudReco = GetComponent<CloudRecoBehaviour>();

        if (cloudReco)
        {
            cloudReco.RegisterEventHandler(this);
        }

        MainPlayer = GameObject.Find("Player");

        //Hide(MainPlayer);
    }
    /*void Hide(GameObject obj)
    {
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        Collider[] cols = obj.GetComponentsInChildren<Collider>();

        foreach (var item in rends)
        {
            item.enabled = false;
        }
        foreach (var item in cols)
            item.enabled = false;
        if (obj.GetComponent<VideoPlayer>() != null)
        { obj.GetComponent<VideoPlayer>().Stop(); }


    }*/

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
    }
    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }
    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;
        if (scanning)
        {
            // clear all known trackables
            var tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);
        }

    }
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        URL = cloudRecoSearchResult.MetaData;


        string pilih = URL.Substring(URL.Length - 3);       
        Debug.Log(pilih);
        Debug.Log(targetSearchResult.UniqueTargetId);


        /* if (lastTarget != targetSearchResult.UniqueTargetId)
           {        Destroy(GameObject.Find("ImageTarget(clone)"));

             Debug.Log("Disabling the  Clour Reco");
               cloudReco.CloudRecoEnabled = false;
               StartCoroutine(WaitOneFrame());
               cloudReco.CloudRecoEnabled = true;
               Debug.Log("Cloud Reco enabled");
           }
           else
           {
               Debug.Log("Same Targets");
           }*/

        if (pilih == "mp4")
        {            
            GameObject newImageTarget = Instantiate(behaviour.gameObject) as GameObject;
            MainPlayer = newImageTarget.transform.GetChild(0).gameObject;
            GameObject augmentation = null;
            if (augmentation != null)
             {
                 augmentation.transform.SetParent(newImageTarget.transform);
             }
            if (behaviour)
             {
                ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
                ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);
             }
            MainPlayer.GetComponent<VideoPlayer>().url = URL.Trim();
         }
        else
        {
            StartCoroutine(DownloadAndCache());
           // cloudReco.CloudRecoEnabled = false;
            if (mbehaviour)
            {
                ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
                ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, mbehaviour.gameObject);
            }
        }
    }
    IEnumerator WaitOneFrame()
    {
        yield return 0;
    }
    IEnumerator DownloadAndCache()
    {
        while (!Caching.ready)
            yield return null;

        var www = WWW.LoadFromCacheOrDownload(URL, version);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield return www;
        }

        AssetBundle bundle = www.assetBundle;
        if(AssetName == "")
        {
            GameObject instantiated = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[0])) as GameObject;
            GameObject cloudRecoObject = GameObject.Find("logo");
            instantiated.transform.parent = cloudRecoObject.transform;
        }
        bundle.Unload(false);
    }
}


