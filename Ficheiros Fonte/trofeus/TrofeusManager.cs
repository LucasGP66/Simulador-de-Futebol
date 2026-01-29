using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
public class TrofeusManager : MonoBehaviour //contem funções que vêm se o jogador ganhou trofeus de equipa
{
    public LigasDB Ligas;
    public static TrofeusManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public bool GanhouLiga()//verifica se ganhou a liga
    {
        var Player = PlayerManager.Instance.Player;

        var ligaJogador = Player.RetornaLiga();

        var LigaPontModificada = new List<Clube>();//liga com clubes com capacidade base modificada

        //vai por cada clube, cria um clone, modifica a capacidade base
        foreach(var clube in ligaJogador.Clubes)
        {
            var ClubeNovo = clube.Clone();

            //se o jogador tiver uma média de capacidade maior do que a do clube, a a chance deles aumentam
            if (clube.Nome == Player.Clube && clube.CapcidadeBase < Player.GeraMedia())
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-10, 20);

            }
            else
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-15, 15);
            }

            LigaPontModificada.Add(ClubeNovo);
        }

        LigaPontModificada = LigaPontModificada.OrderBy(n => n.CapcidadeBase).ToList();

        Debug.Log(LigaPontModificada.Last().Nome);

        //o ultimo na lista é o vencedor, se é o clube do jogador, ele ganhou a liga
        if (LigaPontModificada.Last().Nome == Player.Clube)
        {
            return true;
        }

        return false;
    }

    public bool GanhouTaca()//verifica se ganhou taça nacional
    {
        var Player = PlayerManager.Instance.Player;

        var ligaJogador = Player.RetornaLiga();

        var LigaPontModificada = new List<Clube>();//liga com clubes com capacidade base modificada

        //vai por cada clube, cria um clone, modifica a capacidade base
        foreach (var clube in ligaJogador.Clubes)
        {
            var ClubeNovo = clube.Clone();

            //se o jogador tiver uma média de capacidade maior do que a do clube, a a chance deles aumentam
            if (clube.Nome == Player.Clube && clube.CapcidadeBase < Player.GeraMedia())
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-15, 25);

            }
            else
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-20, 20);
            }

            LigaPontModificada.Add(ClubeNovo);
        }

        LigaPontModificada = LigaPontModificada.OrderBy(n => n.CapcidadeBase).ToList();

        Debug.Log(LigaPontModificada.Last().Nome);

        //o ultimo na lista é o vencedor, se é o clube do jogador, ele ganhou a liga
        if (LigaPontModificada.Last().Nome == Player.Clube)
        {
            return true;
        }

        return false;
    }

   public bool GanhouChampions()//verifica se ganhou liga internacional
    {
        var Player = PlayerManager.Instance.Player;

        var LigaChampions = new List<Clube>();

        foreach(var liga in Ligas.Ligas)//por cada liga
        {
            var NovaLiga = liga.Clubes.OrderBy(n => n.CapcidadeBase).ToList();//ordena os clubes por capacidade
            LigaChampions.AddRange(NovaLiga.TakeLast(4));//pega os 4 melhores
        }

        var LigaTemp = new List<Clube>();

        foreach (var clube in LigaChampions)//cria uma nova lista de clubes comm capcidades alteradas
        {
            var ClubeNovo = clube.Clone();
            if (clube.Nome == Player.Clube && clube.CapcidadeBase < Player.GeraMedia())
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-15, 25);

            }
            else
            {
                ClubeNovo.CapcidadeBase = clube.CapcidadeBase + Random.Range(-20, 20);
            }

            LigaTemp.Add(ClubeNovo);
        }

        LigaTemp = LigaTemp.OrderBy(n => n.CapcidadeBase).ToList();
        if (LigaTemp.Last().Nome == Player.Clube) return true;

        return false;
    }
}
