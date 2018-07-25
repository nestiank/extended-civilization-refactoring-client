using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;

public class UIManager : MonoBehaviour
{

    // Get UI GameObjects from the Scene
    public GameObject mapUI;
    public GameObject managementUI;
    public GameObject questUI;
    public GameObject selectedActor;
    public GameObject SpecialSpec;

    public GameObject UnitSelTab;
    public GameObject BuildingSelTab;
    public GameObject EpicTab, HighTab, IntermediateTab, LowTab;    // Unit production
    public GameObject CityTab, CityBuildingTab, NormalBuildingTab;  // Building production

    GameObject skillSet;
    GameObject unitInfo;

    GameObject Actions;
    GameObject moveBtn;
    GameObject attackBtn;
    GameObject skipBtn;
    GameObject skillBtn;

    public Image UnitPortrait;

    // RayCast For Selection
    public Ray ray;
    public RaycastHit hit;

    // UIManager Class Instance Singleton
    private static UIManager _manager = null;
    public static UIManager Instance { get { return _manager; } }

    private void Awake()
    {
        if (_manager != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _manager = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        mapUI = GameObject.Find("MapUI");
        managementUI = GameObject.Find("ManagementUI");
        questUI = GameObject.Find("QuestUI");
        SpecialSpec = GameObject.Find("SpecialSpec");
        skillSet = GameObject.Find("Skill Set");
        unitInfo = GameObject.Find("UnitInfo");
        UnitPortrait = GameObject.Find("Portrait").GetComponent<Image>();

        Actions = GameObject.Find("Actions");
        moveBtn = GameObject.Find("Move");
        attackBtn = GameObject.Find("Attact");
        skipBtn = GameObject.Find("Wait");
        skillBtn = GameObject.Find("Skill");

        Actions.SetActive(false);
        managementUI.SetActive(false);
        questUI.SetActive(false);
        SpecialSpec.SetActive(false);
        skillSet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Selecting Actor(Tile, Unit) of the Game
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                selectedActor = hit.transform.gameObject;
                HexTile tile = selectedActor.GetComponent<HexTile>();

                // Update selectedTile
                GameManager.Instance.selectedTile = tile;
                // Update selectedPoint using tile information
                GameManager.Instance.selectedPoint = tile.point;

                if (tile.point.Unit != null && tile.point.TileBuilding != null)
                {
                    if (tile.isFirstClick)
                    {
                        GameManager.Instance.selectedActor = tile.point.Unit;
                        tile.isFirstClick = false;
                        GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                    }
                    else
                    {
                        GameManager.Instance.selectedActor = tile.point.TileBuilding;
                        tile.isFirstClick = true;
                        GameManager.Instance.selectedGameObject = selectedActor;
                    }
                }
                else if (tile.point.TileBuilding != null)
                {
                    GameManager.Instance.selectedActor = tile.point.TileBuilding;
                    tile.isFirstClick = true;
                    GameManager.Instance.selectedGameObject = selectedActor;
                }
                else if (tile.point.Unit != null)
                {
                    GameManager.Instance.selectedActor = tile.point.Unit;
                    tile.isFirstClick = false;
                    GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                }
                // If neither Unit nor TileBuilding exists on the selected tile
                else
                {
                    GameManager.Instance.selectedActor = null;
                }

                // Change Button Interaction correponds to the selected Actor
                ButtonInteractChange();

                // Filcker Selected Tile with Cyan
                // IEnumerator _tileCoroutine = FlickerSelectedTile(tile);
                // StartCoroutine(_tileCoroutine);
            }
            // Set Unit Information
            if (questUI.activeSelf == false && managementUI.activeSelf == false)
            {
                if (GameManager.Instance.selectedActor != null)
                {
                    unitInfo.SetActive(true);
                    UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.Instance.selectedActor)).ToLower()), typeof(Sprite)) as Sprite;
                    if (GameManager.Instance.selectedActor is CivModel.Actor)
                    {
                        GameObject.Find("UnitName").GetComponent<Text>().text = ProductionFactoryTraits.GetName(GameManager.Instance.selectedActor);
                    }
                    if (GameManager.Instance.selectedActor is CivModel.CityBase)
                    {
                        GameObject.Find("UnitName").GetComponent<Text>().text = GameManager.Instance.selectedActor.Name;
                    }

                    GameObject.Find("UnitAttack").GetComponent<Text>().text = GameManager.Instance.selectedActor.AttackPower.ToString();
                    GameObject.Find("UnitDefence").GetComponent<Text>().text = GameManager.Instance.selectedActor.DefencePower.ToString();
                    GameObject.Find("UnitEffect").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainHP.ToString() + "/" + GameManager.Instance.selectedActor.MaxHP;
                    GameObject.Find("ActionPoint").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainAP.ToString() + "/" + GameManager.Instance.selectedActor.MaxAP;
                    GameObject.Find("HealthPoint").GetComponent<RectTransform>().sizeDelta = new Vector2(30, 280 * (float)GameManager.Instance.selectedActor.RemainHP / (float)GameManager.Instance.selectedActor.MaxHP);
                }
                else unitInfo.SetActive(false);
            }
        }
        if(SpecialSpec.activeSelf == true)
        {
            SpecialSpec.transform.position = Input.mousePosition;
        }


    }

    // Flicker Selected Tile With Cyan
    IEnumerator FlickerSelectedTile(HexTile prevTile)
    {
        while (true)
        {
            if (prevTile != GameManager.Instance.selectedTile)
            {
                prevTile.StopFlickering();
                if(GameManager.Instance.selectedActor == null)
                {
                    GameManager.Instance.selectedTile.FlickerWhite();
                }
                else
                {
                    GameManager.Instance.selectedTile.FlickerCyan();
                }
                break;
            }
            yield return null; 
        }
    }

    // On Click UI Button
    public void onClick(GameObject go)
    {
        if (GameManager.Instance.selectedGameObject != null)
        {
            if (GameManager.Instance.selectedGameObject.GetComponent<Unit>() != null)
            {
                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateExit(GameManager.Instance.selectedActor);
            }
        }
        if (go.activeSelf == false)
        {
            go.SetActive(true);
            if (go != mapUI) mapUI.SetActive(false);
            if (go != managementUI) managementUI.SetActive(false);
            if (go != questUI) questUI.SetActive(false);
        }
        else
        {
            go.SetActive(false);
            if (go != mapUI) mapUI.SetActive(true);
        }
    }

    // On Click Move Button
    public void onClickMove()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateEnter();
        }
    }
    // On Click Attack Button
    public void onClickAttack()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackStateEnter();
        }
    }
    // On Click Skill Button
    public void onClickSkill()
    {
        if (skillSet.activeSelf == false)
            skillSet.SetActive(true);
        else skillSet.SetActive(false);
    }
    // On Click a Specific Skill
    public void onClickSkillBtn(int idx)
    {
        if (GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            // Unit Skill
            if (GameManager.Instance.selectedActor is CivModel.Unit)
                GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateEnter(idx);
            // Tile Building Skill
            else if (GameManager.Instance.selectedActor is CivModel.TileBuilding)
            {
                GameManager.Instance.selectedTile.SkillStateEnter(idx);
            }
            else Debug.Log("Cannot Use Skill");
        }
    }
    // On Click Skip Button
    public void onClickSkip()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedActor.SkipFlag = true;
        }
        ButtonInteractChange();
        GameManager.Instance.CheckToDo();
        GameManager.Instance.FocusOnActableUnit();
    }

    public void SpecialMouseOver()
    {
        SpecialSpec.SetActive(true);
    }
    public void SpecialMouseExit()
    {
        SpecialSpec.SetActive(false);
    }

    //// Management UI (Production Selection) ////
    public void UnitSelTabActive()
    {
        UnitSelTab.SetActive(true);
        BuildingSelTab.SetActive(false);
    }
    public void BuildingSelTabActive()
    {
        UnitSelTab.SetActive(false);
        BuildingSelTab.SetActive(true);
    }

    public void EpicTabActive()                 // Unit production
    {
        EpicTab.SetActive(true);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(false);
    }
    public void HighTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(true);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(false);
    }
    public void IntermediateTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(true);
        LowTab.SetActive(false);
    }
    public void LowTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(true);
    }

    public void CityTabActive()                 // Building production
    {
        CityTab.SetActive(true);
        CityBuildingTab.SetActive(false);
        NormalBuildingTab.SetActive(false);
    }
    public void CityBuildingTabActive()
    {
        CityTab.SetActive(false);
        CityBuildingTab.SetActive(true);
        NormalBuildingTab.SetActive(false);
    }
    public void NormalBuildingTabActive()
    {
        CityTab.SetActive(false);
        CityBuildingTab.SetActive(false);
        NormalBuildingTab.SetActive(true);
    }

    // Change Button Interaction according to selectedActor
    public void ButtonInteractChange()
    {
        // Hide Actions Tab
        if(GameManager.Instance.selectedActor == null || GameManager.Instance.selectedActor.Owner != GameManager.Instance.Game.PlayerInTurn)
        {
            Actions.SetActive(false);
        } else
        {
            Actions.SetActive(true);
        }
        // Move Button
        if(GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            moveBtn.GetComponent<Button>().interactable = true;
        } else
        {
            moveBtn.GetComponent<Button>().interactable = false;
        }

        // Attack Button
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            attackBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            attackBtn.GetComponent<Button>().interactable = false;
        }

        // SkipButton
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            if (GameManager.Instance.selectedActor.SkipFlag == false)
            {
                skipBtn.GetComponent<Button>().interactable = true;
            } else
            {
                skipBtn.GetComponent<Button>().interactable = false;
            }
        } else
        {
            skipBtn.GetComponent<Button>().interactable = false;
        }

        // SkillButton
        if (GameManager.Instance.selectedActor is CivModel.Actor && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            if (GameManager.Instance.selectedActor.SpecialActs != null)
            {
                skillBtn.GetComponent<Button>().interactable = true;

                Button[] skillsBtn = skillSet.GetComponentsInChildren<Button>();
                foreach (var skill in skillsBtn) {
                    skill.interactable = false;
                }
                int skillIdx = 0;
                foreach(var skill in GameManager.Instance.selectedActor.SpecialActs) {
                    skillsBtn[skillIdx].interactable = true;
                    skillsBtn[skillIdx].GetComponentInChildren<Text>().text = GameManager.Instance.selectedActor.SpecialActs[skillIdx].Owner.Name;
                    skillIdx++;
                }

            }
            else
            {
                skillBtn.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            skillBtn.GetComponent<Button>().interactable = false;
        }
    }

    // Update Unit Info & selectedActor information according to the given actor
    public void updateSelectedInfo(CivModel.Actor actor)
    {
        GameManager.Instance.selectedActor = actor;
        var pt = actor.PlacedPoint.Value;
        GameManager.Instance.selectedPoint = pt;
        foreach (GameObject unit in GameManager.Instance.Units)
        {
            if (unit.GetComponent<Unit>().point == pt)
            {
                GameManager.Instance.selectedGameObject = unit;
                break;
            }
        }
        GameManager.Instance.selectedTile = GameManager.Instance.Tiles[pt.Position.X, pt.Position.Y].GetComponent<HexTile>();
        UIManager.Instance.Actions.SetActive(true);
        ButtonInteractChange();
    }

}