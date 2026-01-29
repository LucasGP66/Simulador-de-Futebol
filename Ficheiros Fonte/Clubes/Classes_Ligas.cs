using UnityEngine;

[System.Serializable]
public class Liga//define uma liga
{
    public string Nome;
    public Clube[] Clubes;
    public float MultReputacao;

    public Liga() { }

    public Liga(string nome, Clube[] clubes, int multReputacao)
    {
        Nome = nome;
        Clubes = clubes;
        MultReputacao = multReputacao;
    }
}