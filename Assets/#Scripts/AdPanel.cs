using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AdPanel : MonoBehaviour
{
    #pragma warning disable
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int rows;
    [SerializeField] private int cols;

    [SerializeField] private GameObject markerTopLeft;
    [SerializeField] private GameObject markerBottomLeft;
    [SerializeField] private GameObject markerTopRight;
    [SerializeField] private GameObject markerBottomRight;

    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject buttonPrev;

    [SerializeField] private Text infoText;
    [SerializeField] private Text countText;
    #pragma warning enable

    private int pageIndexCurrent;
    private int pageIndexMax;

    private float spanHorizontal;

    private float spanVertical;
    private float spacingHorizontal;
    private float spacingVertical;

    private List<GameObject> pageCubes = new List<GameObject>();

    private List<JobAdClasses.Hit> allAds = new List<JobAdClasses.Hit>();
    private string occupationGroupID;
    private string occupationGroupTitle;
    private bool canInteract = false;

    void Start()
    {
        spanHorizontal = Vector3.Distance(markerTopLeft.transform.localPosition, markerTopRight.transform.localPosition);
        spanVertical = Vector3.Distance(markerTopLeft.transform.localPosition, markerBottomLeft.transform.localPosition);
        spacingHorizontal = spanHorizontal / (cols - 1);
        spacingVertical = spanVertical / (rows - 1);

        markerTopLeft.SetActive(false);
        markerBottomLeft.SetActive(false);
        markerTopRight.SetActive(false);
        markerBottomRight.SetActive(false);

        infoText.text = "";

        ShowNavigation(false);
    }

    private void ShowNavigation(bool show){
        buttonPrev.SetActive(show);
        buttonNext.SetActive(show);
        countText.gameObject.SetActive(show);
    }

    public void UpdateButtonVisibility() {
        buttonPrev.SetActive(pageIndexCurrent > 0);
        buttonNext.SetActive(pageIndexCurrent < pageIndexMax);
    }

    public void Show() {
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }

    public void SetPosition(float x, float y, float z) {
        this.transform.localPosition = new Vector3(x, y, z);
    }

    public void SetScale(float scale) {
        this.transform.localScale = Vector3.one * scale;
    }

    private int GetMaxIndex(int total, int page) {
        var index = 0;
        while (total > page) {
            total -= page;
            index++;
        }
        return index;
    }

    public async Task LoadAdsForOccupationGroup(string id) {
        ShowNavigation(false);
        occupationGroupID = id;
        occupationGroupTitle = SkillsToAdsManager.Instance.GetOccupationTitle(id);
        canInteract = false;
        ClearCubes();
        infoText.text = "LOADING";
        allAds = await SkillsToAdsManager.Instance.GetJobAdsForOccupationGroup(occupationGroupID);
        
        if(allAds.Count == 0) {
            infoText.text = "Currently no ads listed for this profession." + Environment.NewLine + "Try again soon!";
            canInteract = true;
            return;
        }
        await Task.Delay(1000);
        pageIndexCurrent = 0;
        pageIndexMax = GetMaxIndex(allAds.Count, rows * cols);
        ShowPage();
        canInteract = true;
    }

    private void ClearCubes() {
        foreach (var cube in pageCubes) {
            Destroy(cube);
        }
        pageCubes.Clear();
    }

    public async void RunTestSequence() {
        await Task.Delay(1000);
        PageBack();
        for (int i = 0; i < 12; i++) {
            await Task.Delay(500);
            PageForward();
        }
        for (int i = 0; i < 12; i++) {
            await Task.Delay(500);
            PageBack();
        }
    }

    public void PageForward() {
        if(!canInteract){
            return;
        }
        if (pageIndexCurrent < pageIndexMax) {
            canInteract = false;
            pageIndexCurrent++;
            AnimateOut(pageCubes, 1);
            ClearCubes();
            ShowPage();
            AnimateIn(pageCubes, -1);
        }
    }
    public void PageBack() {
        if (!canInteract) {
            return;
        }
        if (pageIndexCurrent > 0) {
            canInteract = false;
            pageIndexCurrent--;
            AnimateOut(pageCubes, -1);
            ClearCubes();
            ShowPage();
            AnimateIn(pageCubes, 1);
        }
    }

    private void ShowPage() {
        var firstIndex = pageIndexCurrent * rows * cols;
        var lastIndex = Mathf.Min(firstIndex + (rows * cols), allAds.Count);
        var currentIndex = firstIndex;
        infoText.text = occupationGroupTitle.ToUpper();
        countText.text = (firstIndex + 1) + "/" + lastIndex + " of " + allAds.Count;
        for (int c = 0; c < cols; c++) {
            for (int r = 0; r < rows; r++) {
                if(currentIndex < lastIndex) {
                    var pos = markerBottomLeft.transform.localPosition;
                    pos.y += r * spacingVertical;
                    pos.x += c * spacingHorizontal;
                    var cube = Instantiate(cubePrefab, this.transform);
                    cube.GetComponent<AdCube>().JobAd = allAds[currentIndex];
                    cube.transform.localPosition = pos;
                    cube.transform.localScale = Vector3.one * 1f;
                    cube.SetActive(true);
                    pageCubes.Add(cube);
                    currentIndex++;
                }
            }
        }
         ShowNavigation(allAds.Count > rows * cols);
    }

    private void AnimateOut(List<GameObject> cubes, int direction) {
        var distance = spanHorizontal;
        foreach (var cube in cubes) {
            var z = cube.transform.localPosition.x;
            cube.transform.DOScale(0f, 0.3f)
                .SetDelay(0.1f);
            cube.transform.DOLocalMoveX(z - distance * direction, 0.4f)
                .OnComplete(() => Destroy(cube));
        }
    }

    private void AnimateIn(List<GameObject> cubes, int direction){
        var distance = spanHorizontal;
        foreach (var cube in cubes) {
            var pos = cube.transform.localPosition;
            var originalX = pos.x;
            pos.x = originalX - distance * direction;
            cube.transform.localPosition = pos;
            cube.transform.DOLocalMoveX(originalX, 0.4f)
                .OnComplete(() => {
                    canInteract = true;
                    cube.GetComponent<AdCube>()?.UpdateCubeStartPosition();
                });
        }
    }


    public void OnButtonNext()
    {
        PageForward();
    }

    public void OnButtonPrev()
    {
        PageBack();
    }
}