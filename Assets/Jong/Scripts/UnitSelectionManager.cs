using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance
    {
        get; set;
    }
    
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();
    public GameObject floorMarker;


    bool isUnitGrid = false;
    private void Awake()
    {
        if(Instance != null && !Instance == this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("PlayerUnit")))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hitInfo.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hitInfo.collider.gameObject);
                }
            }
            else // 유닛이 아닌 것을 클릭
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
            {
                floorMarker.transform.position = hitInfo.point;
                floorMarker.SetActive(false);
                floorMarker.SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
            {
                PathRequestManager.CheckUnitGrid(hitInfo.point, out isUnitGrid);
                Debug.Log("isUnitGrid : " + isUnitGrid);
            }
        }
    }

    private void SelectByClicking(GameObject _selectUnit)
    {
        DeselectAll();

        unitsSelected.Add(_selectUnit);
        SelectUnits(_selectUnit, true);

    }
    private void MultiSelect(GameObject _selectUnit)
    {
        if(unitsSelected.Contains(_selectUnit) == false)
        {
            unitsSelected.Add(_selectUnit);
            SelectUnits(_selectUnit, true);

        }
        else
        {
            EnableUnitMovement(_selectUnit, false);
            VisibleIndicator(_selectUnit, true);
            unitsSelected.Remove(_selectUnit);
        }
    }
    public void DeselectAll()
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit != null)
            {
                SelectUnits(unit, false);
            }
        }
        unitsSelected.Clear();
        floorMarker.SetActive(false);
    }
    
    private void SelectUnits(GameObject _gameObject, bool _select)
    {
        EnableUnitMovement(_gameObject, _select);
        VisibleIndicator(_gameObject, _select);
    }
    public void DragSelect(GameObject _selectUnit)  
    {
        if(unitsSelected.Contains(_selectUnit) == false)
        {
            unitsSelected.Add(_selectUnit);
            SelectUnits(_selectUnit, true);

        }
    }
    public void EnableUnitMovement(GameObject _selectUnit, bool _shouldMove)
    {
        if (_selectUnit == null) return;

        PlayerUnit playerUnit = _selectUnit.GetComponent<PlayerUnit>();

        if (playerUnit == null) return;

        // 안전하게 값 변경
        playerUnit.shouldMove = _shouldMove;
    }

    private void VisibleIndicator(GameObject _selectUnit, bool _visible)
    {
        if (_selectUnit.transform.childCount > 0)
            _selectUnit.transform.GetChild(0).gameObject.SetActive(_visible);
    }

} 
