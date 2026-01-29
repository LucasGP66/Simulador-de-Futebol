using System;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PerfilManager : MonoBehaviour//gere o menu de perfil
{
    public TMP_Text Nome;
    public TMP_Text Clube;
    public TMP_Text Golos;
    public TMP_Text Assistencias;

    public GameObject Row;
    public Transform Tabela;

    public void atualizaPerfil()
    {
        var player = PlayerManager.Instance.Player;
        Nome.text = $"Nome: {player.Nome}";
        if (player.Contrato <= 0)
        {
            Clube.text = "Agente Livre";
        }
        else
        {
            Clube.text = $"Clube: {player.Clube}";
        }  
        Golos.text = $"{player.JStats.NumGolos} Golos";
        Assistencias.text = $"{player.JStats.NumAssistencias} Assistencias";

        //a sequencia de valores na string do historico do jogador é a seguinte: clube, golos, assists, defesas

        int keep = 4;
        int index = 0;
        foreach (var celula in Tabela) //limpa a tabela antes de a criar, mantendo as 4 primeiras celulas, ou seja o cabecalho
        {
            if (index >= keep)
            {
                Destroy(Tabela.GetChild(index).gameObject);
            }
            index++;
        }

        foreach (var linha in PlayerManager.Instance.Player.JStats.StatsHistorico)//cria uma tabela com o historico do jogador
        {

            var clube = Instantiate(Row, Tabela);
            var golos = Instantiate(Row, Tabela);
            var assists = Instantiate(Row, Tabela);
            var defesas = Instantiate(Row, Tabela);
            var dados = linha.Split('|');
            clube.GetComponentInChildren<TMP_Text>().text = dados[0];
            golos.GetComponentInChildren<TMP_Text>().text = dados[1];
            assists.GetComponentInChildren<TMP_Text>().text = dados[2];
            defesas.GetComponentInChildren<TMP_Text>().text = dados[3];
            
        }
    }

}
