using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class ClubeManager : MonoBehaviour //gere os clubes, e os que aparecem nos dropdowns
{
    public TMP_Dropdown dropdownClubes;
    public TMP_Dropdown dropdownRep;

    public TMP_Dropdown drpClubeCarreira;
    public TMP_Dropdown drpLigas;

    public LigasDB Ligas;

    public static ClubeManager Instance;

    public List<Clube> clubesInterassados = new List<Clube>();
    private List<string> dropdownNomes = new List<string>();

    Jogador player;

    void Start()
    {
        Instance = this;
        //player = PlayerManager.Instance.Player;
        dropdownRep.onValueChanged.AddListener(AtualizaClubes);
        //drpClubeCarreira.onValueChanged.AddListener(AtualizaClubesCarreira);
        AtualizaClubes(0);
        AtualizaClubesCarreira();
    }

    private void AtualizaClubes(int index)
    {
        player = PlayerManager.Instance.Player;
        Debug.Log("A atualizar Clubes");

        //limpa a lista no dropdown
        dropdownClubes.ClearOptions();
        dropdownNomes.Clear();

        //vai buscar a reputacao escolhida do outro dropdown
        string repString = dropdownRep.options[dropdownRep.value].text;
        Debug.Log(repString);
        int rep = 0;

        switch (repString)
        {
            case "Baixa":
                {
                    rep = 300;
                    break;
                }
            case "Media":
                {
                    rep = 550;
                    break;
                }
            case "Alta":
                {
                    rep = 700;
                    break;
                }
        }

        //cria uma lista para clubes, e uma para o nome dos clubes

        //verifica quais clubes estão interessados
        foreach (var ligas in Ligas.Ligas)
        {
            if (ligas.Nome != "Primeira Liga Portuguesa") continue;
                foreach (var clube in ligas.Clubes)
            {
                if (clube.ReputacaoBase <= rep)
                {
                    clubesInterassados.Add(clube);
                    dropdownNomes.Add(clube.Nome);
                }
            }
        }
        dropdownNomes.Sort();
        //adiciona os clubes interessados
        dropdownClubes.AddOptions(dropdownNomes);
        dropdownClubes.value = 0;
        dropdownClubes.RefreshShownValue();

    }

    public void AtualizaClubesCarreira()
    {
        player = PlayerManager.Instance.Player;
        Debug.Log("A Atualizar os clubes interessados, rep " + player.Reputacao);
        clubesInterassados.Clear();
        dropdownNomes.Clear();

        foreach (var liga in Ligas.Ligas)
        {
            if (liga.Nome != drpLigas.options[drpLigas.value].text)//caso a liga não seja a do filtro
            {
                continue;
            }
            foreach (var clube in liga.Clubes)
            {
                if (clube.Nome == player.Clube) continue;//caso o clube seja o do jogador

                if (clube.ReputacaoBase - 200 <= player.Reputacao && clube.CapcidadeBase - 20 <= player.GeraMedia())// se a reputacao do clube for menor que a do player e a media dele for 10 pontos menor que a deles
                {
                    clubesInterassados.Add(clube);
                    dropdownNomes.Add(clube.Nome);
                }
            }
        }
        dropdownNomes.Sort();
        drpClubeCarreira.ClearOptions();

        drpClubeCarreira.AddOptions(dropdownNomes);

        drpClubeCarreira.value = 0;

        drpClubeCarreira.RefreshShownValue();
    }

}
