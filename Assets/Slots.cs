using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Slots : MonoBehaviour
{
    public List<Image> slots;
    public Image ObjectSlot;
    public Transform DragSlot;
    private Vector3 InitialDragPosition;
    public List<Sprite> objects;
    public List<Sprite> SlotsImages;
    public Dictionary<int, bool> usedObjects = new Dictionary<int, bool>();
    private Transform currentSlot;
    private Vector3 currentSlotTargetPosition;
    public int mode;
    private Action CBack;
    public float forwardFactor;
    public float forwardSpeed;
    public float backSpeed;
    private int index;
    private bool drag;
    private Vector3 strtdragmousePosition;
    private Vector3 startdragObjectPosition;
    private int objIndex;
    private bool clone;
    public float returnSpeed;
    public CanvasGroup WinText;
    public CanvasGroup MainPanel;
    private bool Win;

    public Vector3 DragPosition { get; private set; }

    void Start()
    {
        WinText.alpha = 0;
        DragPosition = ObjectSlot.transform.position;
        for (int i = 0; i < slots.Count; i++)
        {
            usedObjects.Add(i, false);
            slots[i].sprite = SlotsImages[i];
            slots[i].transform.localScale = Vector3.zero;
        }
        index = 0;
        GoAppear(() =>
        {
            index = 1;
            GoAppear(() =>
            {
                index = 2;
                GoAppear(NewObject);

            });
        });
    }
    private void NewObject()
    {
        Sounds.Play("Appear",1);

        int searching = usedObjects.Count;
        objIndex = -1;
        foreach (KeyValuePair<int, bool> kvp in usedObjects)
            if (!kvp.Value)
            {
                objIndex = kvp.Key;

            }
            else
                searching--;

        if (searching == 0)
        {
            //EditorApplication.isPaused = true;
            print("WIN!!!!!!!!!!!!!!!!!");
            Win = true;
            IEnumerator cor = ShowWin();
            StartCoroutine(cor);
        }
        else
        {
            if (clone)
            {
                print("New Object");
                DragSlot = Instantiate(ObjectSlot.gameObject, ObjectSlot.transform.parent).GetComponent<Transform>();
                DragSlot.position = DragPosition;
                ObjectSlot = DragSlot.GetComponent<Image>();
            }
            if (objIndex == -1)
                objIndex = Random.Range(0, objects.Count);



            usedObjects[objIndex] = true;
            ObjectSlot.sprite = objects[objIndex];

            currentSlot = ObjectSlot.transform;
            currentSlotTargetPosition = currentSlot.localPosition;
            currentSlot.localPosition -= Vector3.up * 500;
            currentSlot.localScale = Vector3.zero;
            CBack = null;
            mode = 1;
        }
    }
    private IEnumerator ShowWin()
    {
        print("ShowWin");


        while (WinText.alpha <= 1)
        {
            WinText.alpha += 0.01f;
            MainPanel.alpha -= 0.1f;
            yield return new WaitForEndOfFrame();
        }

        print("done");
        yield return null;
    }
    public void StartDRAG(Transform target)
    {

        if (Win || target != currentSlot) return;


        DragSlot = ObjectSlot.transform;
        InitialDragPosition = ObjectSlot.transform.position;
        strtdragmousePosition = Input.mousePosition;
        startdragObjectPosition = currentSlot.transform.localPosition;
        drag = true;
    }
    public void StopDRAG(Transform target)
    {
        if (Win || target != currentSlot) return;

        drag = false;
        CheckSlots();
    }

    private void CheckSlots()
    {
        float distance;
        bool match = false;
        clone = false;
        for (int i = 0; i < slots.Count; i++)
        {
            distance = Vector3.Distance(slots[i].transform.position, DragSlot.position);
            if (distance < 10 && i == objIndex)
            {
                DragSlot.position = slots[i].transform.position;
                print("Match!!!"); clone = true;
                
                NewObject();
                Sounds.Play("Success", 1);
            }
        }
        if (!clone)
        {
            print("MISS"); Sounds.Play("Whip", 1);
            mode = 3;
        }
    }

    private void GoAppear(Action callBack)
    {
        
        CBack = callBack;
        currentSlot = slots[index].transform;
        
        currentSlotTargetPosition = currentSlot.localPosition;
              currentSlot.localPosition += Vector3.up * 500;
        mode = 1;
        
    }

    void Update()
    {


        switch (mode)
        {
            case 1:
                currentSlot.localScale = Vector3.Lerp(currentSlot.localScale, Vector3.one * forwardFactor, forwardSpeed);
                currentSlot.localPosition = Vector3.Lerp(currentSlot.localPosition, currentSlotTargetPosition, forwardSpeed);
                if (Vector2.Distance(currentSlot.localScale, Vector3.one * forwardFactor) <= 0.1f)
                    mode = 2;

                break;
            case 2:
                currentSlot.localScale = Vector3.Lerp(currentSlot.localScale, Vector3.one, backSpeed);
                currentSlot.localPosition = Vector3.Lerp(currentSlot.localPosition, currentSlotTargetPosition, forwardSpeed);
                if (Vector2.Distance(currentSlot.localScale, Vector3.one) <= 0.01f)
                {
                    mode = 0; Sounds.Play("Appear", 1);
                    if (CBack != null) CBack();
                }
                break;
            case 3:
                currentSlot.transform.position = Vector3.Lerp(currentSlot.transform.position, InitialDragPosition, returnSpeed);
                if (Vector2.Distance(currentSlot.transform.position, InitialDragPosition) <= 0.01f)
                    mode = 0;
                break;

        }

    }

    private void FixedUpdate()
    {
        if (drag)
        {
            currentSlot.transform.localPosition = startdragObjectPosition + (Input.mousePosition - strtdragmousePosition);
        }
    }

    private void LateUpdate()
    {

    }
}