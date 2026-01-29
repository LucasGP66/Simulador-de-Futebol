using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using NUnit.Framework;

public class PopUpManager : MonoBehaviour //gere os popups, exceto o de renovação de contrato e o de baixa aprovação do treinador
{
    public GameObject popAgenteLivre;
    public GameObject popUpGenerico;
    public GameObject popUpTrofeu;

    //corpo do popUpTrofeu
    public TMP_Text CorpoTrofeu;
    public Image ImagemTrofeu;
    public Sprite taca;
    public Sprite liga;
    public Sprite champions;

    public GameObject popupMensagem;
    public GameObject popupSair;

    public LeanTweenType ease;
    public LeanTweenType easeout;

    public static PopUpManager Instance;


    //aquilo que existe dentro do pop up generico
    public TMP_Text Titulo;
    public TMP_Text Corpo;
    public Button btnEsquerda;
    public Button btnDireita;
    public TMP_Text txtBtnDireita;
    public TMP_Text txtBtnEsquerda;
    public float time = 1f;

    //corpo do popup de lesão
    public TMP_Text CorpoLesao;
    public GameObject popLesao;

    public void Start()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;
        }

        Instance = this;

        popupSair.SetActive(false);
        popLesao.SetActive(false);
        popUpTrofeu.SetActive(false);
    }

    #region mensagem txt
    public void MostraMensagem(string mensagem, float tempo)//mostra uma mensagem com efeito de fade in
    {
        if (popupMensagem.LeanIsTweening()) popupMensagem.LeanCancel(); //cancela a animação se já estiver a animar
        
        //atualiza texto
        var texto = popupMensagem.GetComponentInChildren<TMP_Text>();
        texto.text = mensagem;

        //vai buscar canvas group
        var canvas = popupMensagem.GetComponent<CanvasGroup>();

        //da fade in da mensagem, desativa a mensagem no fim
        popupMensagem.SetActive(true);
        LeanTween.alphaCanvas(canvas, 1f, time);

        LeanTween.alphaCanvas(canvas, 0f, time)
            .setDelay(tempo)
            .setOnComplete(() => popupMensagem.SetActive(false));
    }
    #endregion

    public void AbrePop(GameObject popup)//anima o popup
    {
        Debug.Log("A abrir pop");
        popup.transform.localScale = new Vector3(0, 0, 0);
        popup.SetActive(true);
        LeanTween.scale(popup, new Vector3(1, 1, 1), time).setEase(ease);
    }
    public void FechaPop(GameObject popup)//anima o popup, quando acaba a animação fecha
    {
        Debug.Log("A fechar pop");
        popup.transform.localScale = new Vector3(1, 1, 1);
        LeanTween.scale(popup, new Vector3(0, 0, 0), time).setEase(easeout).setOnComplete(() => popup.SetActive(false)); //set on complete não aceita funções com parametros, tenho de usar lambda
    }
    public void OferecidoAClube()// quando o jogador tem baixa reputação e é oferecido sair do clube
    {
        var Player = PlayerManager.Instance.Player;

        Clube clubeInteressado = null;
        if (1 == Random.Range(1, 3)) clubeInteressado = Player.TreinadorTentaVender(); //uma chance em 3 de seres oferecido a um clube

        if (clubeInteressado == null) return;

        //caso funcione
        AbrePop(popUpGenerico);

        Titulo.text = "O teu treinador quer falar contigo...";

        Corpo.text = "Hoje, o presidente e o treinador pediram para fazer um reunião contigo. Eles disseram que não tem espaço para ti e acreditam que seria melhor se abandonasses o clube.";
        Corpo.text += $"\nO {clubeInteressado.Nome} fez uma proposta interessante por ti, e o clube está pronto a vender-te, mas não te podem forçar.";
        Corpo.text += $"\nJuntas-te ao {clubeInteressado.Nome}, ou decides ficar no {Player.Clube}?";

        //limpa os botões das funcões
        btnEsquerda.onClick.RemoveAllListeners();
        btnDireita.onClick.RemoveAllListeners();

        //botão da direita
        txtBtnDireita.text = "Mudar de clube";
        btnDireita.onClick.AddListener(() => //lambda
        {
            Player.Clube = clubeInteressado.Nome;
            Player.Contrato = 3; //um contrato de 3 anos é oferecido
            Player.DefineAprov();
            FechaPop(popUpGenerico);
            ResumosTexto.Instance.ResumoPlayerForcadoSair();
        });

        //botão da esquerda
        txtBtnEsquerda.text = "Recusar oferta";
        btnEsquerda.onClick.AddListener(() => //lambda
        {
            Player.Reputacao -= 20;
            if (Player.Reputacao < 0) Player.Reputacao = 0;
            FechaPop(popUpGenerico);
            ResumosTexto.Instance.ResumoPlayerPermanece(clubeInteressado.Nome);
        });

    }
    public void PopUpLesao()//popup de lesão
    {
        var Player = PlayerManager.Instance.Player;
        AbrePop(popLesao);

        //textos para o corpo
        string[] txtMuitoGrave = new string[3] {
            $"Durante um jogo, um adversário faz uma entrada extremamente violenta e partiste a perna. Foste de urgencia para o hospital.\n\nVais ficar fora {Player.MesRepouso} meses.",
            $"Estavas no treino a fazer um exercício rotina quando pousas mal a perna e cais no chão em dor extrema.\n\nOs médicos dizem que vais ficar fora {Player.MesRepouso} meses.",
            $"Sentes um estalido seco no joelho, seguido de uma dor aguda que te impede de continuar. O estádio parece ficar em silêncio enquanto cais. À medida que és assistido, torna-se claro: trata-se de uma rutura dos ligamentos cruzados.\n\nVais ficar fora {Player.MesRepouso} meses." };

        string[] txtGrave = new string[2] {
            $"Durante um jogo levas um encontrão e a tua perna não está bem. És imediatamente substituido e os médicos do clube dizem que é uma lesão grave.\n\nVais ficar fora {Player.MesRepouso} meses.",
            $"No auge do sprint, sentes uma fisgada brusca que te faz travar de imediato. A equipa médica corre para te avaliar. Minutos depois, chega a confirmação: uma lesão que te afastará por {Player.MesRepouso} meses, deixando a equipa sem a tua velocidade explosiva no momento em que mais precisavam de ti."
        };

        string txtMedio = "Durante um jogo sentiste um desconforto estranho. Após avisares o treinador, a equipa médica examina-te, e avisam que vais ficar fora por 4 jogos.";

        string[] txtLeve = new string[2]
        {
            "Durante um exercício rotina no treino, começaste a sentir um ligeiro desconforto. A equipa médica informa-te que se calhar é melhor não treinares por uns tempos",
            "Na tentativa dramática de arrancar uma falta, atiras-te ao chão com tanto entusiasmo que acabas por te aleijar. O treinador mete as mão a cabeça. Ficaste dorido, é melhor não treinar por algum tempo"
        };
        
        //chama o texto aleatorio
        switch (Player.TipoLesao)
        {
            case "Muito Grave":
                {
                    CorpoLesao.text = txtMuitoGrave[UnityEngine.Random.Range(1, txtMuitoGrave.Length)];
                    break;
                }
            case "Grave":
                {
                    CorpoLesao.text = txtGrave[UnityEngine.Random.Range(1, txtGrave.Length)];
                    break;
                }
            case "Média":
                {
                    CorpoLesao.text = txtMedio;
                    break;
                }
            default:
                {
                    CorpoLesao.text = txtLeve[UnityEngine.Random.Range(1, txtLeve.Length)];
                    break;
                }
        }

        Debug.Log("Lesão " + Player.TipoLesao);
        Debug.Log("meses " + Player.MesRepouso);
    }
    public void PopUpEventoAleatorio(EventosSO evento)
    {
        var Player = PlayerManager.Instance.Player;

        //Adiciona o texto ao evento
        Titulo.text = evento.Titulo;
        Corpo.text = evento.Corpo;
        txtBtnEsquerda.text = evento.Opcao1.OpcaoNome;
        txtBtnDireita.text = evento.Opcao2.OpcaoNome;

        //retira todas as funcões dos botões por segurança
        btnDireita.onClick.RemoveAllListeners();
        btnEsquerda.onClick.RemoveAllListeners();

        btnEsquerda.onClick.AddListener(() =>
        {
            var opcao1 = evento.Opcao1;
            var chanceSucesso = opcao1.ChanceSucesso;
            if(Random.Range(1, 101) <= chanceSucesso) //Caso haja sucesso
            {
                //texto do outcome
                ResumosTexto.Instance.ResumoEventoOutcome(opcao1.Sucesso.TextoEfeito);

                //aplica efeitos
                Player.AprovTreinador += opcao1.Sucesso.AfetaAprov;
                Player.Reputacao += opcao1.Sucesso.AfetaReputacao;
                Player.JStats.GolosTurno += opcao1.Sucesso.AfetaGolos;
                Player.JStats.AssistTurno += opcao1.Sucesso.AfetaAssists;
                Player.JStats.DefesaTurno += opcao1.Sucesso.AfetaDefesas;
                Player.JAtributos.Forca += opcao1.Sucesso.AfetaForca;
                Player.JAtributos.Rapidez += opcao1.Sucesso.AfetaRapidez;
                Player.JAtributos.Remate += opcao1.Sucesso.AfetaRemate;
                Player.JAtributos.Passe += opcao1.Sucesso.AfetaPasse;
                Player.JAtributos.Defesa += opcao1.Sucesso.AfetaDefesaAtributo;
                Player.Cansaco += opcao1.Sucesso.AfetaCansaco;
                Player.DanoAcumulado += opcao1.Sucesso.AfetaDano;

                FechaPop(popUpGenerico);
            }
            else//caso de falha
            {

                ResumosTexto.Instance.ResumoEventoOutcome(opcao1.Falha.TextoEfeito);

                Player.AprovTreinador += opcao1.Falha.AfetaAprov;
                Player.Reputacao += opcao1.Falha.AfetaReputacao;
                Player.JStats.GolosTurno += opcao1.Falha.AfetaGolos;
                Player.JStats.AssistTurno += opcao1.Falha.AfetaAssists;
                Player.JStats.DefesaTurno += opcao1.Falha.AfetaDefesas;
                Player.JAtributos.Forca += opcao1.Falha.AfetaForca;
                Player.JAtributos.Rapidez += opcao1.Falha.AfetaRapidez;
                Player.JAtributos.Remate += opcao1.Falha.AfetaRemate;
                Player.JAtributos.Passe += opcao1.Falha.AfetaPasse;
                Player.JAtributos.Defesa += opcao1.Falha.AfetaDefesaAtributo;
                Player.Cansaco += opcao1.Falha.AfetaCansaco;
                Player.DanoAcumulado += opcao1.Falha.AfetaDano;

                FechaPop(popUpGenerico);
            }
        });

        btnDireita.onClick.AddListener(() =>
        {
            var opcao2 = evento.Opcao2;
            var chanceSucesso = opcao2.ChanceSucesso;
            if (Random.Range(1, 101) <= chanceSucesso) //Caso haja sucesso
            {
                //texto do outcome
                ResumosTexto.Instance.ResumoEventoOutcome(opcao2.Sucesso.TextoEfeito);

                //aplica efeitos
                Player.AprovTreinador += opcao2.Sucesso.AfetaAprov;
                Player.Reputacao += opcao2.Sucesso.AfetaReputacao;
                Player.JStats.GolosTurno += opcao2.Sucesso.AfetaGolos;
                Player.JStats.AssistTurno += opcao2.Sucesso.AfetaAssists;
                Player.JStats.DefesaTurno += opcao2.Sucesso.AfetaDefesas;
                Player.JAtributos.Forca += opcao2.Sucesso.AfetaForca;
                Player.JAtributos.Rapidez += opcao2.Sucesso.AfetaRapidez;
                Player.JAtributos.Remate += opcao2.Sucesso.AfetaRemate;
                Player.JAtributos.Passe += opcao2.Sucesso.AfetaPasse;
                Player.JAtributos.Defesa += opcao2.Sucesso.AfetaDefesaAtributo;
                Player.Cansaco += opcao2.Sucesso.AfetaCansaco;
                Player.DanoAcumulado += opcao2.Sucesso.AfetaDano;

                FechaPop(popUpGenerico);
            }
            else//caso de falha
            {

                ResumosTexto.Instance.ResumoEventoOutcome(opcao2.Falha.TextoEfeito);

                Player.AprovTreinador += opcao2.Falha.AfetaAprov;
                Player.Reputacao += opcao2.Falha.AfetaReputacao;
                Player.JStats.GolosTurno += opcao2.Falha.AfetaGolos;
                Player.JStats.AssistTurno += opcao2.Falha.AfetaAssists;
                Player.JStats.DefesaTurno += opcao2.Falha.AfetaDefesas;
                Player.JAtributos.Forca += opcao2.Falha.AfetaForca;
                Player.JAtributos.Rapidez += opcao2.Falha.AfetaRapidez;
                Player.JAtributos.Remate += opcao2.Falha.AfetaRemate;
                Player.JAtributos.Passe += opcao2.Falha.AfetaPasse;
                Player.JAtributos.Defesa += opcao2.Falha.AfetaDefesaAtributo;
                Player.Cansaco += opcao2.Falha.AfetaCansaco;
                Player.DanoAcumulado += opcao2.Falha.AfetaDano;

                FechaPop(popUpGenerico);
            }
        });

        AbrePop(popUpGenerico);

    }

    public void PopUpTrofeu(string trofeu)
    {
        var Player = PlayerManager.Instance.Player;

        switch (trofeu)
        {
            case "taça":
                {
                    CorpoTrofeu.text = $"Parabéns {Player.Nome}, a tua equipa ganhou a taça nacional!";
                    ImagemTrofeu.sprite = taca;
                    break;
                }
            case "liga":
                {
                    CorpoTrofeu.text = $"Parabéns {Player.Nome}, após uma longa temporada, a tua equipa foi coroada campeã!";
                    ImagemTrofeu.sprite = liga;
                    break;
                }
            case "champions":
                {
                    CorpoTrofeu.text = $"Parabéns {Player.Nome}, tu e atua equipa fizeram história, voçês são campeões da europa!";
                    ImagemTrofeu.sprite = champions;
                    break;
                }
        }

        AbrePop(popUpTrofeu);
    }
}
