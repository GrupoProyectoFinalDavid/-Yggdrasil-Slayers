using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] fondos;

    void Start()
    {
        if (fondos.Length == 0) return;

        int aleatorio = Random.Range(0, fondos.Length);
        ActivarFondo(aleatorio);
    }

    public void ActivarFondo(int index)
    {
        for (int i = 0; i < fondos.Length; i++)
        {
            fondos[i].SetActive(i == index);
        }
    }
}