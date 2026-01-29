using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TreinarManager : MonoBehaviour//permite que o jogador treine
{
    public Transform LinhaDefesa;
    public Transform LinhaRemate;
    public Transform LinhaPasse;
    public Transform LinhaFisico;
    public GameObject estrela;
    public void TreinarRemate(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    {
        if (PlayerManager.Instance.Player == null) Debug.Log("Player é null");
       
        TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaREMATE(modificador);
        if (TurnoManager.Instance.turno.isTreinou)
        {
            BarrasManager.Instance.AtualizaBarras();
            AtualizaEstrelasRemate();
        }
    }
    public void TreinarPasse(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    {

        TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaPASSE(modificador);
        if (TurnoManager.Instance.turno.isTreinou)
        {
            BarrasManager.Instance.AtualizaBarras();
            AtualizaEstrelasPasse();
        }
    }
    public void TreinarDefesa(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    {

        TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaDEFESA(modificador);
        if (TurnoManager.Instance.turno.isTreinou)
        {
            BarrasManager.Instance.AtualizaBarras();
            AtualizaEstrelasDefesa();
        }
        
    }
    public void TreinarRapidez(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    {

        TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaRAPIDEZ(modificador);
        if (TurnoManager.Instance.turno.isTreinou)
        {
            BarrasManager.Instance.AtualizaBarras();
            AtualizaEstrelasFisico();
        }
    }
    public void TreinarForca(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    {

        TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaFORCA(modificador);
        if (TurnoManager.Instance.turno.isTreinou)
        {
            BarrasManager.Instance.AtualizaBarras();
            AtualizaEstrelasFisico();
        }
    }
    //public void TreinarInteligencia(int modificador) //envia um bool para turno manager, serve para ver se um treino ocorreu neste turno.
    //{

    //    TurnoManager.Instance.turno.isTreinou = PlayerManager.Instance.Player.TreinaDEFESA(modificador);
    //    if (TurnoManager.Instance.turno.isTreinou)
    //    {
    //        BarrasManager.Instance.AtualizaBarras();
    //    }
    //}

    public void AtualizaEstrelasDefesa()//Adiciona estrelas de acordo com a defesa
    {
        var Player = PlayerManager.Instance.Player;

        int defesa = Player.JAtributos.Defesa / 10;

        foreach(Transform objeto in LinhaDefesa)
        {
            Destroy(objeto.gameObject);
        }

        for (int i = 0; i <  defesa; i++)
        {
            Instantiate(estrela, LinhaDefesa);
        }
    }

    public void AtualizaEstrelasRemate()//Adiciona estrelas de acordo com a remate
    {
        var Player = PlayerManager.Instance.Player;

        int remate = Player.JAtributos.Remate / 10;

        foreach (Transform objeto in LinhaRemate)
        {
            Destroy(objeto.gameObject);
        }

        for (int i = 0; i < remate; i++)
        {
            Instantiate(estrela, LinhaRemate);
        }
    }

    public void AtualizaEstrelasPasse()//Adiciona estrelas de acordo com o passe
    {
        var Player = PlayerManager.Instance.Player;

        int Passe = Player.JAtributos.Passe / 10;

        foreach (Transform objeto in LinhaPasse)
        {
            Destroy(objeto.gameObject);
        }

        for (int i = 0; i < Passe; i++)
        {
            Instantiate(estrela, LinhaPasse);
        }
    }

    public void AtualizaEstrelasFisico()//Adiciona estrelas de acordo com a fisicalidade
    {
        var Player = PlayerManager.Instance.Player;

        int fisico = (Player.JAtributos.Forca / 10 + Player.JAtributos.Rapidez /10)/2;

        foreach (Transform objeto in LinhaFisico)
        {
            Destroy(objeto.gameObject);
        }

        for (int i = 0; i < fisico; i++)
        {
            Instantiate(estrela, LinhaFisico);
        }
    }
}
