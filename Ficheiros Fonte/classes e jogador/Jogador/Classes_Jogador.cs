using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Rendering;
using UEngine = UnityEngine;

//Contem todas as classes

[FirestoreData]
[System.Serializable]
public class Jogador
{
    //-------Propriedades---------------------
    [FirestoreProperty] public string Nome { get; set; }
    [FirestoreProperty] public string Posicao { get; set; }//qual a posição do jogador
    [FirestoreProperty] public string Clube { get; set; }
    [FirestoreProperty] public int Contrato { get; set; }//quantos anos de contrato tem
    [FirestoreProperty] public int Reputacao { get; set; }
    [FirestoreProperty] public int Idade { get; set; }
    [FirestoreProperty] public int Prime { get; set; }//idade onde acaba o prime
    [FirestoreProperty] public int Cansaco { get; set; }
    [FirestoreProperty] public int ChanceLesao { get; set; }//chance de lesão - 1 a 75
    [FirestoreProperty] public int DanoAcumulado { get; set; }//dano no corpo do jogador acumulado ao longo do tempo- 0 a 100
    [FirestoreProperty] public string TipoLesao { get; set; }//leve, media, grave, muito grave ou ""
    [FirestoreProperty] public int MesRepouso { get; set; }//quantos meses fica de repouso
    [FirestoreProperty] public float AprovTreinador { get; set; }//Aprovação do treinador -> 0 a 100
    [FirestoreProperty] public Atributos JAtributos { get; set; }
    [FirestoreProperty] public Estatisticas JStats { get; set; }
    [FirestoreProperty] public Turno turno { get; set; }//apenas serve para guardar na bd as informações do turno

    //-------Construtores-----------------------
    public Jogador() { }
    public Jogador(string nome, string posicao, string clube, int reputacao)
    {
        Nome = nome;
        Posicao = posicao;
        Clube = clube;
        Reputacao = reputacao;
        Prime = UnityEngine.Random.Range(25, 37);
        Idade = 18;
        Cansaco = 20;
        Contrato = 5;
        AprovTreinador = 50;
        ChanceLesao = 1;
        DanoAcumulado = 0;
        MesRepouso = 0;
        TipoLesao = "";
        JStats = new Estatisticas
        {
            NumGolos = 0,
            NumAssistencias = 0,
            NumDefesas = 0,
            StatsHistorico = new List<string>(),
            AssistTurno = 0,
            GolosTurno = 0,
            DefesaTurno = 0,
            JogosEpoca = 0,
            PontuacaoList = new List<float> { 6.5f },
            ClubesLista = new List<string> { Clube }
        };
        JAtributos = new Atributos(Posicao);
        turno = new Turno(0, 0);
    }

    //---------Metodos---------------------------
    #region metodos
    public Clube RetornaClube()//retorna o objeto Clube do Jogador, tem de ser assim porque não é possível meter o clube na bd, acho eu
    {
        foreach (var liga in ClubeManager.Instance.Ligas.Ligas)
        {
            Clube clube = liga.Clubes.FirstOrDefault(c => c.Nome == Clube);
            if (clube != null)
                return clube;
        }
        return null;
    }

    public Liga RetornaLiga() //retorna a liga onde p jogador esta
    {
        foreach (var liga in ClubeManager.Instance.Ligas.Ligas)
        {
            if (liga.Clubes.Contains(RetornaClube()))
            {
                return liga;
            }
        }
        return null;
    }
    public void DescansaTurno() //metodo chamado todos os turnos se o jogador não treina, reduz o cansaço do jogador
    {
        Cansaco -= 20;
        MesRepouso = Math.Max(0, MesRepouso - 1); //reduz 1 ao mes repouso
        ChanceLesao = Math.Max(1, ChanceLesao - 3); //reduz a chance lesão por 3, não vai abaixo de 1
        if (Cansaco < 0)
        {
            Cansaco = 0;
        }
    }
    public void Envelhece()//envelhece o jogador, reduz atributos e reputação
    {
        Idade++;
        if (Idade > Prime)//isto é bastante mau, tens de encontrar uma maneira melhor
        {
            JAtributos.Forca -= (UnityEngine.Random.Range(0, 3) + Idade - Prime);
            if (JAtributos.Forca < 1) JAtributos.Forca = 1;

            JAtributos.Rapidez -= (UnityEngine.Random.Range(0, 3) + Idade - Prime);
            if (JAtributos.Rapidez < 1) JAtributos.Rapidez = 1;

            JAtributos.Remate -= (UnityEngine.Random.Range(0, 1) + Idade - Prime);
            if (JAtributos.Remate < 1) JAtributos.Remate = 1;

            JAtributos.Passe -= (UnityEngine.Random.Range(0, 1) + Idade - Prime);
            if (JAtributos.Passe < 1) JAtributos.Passe = 1;

            JAtributos.Defesa -= (UnityEngine.Random.Range(0, 1) + Idade - Prime);
            if (JAtributos.Defesa < 1) JAtributos.Defesa = 1;
        }

        //if (Idade > 35) Reputacao -= UnityEngine.Random.Range(100, 150);

        // diferença de anos * 20 + numero aleatorio
        else if (Idade > 32) Reputacao -= (Idade - 31) * 20 + UnityEngine.Random.Range(100, 151);
    }

