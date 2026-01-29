using UnityEngine;

[System.Serializable]
public class EventosSO//define um evento
{
    public string Titulo;
    [TextArea] public string Corpo;
    public string TriggerID;
    public Opcao Opcao1;
    public Opcao Opcao2;

}

[System.Serializable]
public class Opcao//opção de um botão
{
    public string OpcaoNome;
    public int ChanceSucesso;
    public EfeitoNoJogador Sucesso;
    public EfeitoNoJogador Falha;
}

[System.Serializable]
public class EfeitoNoJogador//efeitos aplicados no jogador
{
    public string TextoEfeito;
    public int AfetaInteligencia;
    public int AfetaReputacao;
    public int AfetaAprov;
    public int AfetaGolos;
    public int AfetaAssists;
    public int AfetaDefesas;
    public int AfetaForca;
    public int AfetaRapidez;
    public int AfetaRemate;
    public int AfetaPasse;
    public int AfetaDefesaAtributo;
    public int AfetaCansaco;
    public int AfetaDano;

}
