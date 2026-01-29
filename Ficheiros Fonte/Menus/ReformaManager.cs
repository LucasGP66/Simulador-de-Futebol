using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.X86;

//gere o menu de reforma
public class ReformaManager : MonoBehaviour//gere a reforma do jogador
{
    public TMP_Text txtConvencer;
    public TMP_Text txtResumo;
    public GameObject MenuReforma;
    public void txtReforma()//atualiza um pequeno texto quando o jogar está prestes a reformar-se
    {
        if (PlayerManager.Instance.Player.Idade > 32)
        {
            txtConvencer.text = "Entende-se, as tuas pernas já não são o que eram, a tua fome já está saciada. Os treinos são duros, e os jogos longos. Talvez seja melhor abandonar o jogo...";
        }
        else
        {
            txtConvencer.text = "Ainda és jovem, e tens muito para dar. Ainda podes fazer muito mais com a tua carreira. Não desistas!";
        }
    }

    public void CriarResumo()
    {
        Jogador player = PlayerManager.Instance.Player;
        Turno turno = TurnoManager.Instance.turno;


        var historico = player.JStats.ClubesLista;

        txtResumo.text = $"Jogaste por {turno.Epoca + 1} épocas, marcaste um total de {player.JStats.TotalGolos} golos, fizeste {player.JStats.TotalAssists} assistencias {player.JStats.TotalDefesas} defesas. ";

        if (historico.Count == 1)
        {
            txtResumo.text += $"\nJogaste toda a tua carreira com as cores do {historico[0]}. Jamais serás esquecido.";
        }
        else
        {
            txtResumo.text += "Na tua carreira passeste por";
            foreach (var clube in historico)//loop que adiciona os clubes ao texto, com pequenos detalhes --- ainda falta fazer isto melhor
            {
                if (clube == historico[historico.Count() - 2]) txtResumo.text += $" {clube} ";
                else if (clube == historico.Last()) txtResumo.text += $"e {clube}.";
                else
                {
                    txtResumo.text += $" {clube},";
                }
            }
        }

        txtResumo.text += "\nFoi uma boa carreira.";
    }


    public void Reformar()
    {
        DadosScene.IsReforma = true;
        SceneManager.LoadScene("Login");
    }
}