    public void CalculaAprov()// calcula a aprovação do treinador
    {
        AprovTreinador += (int)(JStats.GolosTurno * 1.4 + JStats.AssistTurno * 1.4); //1 ponto por golo e assist

        if (JStats.DefesaTurno >= 10) AprovTreinador += 10;

        switch (JStats.Pontuacao)//verifica a pontuação do jogador
        {
            case >= 8: AprovTreinador += 5; break;

            case >= 7: AprovTreinador += 3; break;

            case >= 6.5f: AprovTreinador -= 1; break;

            case >= 6.0f: AprovTreinador -= 3; break;

            case <= 6f: AprovTreinador -= 7; break;
        }

        switch (AprovTreinador)//assegura que aprovação é de 0 a 100
        {
            case > 100: AprovTreinador = 100; break;

            case < 0: AprovTreinador = 0; break;

        }

        switch (AprovTreinador)// define multiplicador de tempo de jogo
        {
            case > 80: JAtributos.MultTempoDeJogo = 1.1f; break;

            case > 50: JAtributos.MultTempoDeJogo = 1f; break;

            case > 40: JAtributos.MultTempoDeJogo = 0.9f; break;

            default: JAtributos.MultTempoDeJogo = 0.6f; break;
        }
    }

    public void DefineAprov()// define a aprov do treinador quando o jogador muda de clube
    {
        AprovTreinador = 50 + Reputacao / 100;

        switch (AprovTreinador)
        {
            case > 80: AprovTreinador = 80; break;
        }
    }

    public Clube TreinadorTentaVender() //chamado quando o jogador tem pouca aprovação do treinador - escolhe um clube aleatorio para te oferecer
    {
        var clubesInteressados = new List<Clube>();

        int meuClubeRep = RetornaClube().ReputacaoBase;

        var ligas = ClubeManager.Instance.Ligas.Ligas;

        foreach (var liga in ligas)
        {
            foreach (var clube in liga.Clubes)
            {
                if (clube.Nome == Clube)
                {
                    continue;//não pode ser o clube do jogador
                }
                if (clube.ReputacaoBase - 200 < meuClubeRep)
                {
                    clubesInteressados.Add(clube);
                }
            }
        }
        if (clubesInteressados.Count == 0) return null;

        return clubesInteressados[UnityEngine.Random.Range(0, clubesInteressados.Count)];
    }

    public bool GeraLesao() //ve se o jogador se lesionou
    {
        //calcula a chance de se lesionar
        int dano = (DanoAcumulado / 10) * 2; // 2 pontos por cada 10
        int chance = UnityEngine.Random.Range(1, 101);

        if (chance > Math.Min(75, ChanceLesao + dano))//se não se lesionou
        {
            return false;
        }

        //verifica o tipo de lesão
        chance = UnityEngine.Random.Range(1, 11);
        switch (chance)
        {
            case 10: TipoLesao = "Muito Grave"; break;

            case >= 8: TipoLesao = "Grave"; break;

            case >= 5: TipoLesao = "Média"; break;

            default: TipoLesao = "Leve"; break;
        }

        //aplica efeitos
        switch (TipoLesao)
        {
            case "Muito Grave":
                {
                    DanoAcumulado += 20;
                    Cansaco = 100;
                    MesRepouso = UnityEngine.Random.Range(8, 13);
                    JAtributos.Rapidez = Math.Max(1, JAtributos.Rapidez -= 20);
                    JAtributos.Forca = Math.Max(1, JAtributos.Forca -= 20);
                    JAtributos.Remate = Math.Max(1, JAtributos.Remate -= 20);
                    JAtributos.Defesa = Math.Max(1, JAtributos.Defesa -= 20);
                    JAtributos.Passe = Math.Max(1, JAtributos.Passe -= 20);
                    break;
                }
            case "Grave":
                {
                    DanoAcumulado += 10;
                    Cansaco = 100;
                    MesRepouso = UnityEngine.Random.Range(2, 5);
                    JAtributos.Rapidez = Math.Max(1, JAtributos.Rapidez -= 10);
                    JAtributos.Forca = Math.Max(1, JAtributos.Forca -= 10);
                    JAtributos.Remate = Math.Max(1, JAtributos.Remate -= 10);
                    JAtributos.Defesa = Math.Max(1, JAtributos.Defesa -= 10);
                    JAtributos.Passe = Math.Max(1, JAtributos.Passe -= 10);
                    break;
                }
            case "Média":
                {
                    DanoAcumulado += 5;
                    Cansaco = 100;
                    MesRepouso = 1;
                    JAtributos.Rapidez = Math.Max(1, JAtributos.Rapidez -= 5);
                    JAtributos.Forca = Math.Max(1, JAtributos.Forca -= 5);
                    JAtributos.Remate = Math.Max(1, JAtributos.Remate -= 5);
                    JAtributos.Defesa = Math.Max(1, JAtributos.Defesa -= 5);
                    JAtributos.Passe = Math.Max(1, JAtributos.Passe -= 5);
                    break;
                }
            default:
                {
                    Cansaco = 100;
                    break;
                }
        }
        return true;

    }

