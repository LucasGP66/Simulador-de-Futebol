using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour //contem o objeto do jogador
{
    public static PlayerManager Instance;
    //pega o valor do edt e do menu dropdown
    public TMP_InputField edtNome;
    public TMP_Dropdown dropPosicao;
    public TMP_Dropdown dropClube;
    public TMP_Dropdown dropRep;
    public Jogador Player;

    public GameObject MenuPrincipal;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (DadosScene.IsLogin) //se foi feito um login
        {
            if (DadosScene.SavesValidos.Contains(DadosScene.SaveNum))//e se o save que estamos a usar é valido
            {
                FirebaseManager.Instance.CarregaPlayerBD(DadosScene.SaveNum);//carrega o save do jogador escolhido

                TodosOsMenus.Instance.MudarMenu(MenuPrincipal);

                TurnoManager.Instance.AtualizaTurnoTxt();
            }
        }
    }


    //criar player falso
    public void CriaPlayerAsync()
    {
        _ = CriaPlayer(); // _ descarta o resultado da Task
    }

    //cria um jogador e envia para a bd
    private async Task CriaPlayer()
    {
        string repString = dropRep.options[dropRep.value].text;
        int rep = repString switch
        {
            "Baixa" => 300,
            "Media" => 550,
            "Alta" => 700,
            _ => 300
        };
        string nome = edtNome.text;
        string posicao = dropPosicao.options[dropPosicao.value].text;
        string clubeNome = dropClube.options[dropClube.value].text;

        if (nome == null || nome == "")
        {
            PopUpManager.Instance.MostraMensagem("Tens de dar um nome ao teu jogador!", 2f);
            return;
        }

        Player = new Jogador(nome, posicao, clubeNome, rep);

        TurnoManager.Instance.AtualizaTurnoTxt();

        if (!DadosScene.IsAutenticado)//se não é autenticafdo, não é preciso mandar para a BD
        {
            TodosOsMenus.Instance.MudarMenu(MenuPrincipal);
            return;
        }

        await FirebaseManager.Instance.AtualizaPlayerBD(FirebaseManager.Instance.user, Player);

        Debug.Log("Player criado e enviado com sucesso!");

        TodosOsMenus.Instance.MudarMenu(MenuPrincipal);


    }

    public async Task AtualizaPlayer()
    {
        await FirebaseManager.Instance.AtualizaPlayerBD(FirebaseManager.Instance.user, Player);
    }

}
