using Firebase.Auth;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ResumosTexto : MonoBehaviour //contem todas as funções que atualizam o texto do menu principal,
{
    public TMP_Text Resumo;

    public static ResumosTexto Instance { get; private set; }

    private Jogador jogador;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public void AtualizaResumoTurno(string mes)//todos os turnos mostra os golos e assists
    {
        jogador = PlayerManager.Instance.Player;

        Resumo.text += $"\n{mes}:";
        Resumo.text += $"\n    Fizeste {(int)jogador.JStats.GolosTurno} golos,";
        Resumo.text += $"\n    {(int)jogador.JStats.AssistTurno} assistencias,";
        Resumo.text += $"\n    {(int)jogador.JStats.DefesaTurno} defesas!";
        limpaResumo();
    }
    public void AtualizaResumoEpoca(string mes)//no final da época diz o total de stats
    {
        jogador = PlayerManager.Instance.Player;
        Resumo.text += $"\n{mes}:";
        Resumo.text += $"\n    Acabou a época, marcaste {jogador.JStats.NumGolos} golos e fizeste {jogador.JStats.NumAssistencias} assistencias";
        Resumo.text += $"\n    Estás de férias!";
        limpaResumo();
    }
    public void AtualizaResumoFerias(string mes)//diz que está de ferias
    {
        Resumo.text += $"\n{mes}:";
        Resumo.text += $"\n    Estas de férias!";
        limpaResumo();
    }

    #region Transferencia de Clubes
    public void ResumoMudaClube()//resumo quando jogador muda de clube
    {
        jogador = PlayerManager.Instance.Player;
        Resumo.text += $"\n    Chegaste a um acordo com o {jogador.Clube} com um contrato de {jogador.Contrato} anos!";

        int? CapClube = jogador.RetornaClube()?.CapcidadeBase;
        int Cap = jogador.GeraMedia();

        //mensagem dinamica que depende da capaciade do jogador
        if (CapClube == null)
        {
            return;
        }
        if (CapClube > Cap)
        {
            Resumo.text += $"\n    Bem-vindo ao {jogador.Clube}! É um passo grande para a tua carreira.";
        }
        else if (CapClube - 5 < Cap && CapClube + 5 > Cap)
        {
            Resumo.text += $"\n    Bem-vindo ao {jogador.Clube}! O clube está feliz por te ter aqui.";
        }
        else
        {
            Resumo.text += $"\n    Bem-vindo ao {jogador.Clube}! É uma honra dizer que fazes parte da história do clube!";
        }
        limpaResumo();
    }

    public void ResumoPlayerForcadoSair() //quando o player é vendido devido a baica aprovação
    {
        jogador = PlayerManager.Instance.Player;
        Resumo.text += $"\n    O teu treinador perdeu fé em ti, e decidiu vender-te ao {jogador.Clube}.\nTalvez tenhas mais sorte aqui.";
        limpaResumo();
    }

    public void ResumoPlayerPermanece(string clube) //quando o player recusa a oferta de sair
    {
        Resumo.text += $"\n    Recusate uma saida para o {clube}, mesmo depois do treinador dizer que não tem espaço para ti. Ele não gostou disso...";
        limpaResumo();
    }

    #endregion
    public void ResumoRenovacao(bool Sucesso)
    {
        var player = PlayerManager.Instance.Player;
        if (Sucesso)
        {
            Resumo.text += $"\n    Parabéns, chegaste a um acordo com o {player.Clube} e renovaste o teu contrato por {player.Contrato} anos!";
        }
        else
        {
            Resumo.text += $"\n    Não conseguiste chegar a um acordo com o {player.Clube}, agora estás em risco de te tornares uma agente livre!";
        }
        limpaResumo();
    }

    public void ResumoRenovacaoNaoOferecida()
    {
        Resumo.text += $"\n    O teu contrato está quase a acabar, mas nem o presidente nem o treinador disseram algo...";
        limpaResumo();
    }

    public void ResumoAgenteLivre(string mes)
    {
        Resumo.text += $"\n{mes}:";
        Resumo.text += $"\n    Estás sem clube!";
        limpaResumo();
    }

    public void ResumoLesao(string mes)
    {
        Resumo.text += $"\n{mes}:";
        Resumo.text += $"\n    Estás lesionado";
        limpaResumo();
    }

    public void ResumoEventoOutcome(string outcome)//apresenta um outcome de um evento
    {
        Resumo.text += $"\n    {outcome}";
        limpaResumo();
    }

    public void ResumoTrofeu(string trofeu)
    {
        switch (trofeu)
        {
            case "champions":
                {
                    Resumo.text += $"\n    A tua equipa fez história e ganharam a Liga dos Campeões!";
                    limpaResumo();
                    break;
                }
            case "liga":
                {
                    Resumo.text += $"\n    Após uma longa temporada, a tua equipa foi titulada de campeã!";
                    limpaResumo();
                    break;
                }
            case "taça":
                {
                    Resumo.text += $"\n     A tua equipa ganhou a taça nacional!";
                    limpaResumo();
                    break;
                }
        }
    }

    private void limpaResumo()//limpa o resumo se for maior que 15 linhas
    {
        var linhas = Resumo.text.Split('\n').ToList();

        while (linhas.Count > 16)
            linhas.RemoveAt(0); // remove a linha mais antiga

        Resumo.text = string.Join("\n", linhas);
    }
}
