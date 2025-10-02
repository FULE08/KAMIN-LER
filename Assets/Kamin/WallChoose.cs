using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class WallChoose : MonoBehaviour
{
    public static WallChoose Instance;
    public GameObject Ui;
    public GameObject ConfirmUi;
    public Button Button;
    public Dropdown Dropdown;
    public GameObject MaxPrefab;
    public GameObject SwitchPrefab;
    public GameObject DisguisePrefab;
    public GameObject LaserPrefab;
    private Wall CurrentWall;
    private int Option;

    void Awake()
    {
        Instance = this;
        Ui.SetActive(false);
        ConfirmUi.SetActive(false);
    }
    void Start()
    {
        Button.onClick.AddListener(OnClicked);
    }
    private void ShowUi()
    {
        Ui.SetActive(true);
        ConfirmUi.SetActive(true);
    }
    private void DisableUi()
    {
        Ui.SetActive(false);
        ConfirmUi.SetActive(false);
    }
    public void ShowUpgrade(Wall wall)
    {
        CurrentWall = wall;
        ShowUi();
    }
    private void OnClicked()
    {
        Option = Dropdown.value;
        OnConfirmed(Option);
    }
    private void OnConfirmed(int WallType)
    {
        switch (WallType)
        {
            case 0:
                Choosed(MaxPrefab);
                break;
            case 1:
                Choosed(SwitchPrefab);
                break;
            case 2:
                Choosed(DisguisePrefab);
                break;
            case 3:
                Choosed(LaserPrefab);
                break;
        }
    }
    private void Choosed(GameObject Choice)
    {
        Vector3 pos = CurrentWall.transform.position;
        Quaternion rot = CurrentWall.transform.rotation;
        Destroy(CurrentWall.gameObject);
        Instantiate(Choice, pos, rot);
        DisableUi();
    }
}