    public int GeraMedia()
    {
        //return (JAtributos.Rapidez + JAtributos.Forca + JAtributos.Inteligencia + JAtributos.Investimento + JAtributos.Remate + JAtributos.Passe + JAtributos.Defesa) / 7;
        switch (Posicao)
        {
            case "Defesa": return (JAtributos.Rapidez + JAtributos.Forca + JAtributos.Inteligencia + JAtributos.Investimento + JAtributos.Remate + JAtributos.Passe + JAtributos.Defesa * 2) / 7;

            default: return (JAtributos.Rapidez + JAtributos.Forca + JAtributos.Inteligencia + JAtributos.Investimento + JAtributos.Remate + JAtributos.Passe + JAtributos.Defesa) / 7;
        }
    }
    #region metodos de treino
    public bool TreinaREMATE(int multiplicador) // Treina remate do jogador, retorna true se o jogador treinar
    {
        if (Cansaco + (multiplicador * 3) > 100)
        {
            return false;
        }
        Cansaco += multiplicador * 3;
        if (Idade > Prime) multiplicador = multiplicador / 2;
        JAtributos.Remate += multiplicador;
        AprovTreinador += 0.5f;

        ChanceLesao = Math.Min(75, ChanceLesao + 1);

        if (JAtributos.Remate > 100) JAtributos.Remate = 100;

        return true; //este return true diz que o jogador de facto treinou
    }
    public bool TreinaPASSE(int multiplicador) // Treina passe do jogador, retorna true se o jogador treinar
    {
        if (Cansaco + (multiplicador * 3) > 100)
        {
            return false;
        }
        Cansaco += multiplicador * 3;
        if (Idade > Prime) multiplicador = multiplicador / 2;
        JAtributos.Passe += multiplicador;
        AprovTreinador += 0.5f;

        ChanceLesao = Math.Min(75, ChanceLesao + 1);

        if (JAtributos.Passe > 100) JAtributos.Passe = 100;

        return true;
    }
    public bool TreinaDEFESA(int multiplicador) // Treina defesa do jogador, retorna true se o jogador treinar
    {
        if (Cansaco + (multiplicador * 3) > 100)
        {
            return false;
        }
        Cansaco += multiplicador * 3;
        if (Idade > Prime) multiplicador = multiplicador / 2;
        JAtributos.Defesa += multiplicador;
        AprovTreinador += 0.5f;

        ChanceLesao = Math.Min(75, ChanceLesao + 1);

        if (JAtributos.Defesa > 100) JAtributos.Defesa = 100;

        return true;
    }

    public bool TreinaFORCA(int multiplicador) // Treina forca do jogador, retorna true se o jogador treinar
    {
        if (Cansaco + (multiplicador * 3) > 100)
        {
            return false;
        }
        Cansaco += multiplicador * 3;
        if (Idade > Prime) multiplicador = multiplicador / 2;
        JAtributos.Forca += multiplicador;
        AprovTreinador += 0.5f;

        ChanceLesao = Math.Min(75, ChanceLesao + 1);

        if (JAtributos.Forca > 100) JAtributos.Forca = 100;

        return true;
    }

    public bool TreinaRAPIDEZ(int multiplicador) // Treina Rapidez do jogador, retorna true se o jogador treinar
    {
        if (Cansaco + (multiplicador * 3) > 100)
        {
            return false;
        }
        Cansaco += multiplicador * 3;
        if (Idade > Prime) multiplicador = multiplicador / 2;
        JAtributos.Rapidez += multiplicador;
        AprovTreinador += 0.5f;

        ChanceLesao = Math.Min(75, ChanceLesao + 1);

        if (JAtributos.Rapidez > 100) JAtributos.Rapidez = 100;

        return true;
    }
    #endregion

    #endregion

    //---------Override--------------------------
    public override string ToString()
    {
        return $"Nome:{Nome}\nPosicao:{Posicao}\nRapidez:{JAtributos.Rapidez}\nForça:{JAtributos.Forca}\nInteligencia:{JAtributos.Inteligencia}";
    }

}
