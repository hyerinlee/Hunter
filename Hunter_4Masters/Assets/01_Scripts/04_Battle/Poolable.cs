using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    protected ObjectPool pool;

    // Pool 할 오브젝트 미리 생성
    public virtual void Create(ObjectPool pool)
    {
        this.pool = pool;
        gameObject.SetActive(false);
    }

    public virtual void Push()
    {
        pool.Push(this);
    }
}