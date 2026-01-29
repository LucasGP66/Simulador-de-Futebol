using System;
using Firebase.Firestore;
using UEngine = UnityEngine;

//Contem todas as classes

[FirestoreData]
public class Atributos //guarda info dos atributos e gera estatisticas
{
    //------------Propriedades
    [FirestoreProperty] public int Rapidez { get; set; }
    [FirestoreProperty] public int Forca { get; set; }
    [FirestoreProperty] public int Inteligencia { get; set; }
    [FirestoreProperty] public int Egoismo { get; set; }
    [FirestoreProperty] public int Investimento { get; set; }
    [FirestoreProperty] public int Remate { get; set; }
    [FirestoreProperty] public int Passe { get; set; }
    [FirestoreProperty] public int Defesa { get; set; }
    [FirestoreProperty] public float MultiplicadorGolos { get; set; }
    [FirestoreProperty] public float MultiplicadorAssists { get; set; }
    [FirestoreProperty] public float MultiplicadorDefesas { get; set; }
    [FirestoreProperty] public float MultTempoDeJogo { get; set; }
    [FirestoreProperty] public float MultReputacaoLiga { get; set; }
    //-------construtor--------------------------------------
    public Atributos() { }
    public Atributos(string Posicao)
    {
        Rapidez = UEngine.Random.Range(0, 100);
        Forca = UEngine.Random.Range(0, 100);
        Inteligencia = UEngine.Random.Range(0, 100);
        Egoismo = UEngine.Random.Range(0, 100);
        Investimento = UEngine.Random.Range(0, 100);
        MultTempoDeJogo = 1;
        switch (Posicao)
        {
            case "Avançado":
                {
                    MultiplicadorGolos = 2f;
                    MultiplicadorAssists = 1f;
                    MultiplicadorDefesas = 0.5f;

                    Remate = UEngine.Random.Range(10, 30);
                    Passe = UEngine.Random.Range(5, 15);
                    Defesa = UEngine.Random.Range(0, 10);

                    break;
                }
            case "Extremo":
                {
                    MultiplicadorGolos = 1.5f;
                    MultiplicadorAssists = 1.5f;
                    MultiplicadorDefesas = 0.6f;

                    Remate = UEngine.Random.Range(15, 25);
                    Passe = UEngine.Random.Range(15, 20);
                    Defesa = UEngine.Random.Range(0, 25);

                    break;
                }
            case "Médio":
                {
                    MultiplicadorGolos = 0.8f;
                    MultiplicadorAssists = 2f;
                    MultiplicadorDefesas = 1.2f;

                    Remate = UEngine.Random.Range(5, 15);
                    Passe = UEngine.Random.Range(10, 30);
                    Defesa = UEngine.Random.Range(5, 15);

                    break;
                }
            case "Defesa":
                {
                    MultiplicadorGolos = 0.3f;
                    MultiplicadorAssists = 0.8f;
                    MultiplicadorDefesas = 3f;

                    Remate = UEngine.Random.Range(0, 5);
                    Passe = UEngine.Random.Range(5, 15);
                    Defesa = UEngine.Random.Range(10, 30);
                    break;
                }
        }
    }
    //-------Metodos-----------------------------------------
    public void GeraEstatisticas(Jogador Player)//gera golos, assistencias e defesas, levando em conta todos os atributos
    {
        //multiplicadores do clube
        float ModificadorClube = 0f;
        if (Player.RetornaClube().CapcidadeBase - 5 > Player.GeraMedia()) //Se o clube tem uma media de capacidade maior que a do jogador
        {
            ModificadorClube = 0.8f;
        }
        else
        {
            ModificadorClube = 1.1f;
        }

        //obtem a reputação da liga
        float modificadorLiga = Player.RetornaLiga().MultReputacao;
        if (modificadorLiga == 0) modificadorLiga = 1;

        //multiplicadores randomizados
        float randG = UEngine.Random.Range(.5f, 1.5f);
        float randA = UEngine.Random.Range(.5f, 1.5f);
        float randD = UEngine.Random.Range(.5f, 1.5f);

        //cria ego normalizado, quanto menor form menor as assistencias
        var egoNormalizado = Math.Max(1, 100 - Egoismo) / 100;

        //calculo principal, define estatisticas do turno
        Player.JStats.GolosTurno += MultiplicadorGolos * (0.70f * (Rapidez / 10) + 0.75f * (Forca / 10) * (Remate / 10)) * randG;
        Player.JStats.AssistTurno += MultiplicadorAssists * (0.80f * (Investimento / 10) + (Inteligencia / 10) * (Passe / 10)) * randA;
        Player.JStats.DefesaTurno += MultiplicadorDefesas * (0.60f * Forca + 0.40f * (Inteligencia / 10) + 0.60f * Investimento * Defesa) * randD;

        //limpa e arrendonda as stats de turno - adiciona tempo do jogo
        Player.JStats.GolosTurno = Player.JStats.GolosTurno / 30 * MultTempoDeJogo * ModificadorClube;
        Player.JStats.AssistTurno = Player.JStats.AssistTurno /30 * MultTempoDeJogo * ModificadorClube;
        Player.JStats.DefesaTurno = Player.JStats.DefesaTurno / 150 * MultTempoDeJogo * ModificadorClube;

        //gera a reputação
        Player.Reputacao += (int)((Player.JStats.GolosTurno * 3 + Player.JStats.AssistTurno * 3 + Player.JStats.DefesaTurno * 2) * modificadorLiga);

        //soma as statisticas de epoca
        Player.JStats.NumGolos += (int)Player.JStats.GolosTurno;
        Player.JStats.NumAssistencias += (int)Player.JStats.AssistTurno;
        Player.JStats.NumDefesas += (int)Player.JStats.DefesaTurno;

        //soma aos totais de carreira
        Player.JStats.TotalGolos += (int)Player.JStats.GolosTurno;
        Player.JStats.TotalAssists += (int)Player.JStats.AssistTurno;
        Player.JStats.TotalDefesas += (int)Player.JStats.DefesaTurno;

        //as stats de turno são postas a 0 em TurnoManager depois de atualizar o resumo

        //adiciona 4 jogos ao contador
        Player.JStats.JogosEpoca += 4;

        //gera media de desempenho
        Player.JStats.GeraMediaDesempenho();
    }
    public void LimpaEstatisticas(Jogador Player)//guarda estatisticas importantes, limpa propriedades, e retira 1 ano ao contrato
    {
        string stats = $"{Player.Clube}|{Player.JStats.NumGolos.ToString()}|{Player.JStats.NumAssistencias.ToString()}|{Player.JStats.NumDefesas.ToString()}";
        Player.JStats.StatsHistorico.Add(stats);

        //bonus de reputação
        if (Player.JStats.NumGolos >= 20) Player.Reputacao += 150;
        if (Player.JStats.NumGolos >= 30) Player.Reputacao += 300;
        if (Player.JStats.AssistTurno >= 15) Player.Reputacao += 150;
        if (Player.JStats.NumGolos >= 200) Player.Reputacao += 150;

        Player.JStats.NumGolos = 0;
        Player.JStats.NumAssistencias = 0;
        Player.JStats.NumDefesas = 0;
        Player.JStats.JogosEpoca = 0;
        Player.JStats.PontuacaoEpocaPassada = Player.JStats.Pontuacao;
        Player.JStats.Pontuacao = 6.5f;
        Player.Contrato -= 1;
    }

    //public int GeraMedia()
    //{
    //    return (Rapidez + Forca + Inteligencia + Investimento + Remate + Passe + Defesa) / 7;
    //}
}
