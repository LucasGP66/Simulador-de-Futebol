
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

//gere o menu de login, registo e saves
public class LoginManager : MonoBehaviour
{
    public TMP_InputField EmailLog;
    public TMP_InputField PasswordLog;

    public TMP_InputField EmailPass;
    public TMP_InputField PasswordPass;

    public Button btnSave1;
    public Button btnSave2;
    public Button btnSave3;

    public GameObject menuRegisto;
    public GameObject menuLogin;
    public GameObject menuSave;
    public GameObject menuApagar;
    public GameObject menuOpcoes;
    public GameObject MenuPrincipal;

    public GameObject PopVerificarEmail;
    public GameObject PopEmailNaoVerificado;

    public GameObject ErroApagar;



    public static LoginManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        if (DadosScene.IsReforma && DadosScene.IsAutenticado) // se é reforma, vai para o save menu e apaga o save
        {
            ApagaSaveMascara(DadosScene.SaveNum);
            MenusLogins.Instance.MudaMenu(menuSave);
            DadosScene.IsReforma = false;
        }
        else if (DadosScene.IsAutenticado)//se está atutenticado mas não é reforma
        {
            MostraSaves();
        }
        else
        {
            MenusLogins.Instance.MudaMenu(MenuPrincipal);
        }
        
    }
    public void JogarComLogin()
    {
        _ = asyncJogarComLogin();
    }
    private async Task asyncJogarComLogin()
    {
        await FirebaseManager.Instance.CarregaSaveBD();//carrega os saves da db
        MostraSaves();
        DadosScene.IsLogin = true;
        MenusLogins.Instance.MudaMenu(menuSave);
    }

    public void TentarLogin() //wrapper para login
    {
        _ = TentarLoginAsync();
    }
    public void TentarRegisto()//wrapper para registo
    {
        _ = TentarRegistoAsync();
    }

    private async Task TentarLoginAsync()//metodo privado que tenta dar login
    {
        bool sucesso = await FirebaseManager.Instance.TentarLogin(EmailLog.text, PasswordLog.text);

        //if (FirebaseManager.Instance.user == null) return;//se não está autenticado
        if (!sucesso)
        {
            PopEmailNaoVerificado.SetActive(true);
        }

        await FirebaseManager.Instance.CarregaSaveBD();//carrega os saves da db
        MostraSaves();

        DadosScene.IsLogin = true;
        MenusLogins.Instance.MudaMenu(MenuPrincipal);
    }
    private async Task TentarRegistoAsync()//metodo privado que tenta fazer um registo
    {
        //cria a conta
        var sucesso = await FirebaseManager.Instance.TentaRegistar(EmailPass.text, PasswordPass.text);

        if (!sucesso) return;//se falhou

        //verifica se está verificada, se não, envia a verificação -- nunca vai estar verificada, então e sempre enviada a verificação
        if (!FirebaseManager.Instance.user.IsEmailVerified)
        {
            await FirebaseManager.Instance.user.SendEmailVerificationAsync();
            FirebaseManager.Instance.auth.SignOut();
        }
        PopVerificarEmail.SetActive(true);

        //if (FirebaseManager.Instance.user == null) return;
    }

    private void MostraSaves() //atualiza os botões de save
    {
        Button[] btnArray = new Button[3] { btnSave1, btnSave2, btnSave3 };

        TMP_Text btnText1 = btnSave1.GetComponentInChildren<TMP_Text>();
        TMP_Text btnText2 = btnSave2.GetComponentInChildren<TMP_Text>();
        TMP_Text btnText3 = btnSave3.GetComponentInChildren<TMP_Text>();

        TMP_Text[] txtArray = new TMP_Text[3] { btnText1, btnText2, btnText3 };
        Jogador[] SaveArray = new Jogador[3] { FirebaseManager.Instance.save.Save1, FirebaseManager.Instance.save.Save2, FirebaseManager.Instance.save.Save3 };

        int i = 0;
        DadosScene.SavesValidos.Clear();
        foreach (var txt in txtArray)
        {
            if (SaveArray[i] != null) 
            {
                Debug.Log($"Save {i + 1} tem dados");
                txt.text = SaveArray[i].Nome;
                DadosScene.SavesValidos.Add(i + 1);
            }
            else
            {
                txt.text = $"Save {i + 1}";
            }
            i++;
        }

    }

    public async void TentaApagarConta()// apaga um utilizador
    {
        bool sucesso = await FirebaseManager.Instance.TentaApagarConta();
        if (sucesso) MenusLogins.Instance.MudaMenu(MenuPrincipal);
        else
        {
            Debug.Log("erro ao apagar");
            menuApagar.SetActive(false);
            ErroApagar.SetActive(true);
        }
    }


    public void GoSave(int savenum)//muda para o jogo
    {
        DadosScene.SaveNum = savenum;
        SceneManager.LoadScene("MainScene");
    }

    public void ApagaSaveMascara(int savenum)
    {
        _ = ApagaSave(savenum);
    }

    public async Task ApagaSave(int savenum)
    {
        await FirebaseManager.Instance.ApagaSave(savenum);
        MostraSaves();
    }

    public void IrMenuApagar()
    {
        menuApagar.SetActive(true);
    }

    public void LogOut()
    {
        FirebaseManager.Instance.LogOut();
        DadosScene.IsAutenticado = false;
        MenusLogins.Instance.MudaMenu(MenuPrincipal);
    }
}
