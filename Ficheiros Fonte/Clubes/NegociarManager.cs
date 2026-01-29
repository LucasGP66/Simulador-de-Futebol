using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NegociarManager : MonoBehaviour //permite a negociação com clubes, e a renovação de contrato atraves de pop up ou menu de carreira
{
    public TMP_Dropdown drpClubeCarreira;
    public TMP_Dropdown drpLiga;
    public TMP_Text Titulo;
    public TMP_Text txtOferta;
    public TMP_Text txtAnos;

    public TMP_Text txtRenovacao;

    public Slider sliderAnos;

    public Button btnNegociar;

    public GameObject painelNegociarAtivo;
    public GameObject painelNegociarFechado;

    public GameObject painelRenovar;

    private Clube ClubeANegociar;

    public static NegociarManager Instance;

    private bool IsRenovacao = false;

    public void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SliderTxt() //ajusta um txt automaticamente
    {
        txtAnos.text = sliderAnos.value.ToString() + " Anos";
    }

    public void AbrirJanelaNegociar()//função para executar ao começar as negociaçoes
    {
        if (!TurnoManager.Instance.turno.IsJanelaTransf) 
        {
            PopUpManager.Instance.MostraMensagem("A janela de transferencias não está aberta", 3f);
            painelNegociarAtivo.SetActive(false);
            PopUpManager.Instance.AbrePop(painelNegociarFechado);
            return;
        }
        if (drpClubeCarreira.options.Count == 0)//se não há clubes interessados, o dropdown não tem valores - um clube não é escolhido
        {
            PopUpManager.Instance.MostraMensagem("Tens de escolher um clube!", 3f);
            painelNegociarAtivo.SetActive(false);
            PopUpManager.Instance.AbrePop(painelNegociarFechado);
            return;
        }
        painelNegociarFechado.SetActive(false);
        PopUpManager.Instance.AbrePop(painelNegociarAtivo);
        IsRenovacao = false;
        Negociar();
    }

    public void AbrirJanelaNegociarRenovar()//começa renovações de contrato
    {
        if (PlayerManager.Instance.Player.Contrato > 0 && PlayerManager.Instance.Player.AprovTreinador >= 40) //se tem um contrato e tem aprovação suficiente
        {
            PopUpManager.Instance.AbrePop(painelNegociarAtivo);
            painelNegociarFechado.SetActive(false);
            IsRenovacao = true;
            RenovarCarreira();
        }
        else if (PlayerManager.Instance.Player.Contrato <= 0)
        {
            PopUpManager.Instance.MostraMensagem("És um agente livre, não tens clube.", 3f);
        }
        else
        {
            PopUpManager.Instance.MostraMensagem("A direção não está disposta a renovar o teu contrato", 3f);
        }

    }

    #region Negociacoes
    public void Negociar ()// gera uma prosta de contrato entre 1 a 5 anos
    {
        //obtem o clube a partir da string
        string clubeNome = drpClubeCarreira.options[drpClubeCarreira.value].text;
        int EpocaAtual = TurnoManager.Instance.turno.Epoca;

        ClubeANegociar = ClubeManager.Instance.clubesInterassados.First(c => c.Nome == clubeNome);
        Titulo.text = $"Proposta e contrato de {ClubeANegociar.Nome}";

        //verifica se o clube ja apresentou uma proposta, se as negociações falharam antes se está disponivel
        if (ClubeANegociar.Proposta == -1 && ClubeANegociar.ultimaVezNegociado == EpocaAtual) //se a proposta é -1 é porque já foi negociada e falhou
        {
            txtOferta.text = $"As negociações falharam, e o {ClubeANegociar.Nome} não está desposto a oferecer-te um contrato";
        }
        else if (ClubeANegociar.Proposta != 0 && ClubeANegociar.ultimaVezNegociado == EpocaAtual) //se não é 0, é porque uma proposta já foi gerada
        {
            txtOferta.text = $"Estamos disponiveis a oferecer um contrato de {ClubeANegociar.Proposta} anos.\r\nSerás visto como um jogador importante.";
        }
        else // Gera uma proposta nova e apresenta
        {
            ClubeANegociar.GeraProposta(PlayerManager.Instance.Player, EpocaAtual);
            txtOferta.text = $"Estamos disponiveis a oferecer um contrato de {ClubeANegociar.Proposta} anos.\r\nSerás visto como um jogador importante.";
        }  
    }

    public void AvaliaContraP()//avalia a contraproposta do jogador
    {
        if(!IsRenovacao)
        {
            string clubeNome = drpClubeCarreira.options[drpClubeCarreira.value].text;
            ClubeANegociar = ClubeManager.Instance.clubesInterassados.First(c => c.Nome == clubeNome);
            Titulo.text = $"Negociações com {ClubeANegociar.Nome}";
        }
        else
        {
            string clubeNome = PlayerManager.Instance.Player.Clube;
            ClubeANegociar = PlayerManager.Instance.Player.RetornaClube();
            Titulo.text = $"Renovação de contrato com {ClubeANegociar.Nome}";
        }

        if (ClubeANegociar.Proposta == -1) //se a proposta existente revela que as negociações falharam
        {
            return;
        }

        int contraP = (int)sliderAnos.value;
        int contraPClube;
        if (contraP == ClubeANegociar.Proposta) //se a contraporposta é igual a proposta, não fazes nada
        {
            return;
        }
        bool sucesso = ClubeANegociar.ContraProposta(contraP, out contraPClube);

        if (sucesso) //se o clube aceita a contra proposta
        {
            txtOferta.text = $"Aceitamos a sua proposta, um contrato de {contraP} anos serve. \r\nEstamos felizes por estas negociações serem concluidas com sucesso!";
        }
        else if (ClubeANegociar.Proposta == -1) //Se rejeita  acaba as negociações
        {
            txtOferta.text = $"Não podemos aceitar um contrato dessa duração. \r\nO {ClubeANegociar.Nome} perdeu o interesse na sua aquisição.";
        }
        else //Se oferecem uma proposta nova
        {
            txtOferta.text = $"Um contrato de {contraP} anos ainda é distante das nossas ideias \r\nEstamos prontos a oferecer um contrato de {contraPClube} anos.";
        }
    }

    public void AceitarProposta()//função para o botao de aceitar a proposta
    {
        var Player = PlayerManager.Instance.Player;
        if (ClubeANegociar.Proposta <= 0) return;//se a prosta for invalida, faz nada
        else
        {
            if (IsRenovacao)//se é uma renovacao
            {
                Player.Contrato = ClubeANegociar.Proposta; //a proposta é o novo contrato dele
                ResumosTexto.Instance.ResumoRenovacao(true); //apresenta um resumo no txt
            }
            else
            {
                Player.Clube = ClubeANegociar.Nome;//muda o clube no player
                Player.Contrato = ClubeANegociar.Proposta;
                Player.JStats.AdicionaClubeHistorico(ClubeANegociar.Nome);//adiciona o clube ao historico
                ResumosTexto.Instance.ResumoMudaClube();

                Player.DefineAprov();//define a aprovação do jogador
            }
        }

    }

    public void Recusar()//função que fecha janela de nogociações
    {
        painelNegociarAtivo.SetActive(false);
        painelNegociarFechado.SetActive(true);
    }
    #endregion

    #region renovar contratos

    #region pop up de renovacao
    public void RenovarContrato(bool IsAcabouContrato)//apresenta uma proposta de renovação ao jogador -- vem do pop up de renovacao
    {
        //painelRenovar.SetActive(true);
        PopUpManager.Instance.AbrePop(painelRenovar);
        var player = PlayerManager.Instance.Player;
        var clube = player.RetornaClube();

        if (IsAcabouContrato) //se o contrato esta prestes a acabar
        {
            txtRenovacao.text = $"Esta é a ultima chance de renovares o contrato, o presidente ofereceu-te um contrato de {clube.Proposta} anos.\r\n\r\nSe não aceitares a proposta, e até ao final da época não mudares de clube, tu iras te tornar um agente livre, e não jogaras enquanto não tiveres um clube.";
        }
        else
        {
            clube.GeraProposta(player, TurnoManager.Instance.turno.Epoca);
            txtRenovacao.text = $"Falta 1 ano para acabar o teu contrato, e o presidente ofereceu-te um contrato de {clube.Proposta} anos.\r\n\r\nSe não aceitares a proposta, e até ao final da época não mudares de clube, tu iras te tornar um agente livre, e não jogaras enquanto não tiveres um clube.";
        }
    }

    public void AceitarRenovacao()//aceita a proposta do clube -- vem do pop up de renovacao
    {
        var player = PlayerManager.Instance.Player;
        var clube = player.RetornaClube();

        PopUpManager.Instance.FechaPop(painelRenovar);
        player.Contrato = clube.Proposta; 
        ResumosTexto.Instance.ResumoRenovacao(true);
    }

    public void RejeitaRenovar()//rejeita a proposta de renovacao -- vem do pop up de renovacao
    {
        var player = PlayerManager.Instance.Player;
        PopUpManager.Instance.FechaPop(painelRenovar);
        if (player.Contrato == 0)
        {
            PopUpManager.Instance.MostraMensagem("O teu contrato acabou! És um agente livre, tens de encontrar um novo clube", 3f);
            return;
        }
        ResumosTexto.Instance.ResumoRenovacao(false);
    }
    #endregion

    public void RenovarCarreira()//função que permite o jogador renovar o contrato no menu carreira
    {
        PopUpManager.Instance.AbrePop(painelRenovar);
        int EpocaAtual = TurnoManager.Instance.turno.Epoca;

        ClubeANegociar = PlayerManager.Instance.Player.RetornaClube();
        Titulo.text = $"Renovação de contrato com {ClubeANegociar.Nome}";

        //verifica se o clube ja apresentou uma proposta, se as negociações falharam antes se está disponivel
        if (ClubeANegociar.Proposta == -1) //se a proposta é -1 é porque já foi negociada e falhou
        {
            txtOferta.text = $"As negociações falharam, e o {ClubeANegociar.Nome} não está desposto a oferecer-te um contrato";
        }
        else if (ClubeANegociar.Proposta != 0) //se não é 0, é porque uma proposta já foi gerada
        {
            txtOferta.text = $"Estamos disponiveis a oferecer um contrato de {ClubeANegociar.Proposta} anos.";
        }
        else // Gera uma proposta nova e apresenta
        {
            ClubeANegociar.GeraProposta(PlayerManager.Instance.Player, EpocaAtual);
            txtOferta.text = $"Estamos disponiveis a oferecer um contrato de {ClubeANegociar.Proposta} anos.";
        }
    }

    #endregion

}
