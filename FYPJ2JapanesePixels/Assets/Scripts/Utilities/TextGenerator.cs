using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGenerator : MonoBehaviour {
    public List<GameObject> destroyRefs;
    public int size = 10;
    private Canvas canvas2d;
    private Canvas canvas3d;
    public int m_TimeToDestroy = 1;
    private TimerRoutine timer;
	// Use this for initialization
	void Awake () {
        canvas2d = GameObject.FindGameObjectWithTag("2DCanvas").GetComponent<Canvas>();
        canvas3d = GameObject.FindGameObjectWithTag("3DCanvas").GetComponent<Canvas>();
        destroyRefs = new List<GameObject>(size);
        timer = gameObject.AddComponent<TimerRoutine>();
        timer.initTimer(m_TimeToDestroy);
        timer.executedFunction = destroyText;
	}
	
	// Update is called once per frame
	void Update () {
		animate();
	}

    public void GenerateText(string printTxt, bool is3d, Vector3 spawnPos, float yOffset = 1, bool isDestroy = true, Color? col = null)
    {
        Vector3 pos = new Vector3(spawnPos.x, spawnPos.y + yOffset, spawnPos.z);

        if (is3d)
        {
            Text txt = GameObject.Instantiate(canvas3d.transform.GetChild(0), canvas3d.transform).GetComponent<Text>();
            //Text txt = canvas3d.gameObject.AddComponent<Text>();
            RectTransform t = txt.GetComponent<RectTransform>();
            t.position = pos;
            //t.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            txt.text = printTxt;
            //txt.fontStyle = FontStyle.Bold;
            //txt.alignment = TextAnchor.MiddleLeft;
            txt.fontSize = size;
            txt.gameObject.SetActive(true);
            //txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            //txt.verticalOverflow = VerticalWrapMode.Overflow;
            if (col != null)
                txt.color = col.Value;
            if (isDestroy)
            {
                destroyRefs.Add(txt.gameObject);
                timer.executeFunction();
            }
        }
    }

    public void destroyText()
    {
        Destroy(destroyRefs[0]);
        destroyRefs.RemoveAt(0);
    }

    public void animate()
    {
        if (destroyRefs.Count > 0)
        {
            for (int i = 0; i < destroyRefs.Count; ++i)
            {
                destroyRefs[i].transform.Translate(Vector2.up * (0.25f * Time.deltaTime));
            }
        }
    }

    void OnDestroy()
    {
        if (destroyRefs.Count > 0)
        {
            for (int i = 0; i < destroyRefs.Count; ++i)
            {
                Destroy(destroyRefs[i]);
            }
        }
        //StopAllCoroutines();
    }
}
