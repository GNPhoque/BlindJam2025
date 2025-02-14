using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public Action<KeyCode> OnPlayerKeyPushed;
    public Action PushedUp;
    public Action PushedDown;
    public Action PushedOk;
    public Action PushedCancel;

    public static Inputs instance;

    public KeyCode[] humanKeys = new KeyCode[]
    {
        KeyCode.Space,
        KeyCode.A
    };

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PushedUp?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
			PushedDown?.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Return))
        {
			PushedOk?.Invoke();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			PushedCancel?.Invoke();
		}

        foreach (var key in humanKeys)
        {
            if (Input.GetKeyDown(key))
            {
                OnPlayerKeyPushed?.Invoke(key);
            }
        }
	}

}
