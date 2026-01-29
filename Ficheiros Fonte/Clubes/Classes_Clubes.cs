using System;

[System.Serializable]
public class Clube
{
    //--------propriedades---------------------------
    public int ReputacaoBase;
    public int CapcidadeBase;
    public int Proposta;
    public string Nome;
    public int ultimaVezNegociado; //guarda a ultima vez que o clube nengociou com o jogador

    //--------construtores----------------------------
    public Clube() { }
    public Clube(string nome, int reputacao)
    {
        ReputacaoBase = reputacao;
        Nome = nome;
    }
    //----------Override------------------------------

    public override string ToString()
    {
        return Nome;
    }

    //--------metodos----------------------------------

    public void GeraProposta(Jogador player, int Epoca) //gera uma duração de contrato para dar ao jogador
    {
        //guarda a epoca onde esta proposta foi feita
        ultimaVezNegociado = Epoca;

        //gerar anos base de contrato
        int anosBase = 0;
        int media = player.GeraMedia();
        float pontuacao = player.JStats.PontuacaoEpocaPassada;
        if (media > CapcidadeBase)// se o player tem uma capacidade maior que a necessária
        {
            anosBase = UnityEngine.Random.Range(3, 5);
        }
        else if (media > CapcidadeBase - 5)//se o player tiver uma capacidade 5 pontos mais baixa que a necessária
        {
            anosBase = UnityEngine.Random.Range(2, 3);
        }
        else
        {
            anosBase = 1;
        }

        //modificador do contrato por base de anos
        if (player.Idade < 25)
        {
            anosBase += 2;
        }
        else if (player.Idade > 32)
        {
            anosBase -= 2;
        }

        //verifica o desempenho do jogador
        if (pontuacao > 7.5)
        {
            anosBase += 1;
        }
        else if (pontuacao < 6.7)
        {
            anosBase -= 1;
        }

        if (anosBase > 5) Proposta = 5;//limite de 5 anos
        else if (anosBase <= 0) Proposta = 1;
        else Proposta = anosBase;
    }



    public bool ContraProposta(int contraP, out int ContraPClube)// verificca uma contra proposta do jogador
    {
        int diferenca = Math.Abs(Proposta - contraP);
        if (diferenca! > 2)//se a proposta não for maior que 2, aceita
        {
            ContraPClube = contraP;
            Proposta = contraP;
            return true;
        }
        else
        {
            int chanceEvento = UnityEngine.Random.Range(1, 5);
            if (chanceEvento == 1)//se o clube desiste
            {
                ContraPClube = 0;
                Proposta = -1;
                return false;
            }
            else if (chanceEvento > 3)//chance de eles negociarem
            {
                ContraPClube = (Proposta + contraP) / 2;
                return false;
            }
            else//clube aceita
            {
                ContraPClube = contraP;
                Proposta = contraP;
                return true;
            }
        }
    }

    public Clube Clone()//retorna um clone do clube
    {
        return new Clube
        {
            ReputacaoBase = this.ReputacaoBase,
            CapcidadeBase = this.CapcidadeBase,
            Proposta = this.Proposta,
            Nome = this.Nome,
            ultimaVezNegociado = this.ultimaVezNegociado
        };
    }
}
