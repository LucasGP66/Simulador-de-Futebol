using Firebase.Firestore;
using System.Collections.Generic;

[System.Serializable]
[FirestoreData]
public class Turno
{
    //--------propriedades---------------------------
    [FirestoreProperty] public int TurnoNum { get; set; }
    [FirestoreProperty] public int Epoca { get; set; }
    [FirestoreProperty] public bool isTreinou { get; set; }
    [FirestoreProperty] public List<string> Mes { get; set; }

    [FirestoreProperty] public bool IsJanelaTransf { get; set; }

    //--------construtores----------------------------c
    public Turno() { }
    public Turno(int turno, int epoca)
    {
        TurnoNum = turno;
        Epoca = epoca;
        isTreinou = false;
        Mes = new List<string> { "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho" };
        IsJanelaTransf = false;
    }
    //---------Metodos---------------------------
    public void AvancaTurno()
    {
        TurnoNum++;
        if (TurnoNum > 11)
        {
            TurnoNum = 0;
            Epoca++;
        }
    }

}
