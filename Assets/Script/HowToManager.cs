using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToManager : MonoBehaviour
{
    [SerializeField] GameObject howToPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHowToButtonClicked()
    {
        Invoke("ActivateHowToPanel", 0.3f);
    }

    void ActivateHowToPanel()
    {
        howToPanel.SetActive(true);
    }
}
