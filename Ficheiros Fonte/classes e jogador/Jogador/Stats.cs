using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class Estatisticas //guarda info das estatisticas
{
    [FirestoreProperty] public float NumGolos { get; set; }// estatisticas que guardam o total de uma época
    [FirestoreProperty] public float NumAssistencias { get; set; }
    [FirestoreProperty] public float NumDefesas { get; set; }
    [FirestoreProperty] public float GolosTurno { get; set; }// estatisticas que guardam o total de um turno
    [FirestoreProperty] public float AssistTurno { get; set; }
    [FirestoreProperty] public float DefesaTurno { get; set; }
    [FirestoreProperty] public int JogosEpoca { get; set; }// jogos de uma época
    [FirestoreProperty] public float PontuacaoEpocaPassada { get; set; }// pontuação da época passada, talvez inutil
    [FirestoreProperty] public float Pontuacao { get; set; } //pontuação da epoca atual
    [FirestoreProperty] public List<float> PontuacaoList { get; set; }//lista de pontuação ao longo da carreira
    [FirestoreProperty] public List<string> ClubesLista { get; set; }//lista de clubes que o jogador esteve, não se repetem clubes
    [FirestoreProperty] public int TotalGolos { get; set; }// total de estatisticas ao longo da carreira
    [FirestoreProperty] public int TotalAssists { get; set; }
    [FirestoreProperty] public int TotalDefesas { get; set; }


    [FirestoreProperty] public List<string> StatsHistorico { get; set; }

    //construtores----------------------------------------
    public Estatisticas() { }

    //metodos---------------------------------------------

    public void GeraMediaDesempenho() //gera a media de desempenho de 1 mes
    {
        //gera a pontuação de um mês
        float PontuacaoT;
        PontuacaoT = (float)(NumGolos * 3 + NumAssistencias * 2 + 0.3 * DefesaTurno) / 4;
        PontuacaoT = 6 + (PontuacaoT / 5);
        if (PontuacaoT > 10) PontuacaoT = 10;
        else if (PontuacaoT < 0) PontuacaoT = 0;

        //adiciona essa pontuação a pontuação existente
        Pontuacao = PontuacaoT;
        //Pontuacao = (PontuacaoArray.Average() * JogosEpoca) + (PontuacaoT * 4) / (JogosEpoca + 4);
        PontuacaoList.Add(PontuacaoT);
    }

    public void AdicionaClubeHistorico(string clube)//adiciona um clube ao historico de clubes
    {
        if (ClubesLista.Contains(clube)) return;
        ClubesLista.Add(clube);
    }
}


