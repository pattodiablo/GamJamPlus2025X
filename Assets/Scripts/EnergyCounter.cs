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

  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energyIcon1.SetActive(false);
        energyIcon2.SetActive(false);
        energyIcon3.SetActive(false);
        energyIcon4.SetActive(false);
        energyIcon5.SetActive(false);
    }

  public void AddEnergy(int amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        switch (currentEnergy)
        {
            case 1:
                energyIcon1.SetActive(true);
                break;
            case 2:
                energyIcon2.SetActive(true);
                break;
            case 3:
                energyIcon3.SetActive(true);
                break;
            case 4:
                energyIcon4.SetActive(true);
                break;
            case 5:
                energyIcon5.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void RemoveEnergy(int amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        switch (currentEnergy)
        {
            case 1:
                energyIcon1.SetActive(false);
                break;
            case 2:
                energyIcon2.SetActive(false);
                break;
            case 3:
                energyIcon3.SetActive(false);
                break;
            case 4:
                energyIcon4.SetActive(false);
                break;
            case 5:
                energyIcon5.SetActive(false);
                break;
            default:
                break;
        }
    }   
    
   
}
