using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour //gere certos elementos no menu principal
{

    public GameObject Aviso;
    public GameObject Saves;
    public GameObject MenuLogin;
    public GameObject MenuOpcoes;
    public GameObject GrupoMenuPrincipal;
    public GameObject GrupoLogin;

    public void Awake()
    {
        Aviso.SetActive(false);
    }

    public void MudarJogar()//vai para os saves se tiver autenticado, ou mostra um aviso
    {
        if (!DadosScene.IsAutenticado)//se não fez login, mostra um aviso
        {
            Aviso.SetActive(true);
        }
        else
        {
            LoginManager.Instance.JogarComLogin();
        }
    }

    public void MudarLogin()//muda para o login
    {
        if (!DadosScene.IsAutenticado)
        {
            MenusLogins.Instance.MudaMenu(MenuLogin);
        }
        else
        {
            MenusLogins.Instance.MudaMenu(MenuOpcoes);
        }
    }

    public void SairJogo()//sai do jogo
    {
        Application.Quit();
    }

    #region btns aviso
    public void JogarSemLogin()
    {
        DadosScene.IsAutenticado = false;
        DadosScene.IsLogin = false;
        SceneManager.LoadScene("MainScene");
    }

    public void VoltarAviso()
    {
        Aviso.SetActive(false);
    }
    #endregion
}
