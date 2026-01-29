using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TodosOsMenus : MonoBehaviour //gere a mudança entre menus
{
    public GameObject menuTreino;
    public GameObject menuPerfil;
    public GameObject ConfirmarReforma;
    public List<GameObject> ListaMenus;
    public float time = 0.2f;

    public static TodosOsMenus Instance;//permite mudar de menu

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void MudarMenu(GameObject menuAbrir)//muda entre dois menus -- menus tem animação
    {

        foreach(var menu in ListaMenus)
        {
            if (menu == menuPerfil && menuAbrir == ConfirmarReforma) continue;//o menu perfil não recebe animação se abrirmos a reforma

            menu.transform.localPosition = new Vector3(1, 1, 1);
            if (menu.activeSelf && menu != menuAbrir) LeanTween.scale(menu, new Vector3(0, 0, 0), time).setOnComplete(() => menu.SetActive(false));
            //menu.SetActive(false);
        }

        menuAbrir.transform.localPosition = new Vector3(0, 0, 0);
        menuAbrir.SetActive(true);
        LeanTween.scale(menuAbrir, new Vector3(1, 1, 1), time);

        if (menuAbrir == menuTreino) BarrasManager.Instance.BarraCansaco.value = PlayerManager.Instance.Player.Cansaco;
        if (menuAbrir == ConfirmarReforma) menuPerfil.SetActive(true);

    }
    public void SaiJogo()
    {
        if(DadosScene.IsAutenticado)
        {
            _ = PlayerManager.Instance.AtualizaPlayer();
        }
        SceneManager.LoadScene("Login");
    }
}
