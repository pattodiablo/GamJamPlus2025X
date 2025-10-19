using UnityEngine;

public class EnergyCounter : MonoBehaviour
{
    public GameObject energyIcon1;
    public GameObject energyIcon2;
    public GameObject energyIcon3;
    public GameObject energyIcon4;
    public GameObject energyIcon5;

    public int currentEnergy = 0;
    public int maxEnergy = 5;

    void Start()
    {
        UpdateIcons();
    }

    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        UpdateIcons();
    }

    public void RemoveEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy - amount, 0, maxEnergy);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        var icons = new GameObject[] { energyIcon1, energyIcon2, energyIcon3, energyIcon4, energyIcon5 };
        int maxIcons = icons.Length;
        int activeCount = Mathf.Clamp(currentEnergy, 0, maxIcons);

        for (int i = 0; i < maxIcons; i++)
        {
            if (icons[i] != null)
                icons[i].SetActive(i < activeCount);
        }
    }
}
