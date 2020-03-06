using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolObjs
{
    public GameObject Obj;
    public Transform parent;
    public int Count;
    public List<GameObject> ObjList;
    public string name;

    public void Init()
    {
        ObjList = new List<GameObject>();
    }
}

public class PoolingManager : MonoBehaviour
{
    [SerializeField]
    public List<PoolObjs> PoolList = new List<PoolObjs>();

    //빠르게 검색하기 위해서(사실 for문 돌리기 귀찮아서)
    Dictionary<string, int> PoolAddr = new Dictionary<string, int>();

    private void Awake()
    {
        //리스트에 있는 모든 오브젝트들을 풀링한다
        for (int i = 0; i < PoolList.Count; ++i)
        {
            PoolList[i].Init();
            PoolAddr.Add(PoolList[i].name, i);
            for (int j = 0; j < PoolList[i].Count; ++j)
            {
                GameObject obj = Instantiate(PoolList[i].Obj);
                obj.transform.parent = PoolList[i].parent;
                PoolList[i].ObjList.Add(obj);
            }
        }
    }

    //오브젝트를 부르는 함수
    public void ObjOn(string name)
    {
        int num = PoolAddr[name];
        PoolList[num].ObjList[0].SetActive(true);
        PoolList[num].ObjList.RemoveAt(0);
    }

    public void ObjSet(string name, GameObject obj)
    {
        int num = PoolAddr[name];
        PoolList[num].ObjList.Add(obj);
    }

   

}
