using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Singleton Instance 선언
    static T m_instance;

    // Singleton Instance에 접근하기 위한 프로퍼티
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                // T 라는 오브젝트의 형(컴포넌트의 형)을 검색해 가장 처음 오브젝트를 반환 한다.
                m_instance = GameObject.FindObjectOfType<T>();

                // instance 가 없을 경우 GameObject를 생성해 선언한다.
                if (m_instance == null)
                {
                    // T 컴포넌트 의 이름으로 생성 한다.
                    GameObject singleton = new GameObject(typeof(T).Name);
                    m_instance = singleton.AddComponent<T>();
                }
            }

            return m_instance;
        }
    }

    public virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
            DontDestroyOnLoad(this.gameObject); // Manager로 사용하기 위해 DontDestroyOnLoad
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
