using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour //gere o firebase
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth auth {  get; private set; }
    public FirebaseFirestore db { get; private set; }

    public FirebaseUser user { get; private set; }

    public Save save;


    private void Awake()
    {
        //Garante que só existe um FirebaseManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); //mantém o Firebase entre cenas
    }

    private void Start()
    {
        IncializarDB();
    }

    //FUNCAO QUE INICIALIZA O FIREBASE
    private void IncializarDB()
    {
        Debug.Log("A inicializar Firebase...");

        //informação do firebase
        var options = new AppOptions
        {
            /*Parte do codigo omitida por questões de segurança*/
        };

        //tenta criar uma nova instanca do firebase
        try
        {
            var app = FirebaseApp.Create(options);
            auth = FirebaseAuth.GetAuth(app);
            db = FirebaseFirestore.GetInstance(app);
            Debug.Log("Firebase inicializado com sucesso!");
            Debug.Log("Project ID: " + app.Options.ProjectId);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao criar FirebaseApp: " + e.Message);
        }
    }

    //FUNCAO DE LOGIN
    public async Task<bool> TentarLogin(string email, string password)
    {

        try
        {
            var resultado = await auth.SignInWithEmailAndPasswordAsync(email, password);
            if (!resultado.User.IsEmailVerified)
            {
                auth.SignOut();
                return false;
            }
            user = resultado.User;
            Debug.Log("Sucesso ao fazer login!");
            DadosScene.IsAutenticado = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao fazer login: " + e.Message);
            return false;
        }
    }

    //FUNCAO DE REGISTO
    public async Task<bool> TentaRegistar(string email, string password)
    {
        try
        {
            user = (await auth.CreateUserWithEmailAndPasswordAsync(email, password)).User;//cria um user
            if (user == null) return false;
            Debug.Log("utilizador criado com sucesso!");
            Save saveFormato  = new Save();
            await db.Collection("player").Document(user.UserId).SetAsync(saveFormato);//envia para a db um save vazio
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Falha ao criar utilizador! " + e.Message);
            return false;
        }
    }

    public async Task<bool> TentaApagarConta()
    {
        if (!DadosScene.IsAutenticado) return false;
        try
        {
            await user.DeleteAsync();
            Debug.Log("Utilizador Apagado com sucesso");
            DadosScene.IsAutenticado = false;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Falha ao apagar utilizador! " + e.Message);
            return false;
        }
    }

    //FUNCAO DE CARREGAR UM JOGADOR DA BASE DE DADOS
    public async Task CarregaSaveBD()//carrega um save
    {
        if (FirebaseManager.Instance == null)//se o firebase está nulo
        {
            return;
        }
        DocumentSnapshot snap = await db.Collection("player").Document(user.UserId).GetSnapshotAsync();//o snap é o "documento" do jogador

        if(snap.Exists)//se existe
        {
            save = snap.ConvertTo<Save>();//converte o documento para um save
        }
        else
        {
            Debug.Log("snap está vazio");
        }
    }

    public void CarregaPlayerBD(int saveNum) //vai ao save e da "load" de um jogador
    {

        switch (saveNum)
        {
            case 1:
                {
                    //cada um destes blocos carrega um jogador, e certefica-se que não tem nada por inicializar
                    PlayerManager.Instance.Player = save.Save1;
                    PlayerManager.Instance.Player.JAtributos ??= new Atributos(PlayerManager.Instance.Player.Posicao);// ??= se for nulo, atribui este valor, se não for, deixa como está, coalescencia nula
                    PlayerManager.Instance.Player.JStats ??= new Estatisticas();
                    PlayerManager.Instance.Player.JStats.StatsHistorico ??= new List<string>();
                    //PlayerManager.Instance.Player.turno ??= new Turno(0, 0);
                    TurnoManager.Instance.turno = PlayerManager.Instance.Player.turno;
                    Debug.Log("Jogador carregado com sucesso!");
                    Debug.Log($"Turno e época: {PlayerManager.Instance.Player.turno.TurnoNum}, {PlayerManager.Instance.Player.turno.Epoca}");
                    return;
                }
            case 2:
                {
                    PlayerManager.Instance.Player = save.Save2;
                    PlayerManager.Instance.Player.JAtributos ??= new Atributos(PlayerManager.Instance.Player.Posicao);
                    PlayerManager.Instance.Player.JStats ??= new Estatisticas();
                    PlayerManager.Instance.Player.JStats.StatsHistorico ??= new List<string>();
                    PlayerManager.Instance.Player.turno ??= new Turno(0, 0);
                    TurnoManager.Instance.turno = PlayerManager.Instance.Player.turno;
                    Debug.Log("Jogador carregado com sucesso!");
                    return;
                }
            case 3:
                {
                    PlayerManager.Instance.Player = save.Save3;
                    PlayerManager.Instance.Player.JAtributos ??= new Atributos(PlayerManager.Instance.Player.Posicao);
                    PlayerManager.Instance.Player.JStats ??= new Estatisticas();
                    PlayerManager.Instance.Player.JStats.StatsHistorico ??= new List<string>();
                    PlayerManager.Instance.Player.turno ??= new Turno(0, 0);
                    TurnoManager.Instance.turno = PlayerManager.Instance.Player.turno;
                    Debug.Log("Jogador carregado com sucesso!");
                    return;
                }
        }

    }

    //FUNCAO QUE GUARDA O JOGADOR NA BD

    public async Task AtualizaPlayerBD(FirebaseUser user, Jogador player)
    {
        //tem de ser assim, tentei fazer com o for e um array, mas não passa por referencia
        //percorre todos os save slots, verifica qual deles é o slot a ser usado
        if (save.Save1 == player) save.Save1 = player;
        else if (save.Save2 == player) save.Save2 = player;
        else if (save.Save3 == player) save.Save3 = player;
        else // se nenhum save é igual, é porque é novo, vê o slot escolhido
        {
            Debug.Log("Save é novo, a adicionar a novo slot, SaveNum " + DadosScene.SaveNum.ToString());
            if (DadosScene.SaveNum == 1) save.Save1 = player;
            else if (DadosScene.SaveNum == 2) save.Save2 = player;
            else if (DadosScene.SaveNum == 3) save.Save3 = player;
        }

        try
        {
            string uid = user.UserId;

            Debug.Log("A tentar enviar player para a bd");
            await db.Collection("player").Document(user.UserId).SetAsync(save);
            Debug.Log("Player enviado para a bd!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Algo correu mal {e.Message}");
        }

    }

    //funcao que apaga um save
    public async Task ApagaSave(int savenum)
    {
        switch (savenum)
        {
            case 1:
                {
                    if (save.Save1 != null)
                    {
                        save.Save1 = null;
                        await db.Collection("player").Document(user.UserId).SetAsync(save);
                        Debug.Log("Save 1 apagado com sucesso");
                    }
                    break;
                }
            case 2:
                {
                    if (save.Save2 != null)
                    {
                        save.Save2 = null;
                        await db.Collection("player").Document(user.UserId).SetAsync(save);
                        Debug.Log("Save 2 apagado com sucesso");
                    }
                    break;
                }
            case 3:
                {
                    if (save.Save3 != null)
                    {
                        save.Save3 = null;
                        await db.Collection("player").Document(user.UserId).SetAsync(save);
                        Debug.Log("Save 3 apagado com sucesso");
                    }
                    break;
                }
        }
    }


    //LOGOUT
    public void LogOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        DadosScene.IsAutenticado = false;
    }
}

