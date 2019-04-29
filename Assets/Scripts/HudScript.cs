using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour
{
    private HealthComponent PlayerHealth;
    private Image HealthFG;
    private Text HealthText;

    void Start()
    {
        PlayerHealth = transform.parent.gameObject.GetComponent<HealthComponent>();
        PlayerHealth.RaiseUpdated += UpdateHealthBar;
        HealthFG = transform.Find("HealthFG").GetComponent<Image>();

        HealthText = transform.Find("Text").GetComponent<Text>();
        HealthText.text = PlayerHealth.Maximum + "/" + PlayerHealth.Maximum + "cc";
    }

    public void UpdateHealthBar(object Sender, HealthUpdateData Data)
    {
        float CurrentHealthPct = Data.Health / PlayerHealth.Maximum;
        HealthFG.fillAmount = CurrentHealthPct;
        HealthText.text = Data.Health + "/" + PlayerHealth.Maximum + "cc";
    }
}
