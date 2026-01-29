using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public class TurnoManager : MonoBehaviour //Contem a funÁ„o que simula um turno
{
    //gere os turnos
    public TMP_Text txtTurno;
    public TMP_Text txtPontuacao;
    public static TurnoManager Instance;
    public GameObject btnSimular;

    public Turno turno = new Turno(0, 0);

    private bool PodeSimular = true; //define se um turno pode ser simulado

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Simula() //simula um turno
    {
        //caso um popup esteja ativo, proibe simular o turno
        if (!PodeSimular)
        {
            
            return;
        }
        PodeSimular = false;
        LeanTween.rotateZ(btnSimular, 180, 0.1f); //adiciona uma ligeira rotaÁ„o ao bot„o de simulaÁ„o
        LeanTween.rotateZ(btnSimular, 0, 0.1f);

        turno.AvancaTurno();
        JanelaTransferencia();

        var Player = PlayerManager.Instance.Player;

        if (Player.MesRepouso > 0)//Se est· lesionado
        {
            if(turno.TurnoNum == 10) FinalEpoca();
            else
            {
                ResumosTexto.Instance.ResumoLesao(turno.Mes[turno.TurnoNum]);
            }
                Player.Reputacao -= 5;
            VerificaSeTreinou();
            turno.isTreinou = false;
            txtTurno.text = $"{turno.Mes[turno.TurnoNum]}\n…poca {turno.Epoca}\n{Player.Idade} Anos\nPRIME: {Player.Prime}";
            PodeSimular = true;
            return;
        }

        if (Player.Contrato <= 0)//caso seja um agente livre
        {
            Player.Reputacao -= 20;
            VerificaSeTreinou();
            turno.isTreinou = false;
            ResumosTexto.Instance.ResumoAgenteLivre(turno.Mes[turno.TurnoNum]);
            txtTurno.text = $"{turno.Mes[turno.TurnoNum]}\n…poca {turno.Epoca}\n{Player.Idade} Anos\nPRIME: {Player.Prime}";
            PodeSimular = true;
            return;

        }

        switch (turno.TurnoNum)
        {
            case 10://caso acabe a Època
                {
                    FinalEpoca();

                    VerificaLiga();

                    Player.CalculaAprov();//calcula a aprovaÁ„o do treinador
                    VerificaSeTreinou();
                    GeraEventoAleatorio();
                    VerificaLesao();
                    break;
                }
            case int v when (v >= 0 && v <= 9)://caso seja um int, guarda em v, testa v-- caso seja um turno normal
                {
                    TurnoNormal();

                    //verifica se ganhou trofeus
                    if (turno.Mes[turno.TurnoNum] == "Abril") VerificaTaca();
                    if (turno.Mes[turno.TurnoNum] == "Maio") VerificaChampions();

                    VerificaSeTreinou();
                    VerificaLesao();
                    GeraEventoAleatorio();
                    Player.CalculaAprov();
                    break;
                }
            default://no default est·s de fÈrias
                {
                    VerificaSeTreinou();
                    ResumosTexto.Instance.AtualizaResumoFerias(turno.Mes[turno.TurnoNum]);
                    break;
                }
        }

        txtPontuacao.text = $"PontuaÁ„o: {Math.Round(Player.JStats.Pontuacao, 2)}";
        OferecidoAClube();
        txtTurno.text = $"{turno.Mes[turno.TurnoNum]}\n…poca {turno.Epoca}\n{Player.Idade} Anos\nPRIME: {Player.Prime}";
        PodeSimular = true;
    }
    //atualizar texto
    public void AtualizaTurnoTxt()
    {
        var Player = PlayerManager.Instance.Player;
        txtTurno.text = $"{turno.Mes[turno.TurnoNum]}\n…poca {turno.Epoca}\n{Player.Idade} Anos\nPRIME: {Player.Prime}";
    }
    //--------FUN«’ES PRIVADAS-------------------------
    private void GeraEventoAleatorio()
    {
        if (PlayerManager.Instance.Player.MesRepouso > 0) return;//se estiver lesionado, n„o geres eventos
        if (EventosManager.Instance == null) Debug.Log("Evento");
        var evento = EventosManager.Instance.BuscaEvento();

        if (evento == null) return;

        PopUpManager.Instance.PopUpEventoAleatorio(evento);
    }


    private void OferecidoAClube()// o jogador È oferecido a um clube
    {
        var Player = PlayerManager.Instance.Player;

        if (!turno.IsJanelaTransf) return; //caso n„o seja uma janela de tranferencias

        if (Player.RetornaClube().ReputacaoBase <= 200) return; //caso o clube for pequeno, n„o tenta te vender

        if (Player.AprovTreinador < 40)//se a reputaÁ„o È baixa e estamos numa janela de transferencias
        {
            PopUpManager.Instance.OferecidoAClube();
        }

        
    }

    private void JanelaTransferencia()//verifica se estamos na janela de tranferencias
    {
        var Player = PlayerManager.Instance.Player;
        //se calhar È mau perguntar todos os santos turnos se estou numa janela de transferencias
        var janela = new string[4] { "Janeiro", "Junho", "Julho", "Agosto" };
        if (janela.Contains(turno.Mes[turno.TurnoNum]))
        {
            turno.IsJanelaTransf = true;
            return;
        }
        if (Player.Contrato <= 0)//se o jogador n„o tem contrato pode ir para qualquer equipa
        {
            turno.IsJanelaTransf = true;
            return;
        }
        turno.IsJanelaTransf = false;
    }

    private void FinalEpoca() //chamado no final de uma Època -- chama a funÁ„o de renovaÁ„o caso tenha boa aprovaÁ„o
    {
        var Player = PlayerManager.Instance.Player;
        Player.Envelhece();
        ResumosTexto.Instance.AtualizaResumoEpoca(turno.Mes[turno.TurnoNum]);
        PlayerManager.Instance.Player.turno = turno;

        if (DadosScene.IsAutenticado) _ = PlayerManager.Instance.AtualizaPlayer();

        Player.JAtributos.LimpaEstatisticas(Player);


        if (Player.Contrato == 1)
        {
            if (Player.AprovTreinador < 40) //se a aprovaÁ„o for baixa, o contrato n„o È oferecido
            {
                ResumosTexto.Instance.ResumoRenovacaoNaoOferecida();
                return;
            }

            NegociarManager.Instance.RenovarContrato(false);

        }
        else if (Player.Contrato == 0)
        {

            if (Player.AprovTreinador < 40) //se a aprovaÁ„o for baixa, o contrato n„o È oferecido
            {
                ResumosTexto.Instance.ResumoRenovacaoNaoOferecida();
                return;
            }

            NegociarManager.Instance.RenovarContrato(true);
        }
    }

    private void TurnoNormal() //um turno normal onde estatisticas s„o geradas
    {
        var Player = PlayerManager.Instance.Player;
        Player.JAtributos.GeraEstatisticas(PlayerManager.Instance.Player);
        ResumosTexto.Instance.AtualizaResumoTurno(turno.Mes[turno.TurnoNum]);

        //limpa stats de turno
        Player.JStats.GolosTurno = 0;
        Player.JStats.AssistTurno = 0;
        Player.JStats.DefesaTurno = 0;
    }

    //tenho de ter 3 funÁoes para chama-las a tempos diferentes
    //Se chamasse tudo ao mesmo tempo e o jogador ganhasse 2 ou mais trofeus, sÛ 1 iria ser mostrado
    #region trofeus
    private void VerificaTaca()//verifica se ganhou taÁa nacional
    {
        if (TrofeusManager.Instance.GanhouTaca())
        {
            PopUpManager.Instance.PopUpTrofeu("taÁa");
            ResumosTexto.Instance.ResumoTrofeu("taÁa");
        }
    }

    private void VerificaLiga()//verifica se ganhou taÁa nacional
    {
        if (TrofeusManager.Instance.GanhouLiga())
        {
            PopUpManager.Instance.PopUpTrofeu("liga");
            ResumosTexto.Instance.ResumoTrofeu("liga");
        }
    }
    private void VerificaChampions()//verifica se ganhou taÁa nacional
    {
        if (TrofeusManager.Instance.GanhouChampions())
        {
            PopUpManager.Instance.PopUpTrofeu("champions");
            ResumosTexto.Instance.ResumoTrofeu("champions");
        }
    }
    #endregion 

    private void VerificaSeTreinou() //verifica se treinou, se n„o, descansa
    {
        if (!turno.isTreinou)
        {
            PlayerManager.Instance.Player.DescansaTurno();
            BarrasManager.Instance.AtualizaBarras();
        }
        turno.isTreinou = false;
    }

    private void VerificaLesao()
    {
        var Player = PlayerManager.Instance.Player;

        if (Player.MesRepouso > 0) return; //se est· lesionado, n„o se pode lesionar
        Debug.Log("chance " + (Player.ChanceLesao + ((Player.DanoAcumulado /10) * 2)));
        bool IsLesao = Player.GeraLesao();
        if (IsLesao) //se lesionou-se, chama um popup
        {
            PopUpManager.Instance.PopUpLesao();
        }
    }
}
