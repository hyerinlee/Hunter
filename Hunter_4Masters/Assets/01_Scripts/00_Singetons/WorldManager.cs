﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

public class WorldManager : MonoBehaviour
{
    public class JTestClass
    {
        public int i;
        public float f;
        public bool b;
        public string str;
        public int[] iArray;
        public List<int> iList = new List<int>();
        public Dictionary<string, float> fDictionary = new Dictionary<string, float>();

        public JTestClass() { }

        public JTestClass(bool isSet)
        {
            if (isSet)
            {
                i = 10;
                f = 99.9f;
                b = true;
                str = "JSON Test String";
                iArray = new int[] { 1, 1, 3, 5, 8, 13, 21, 34, 55 };

                for (int idx = 0; idx < 5; idx++)
                {
                    iList.Add(2 * idx);
                }

                fDictionary.Add("PIE", Mathf.PI);
                fDictionary.Add("Epsilon", Mathf.Epsilon);
                fDictionary.Add("Sqrt(2)", Mathf.Sqrt(2));
            }
        }

        public void Print()
        {
            Debug.Log("i = " + i);
            Debug.Log("f = " + f);
            Debug.Log("b = " + b);
            Debug.Log("str = " + str);

            for (int idx = 0; idx < iArray.Length; idx++)
            {
                Debug.Log(string.Format("iArray[{0}] = {1}", idx, iArray[idx]));
            }

            for (int idx = 0; idx < iList.Count; idx++)
            {
                Debug.Log(string.Format("iList[{0}] = {1}", idx, iList[idx]));
            }

            foreach(var data in fDictionary)
            {
                Debug.Log(string.Format("iDictionary[{0}] = {1}", data.Key, data.Value));
            }
        }
    }

    string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    // T : 원하는 타입의 객체로 반환 처리 가능
    T JsonToOject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    void Start()
    {
        JTestClass jtc = new JTestClass(true);
        string jsonData = ObjectToJson(jtc);
        Debug.Log(jsonData);

        Debug.Log("\n\n\nJsonToOject");
        var jtc2 = JsonToOject<JTestClass>(jsonData);
        jtc2.Print();

        CreateJsonFile(Application.dataPath, "JTestClass", jsonData);

        Debug.Log("\n\n\nLoadJsonFile");
        jtc2 = LoadJsonFile<JTestClass>(Application.dataPath, "JTestClass");
        jtc2.Print();

    }

}